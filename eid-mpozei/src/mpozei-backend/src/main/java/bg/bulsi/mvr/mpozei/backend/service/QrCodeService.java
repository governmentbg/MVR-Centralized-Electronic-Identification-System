package bg.bulsi.mvr.mpozei.backend.service;

import bg.bulsi.mvr.mpozei.backend.dto.OtpCodeDTO;

import java.util.UUID;

public interface QrCodeService {
    String generateQrCodeImage(UUID applicationId, String issuer);

    UUID validateOtpCode(String otpCode);

    boolean qrCodeExistsByApplicationId(UUID applicationId);
    
    void removeOtpCode(String otpCode);
}
