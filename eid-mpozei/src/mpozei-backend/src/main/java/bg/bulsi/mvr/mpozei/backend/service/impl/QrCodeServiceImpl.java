package bg.bulsi.mvr.mpozei.backend.service.impl;

import bg.bulsi.mvr.common.exception.FaultMVRException;
import bg.bulsi.mvr.common.exception.ValidationMVRException;
import bg.bulsi.mvr.mpozei.backend.dto.OtpCodeDTO;
import bg.bulsi.mvr.mpozei.backend.service.QrCodeService;
import com.google.zxing.BarcodeFormat;
import com.google.zxing.WriterException;
import com.google.zxing.client.j2se.MatrixToImageWriter;
import com.google.zxing.common.BitMatrix;
import com.google.zxing.qrcode.QRCodeWriter;
import lombok.extern.slf4j.Slf4j;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Qualifier;
import org.springframework.cache.Cache;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import javax.imageio.ImageIO;
import java.awt.image.BufferedImage;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.util.*;

import static bg.bulsi.mvr.common.exception.ErrorCode.*;
import static bg.bulsi.mvr.common.util.ValidationUtil.assertNotBlank;
import static bg.bulsi.mvr.common.util.ValidationUtil.assertNotNull;

@Slf4j
@Service
public class QrCodeServiceImpl implements QrCodeService {
    private final static String QR_TEMPLATE = "otpauth://totp/%s?secret=%s&issuer=%s";

    @Autowired
    @Qualifier("otpCodeCache")
    private Cache otpCodesCache;

    @Autowired
    @Qualifier("applicationIdCache")
    private Cache applicationIdCache;

    @Override
    public String generateQrCodeImage(UUID applicationId, String issuer) {
        assertNotNull(applicationId, APPLICATION_ID_CANNOT_BE_NULL);
        assertNotBlank(issuer, ISSUER_CANNOT_BE_BLANK);

        try {
            String otpCode = UUID.randomUUID().toString();
            String qrText = String.format(QR_TEMPLATE,"eIdentity", otpCode, issuer);

            QRCodeWriter qrCodeWriter = new QRCodeWriter();
            BitMatrix bitMatrix = qrCodeWriter.encode(qrText, BarcodeFormat.QR_CODE, 250, 250);
            byte[] image = getImageAsBytes(MatrixToImageWriter.toBufferedImage(bitMatrix));
            String imageString = Base64.getEncoder().encodeToString(image);
            if (Objects.nonNull(applicationIdCache.get(applicationId.toString()))) {
                String oldCode = applicationIdCache.get(applicationId.toString()).get().toString();
                otpCodesCache.evictIfPresent(oldCode);
                applicationIdCache.evictIfPresent(applicationId.toString());
            }
            otpCodesCache.put(otpCode, applicationId.toString());
            applicationIdCache.put(applicationId.toString(), otpCode);
            return imageString;
        } catch (WriterException e) {
            throw new FaultMVRException(CANNOT_GENERATE_QR_CODE);
        }
    }

    @Override
    public UUID validateOtpCode(String otpCode) {
        OtpCodeDTO dto = parseOtpCode(otpCode);
        Cache.ValueWrapper applicationId = otpCodesCache.get(dto.getSecret());
        assertNotNull(applicationId, OTP_CODE_DOES_NOT_EXIST);
        log.info("OTP code found applicationId: " + applicationId.get().toString());
        return UUID.fromString(applicationId.get().toString());
    }

    @Override
    public boolean qrCodeExistsByApplicationId(UUID applicationId) {
        return applicationIdCache.get(applicationId) != null;
    }

    @Transactional
    @Override
    public void removeOtpCode(String otpCode) {
        OtpCodeDTO dto = parseOtpCode(otpCode);
        Cache.ValueWrapper applicationId = otpCodesCache.get(dto.getSecret());
        
        assertNotNull(applicationId, OTP_CODE_DOES_NOT_EXIST);
        log.info("Remove OTP code for applicationId: " + applicationId.get().toString());
        
		this.otpCodesCache.evictIfPresent(dto.getSecret());
		this.applicationIdCache.evictIfPresent(applicationId);
    }
    
    private byte[] getImageAsBytes(BufferedImage buffer) {
        try (ByteArrayOutputStream stream = new ByteArrayOutputStream()) {
            ImageIO.write(buffer, "jpg", stream);
            return stream.toByteArray();
        } catch (IOException e) {
            throw new FaultMVRException("Cannot convert qr code image to byte array", INTERNAL_SERVER_ERROR);
        }
    }

    private OtpCodeDTO parseOtpCode(String code) {
        try {
            String keysAndValues = code.split("\\?")[1];
            Map<String, String> map = new HashMap<>();
            Arrays.stream(keysAndValues.split("&")).forEach(keyValuePair -> {
                String key = keyValuePair.split("=")[0];
                String value = keyValuePair.split("=")[1];
                map.put(key, value);
            });

            OtpCodeDTO dto = new OtpCodeDTO();
            dto.setSecret(map.get("secret"));
            dto.setIssuer(map.get("issuer"));

            assertNotNull(dto.getSecret(), OTP_SECRET_CANNOT_BE_NULL);
            assertNotNull(dto.getIssuer(), OTP_ISSUER_CANNOT_BE_NULL);

            return dto;
        } catch (RuntimeException e) {
            log.error(e.getMessage());
            throw new ValidationMVRException(OTP_CODE_IS_NOT_VALID);
        }
    }
}
