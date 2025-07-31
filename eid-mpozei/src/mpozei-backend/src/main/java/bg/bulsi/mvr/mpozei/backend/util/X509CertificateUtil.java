//package bg.bulsi.mvr.mpozei.backend.util;
//
//import bg.bulsi.mvr.common.exception.ErrorCode;
//import bg.bulsi.mvr.common.exception.FaultMVRException;
//import bg.bulsi.mvr.mpozei.backend.service.FileFormatService;
//import com.nimbusds.jose.util.X509CertUtils;
//import lombok.RequiredArgsConstructor;
//import lombok.extern.slf4j.Slf4j;
//import org.springframework.stereotype.Component;
//
//import java.io.ByteArrayInputStream;
//import java.security.cert.CertificateFactory;
//import java.security.cert.X509Certificate;
//import java.util.Base64;
//
//@Slf4j
//@Component
//@RequiredArgsConstructor
//public class X509CertificateUtil {
//    private final FileFormatService fileFormatService;
//    public static X509Certificate extractCertificate(String inputCertificate) {
//        try {
//            CertificateFactory certificateFactory = CertificateFactory.getInstance("X.509");
//            byte[] certificateByteArray = Base64.getDecoder().decode(inputCertificate);
//            return (X509Certificate) certificateFactory.generateCertificates(
//                            new ByteArrayInputStream(certificateByteArray))
//                    .iterator()
//                    .next();
//        } catch (Exception e) {
//            log.info("Cannot convert cert using Certificate Factory");
//            log.error(e.toString());
//        }
//        try {
//            String formattedCertificate = formatPemCertificate(inputCertificate);
//            X509Certificate cert =  X509CertUtils.parse(formattedCertificate);
//            return cert;
//        } catch (Exception e) {
//            log.info("Cannot convert cert using X509CertUtils");
//            log.error(e.toString());
//        }
//        throw new FaultMVRException("Certificate could not be extracted", ErrorCode.INTERNAL_SERVER_ERROR);
//    }
//
//    public static String formatPemCertificate(String certificate) {
//        if (!certificate.startsWith(X509CertUtils.PEM_BEGIN_MARKER)) {
//            StringBuilder stringBuilder = new StringBuilder();
//            stringBuilder.append(X509CertUtils.PEM_BEGIN_MARKER);
//            stringBuilder.append("\n");
//            stringBuilder.append(certificate);
//            stringBuilder.append("\n");
//            stringBuilder.append(X509CertUtils.PEM_END_MARKER);
//            certificate = stringBuilder.toString();
//            log.info("certificate was missing beginning and end");
//        }
//        return certificate;
//    }
//}
