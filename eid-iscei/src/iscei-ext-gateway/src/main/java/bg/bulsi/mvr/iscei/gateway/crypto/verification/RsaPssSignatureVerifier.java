package bg.bulsi.mvr.iscei.gateway.crypto.verification;

import java.security.InvalidKeyException;
import java.security.PublicKey;
import java.security.Signature;
import java.security.SignatureException;
import java.security.cert.Certificate;
import java.security.spec.MGF1ParameterSpec;
import java.security.spec.PSSParameterSpec;

import org.springframework.context.annotation.Primary;
import org.springframework.stereotype.Component;

import bg.bulsi.mvr.iscei.gateway.service.CertificateProcessor;
import lombok.extern.slf4j.Slf4j;

/**
 * The {@code RsaPssSignatureVerifier} class provides a utility method to verify digital signatures
 * that were created using the RSASSA-PSS algorithm with SHA-256 and MGF1 (SHA-256). 
 * 
 * <p>The RSASSA-PSS scheme is parameterised, and the following parameters are assumed by default:
 * <ul>
 *   <li><b>Hash Algorithm:</b> SHA-256</li>
 *   <li><b>Mask Generation Function (MGF):</b> MGF1 with SHA-256</li>
 *   <li><b>Salt Length:</b> 32 bytes (which is typical for SHA-256)</li>
 *   <li><b>Trailer Field:</b> 1 (the default value)</li>
 * </ul>
 *
 * <p>This class is designed to work with RSA public keys (typically instances of
 * {@link java.security.interfaces.RSAPublicKey} such as Bouncy Castle's {@code BCRSAPublicKey}) and
 * expects the signature to be provided as a byte array (for example, decoded from a Base64-encoded string).
 *
 * <p><b>Note:</b> For correct verification, the RSASSA-PSS parameters used in this verifier must match
 * exactly with those used during the signature generation.
 *
 * @see java.security.Signature
 * @see java.security.spec.PSSParameterSpec
 * @see org.bouncycastle.jce.provider.BouncyCastleProvider
 */
@Slf4j
@Primary
@Component
public class RsaPssSignatureVerifier implements SignatureVerifier {

	@Override
	public boolean verifySignature(byte[] data, byte[] signatureBytes, Certificate certificate) {
        // Extract the public key from the certificate.
		PublicKey publicKey = certificate.getPublicKey();
		
        Signature signatureVerifier = null;
		try {
			signatureVerifier = Signature.getInstance("RSASSA-PSS", CertificateProcessor.BOUNCY_CASTLE_PROVIDER);
			
	        PSSParameterSpec parameterSpec = new PSSParameterSpec(
	                "SHA-256",                   // Hash algorithm
	                "MGF1",                      // Mask Generation Function
	                new MGF1ParameterSpec("SHA-256"), // MGF hash algorithm
	                32,                          // Salt length in bytes (for SHA-256, typically 32)
	                1                            // Trailer field (default is 1)
	            );
			
			signatureVerifier.setParameter(parameterSpec);
		} catch (Exception e) {
			log.error(".verifySignature() Unable to encode signature", e);
			
			return false;
		}
		
        try {
			signatureVerifier.initVerify(publicKey);
		} catch (InvalidKeyException e) {
			log.error(".verifySignature() Could not inititialize SignatureVerifier", e);
			
			return false;
		}

        // Update the Signature object with the original data bytes.
        // Verify the signature.
		try {
			signatureVerifier.update(data);
			
			return signatureVerifier.verify(signatureBytes);
		} catch (SignatureException e) {
			log.error(".verifySignature() Could not verify signature", e);
			
			return false;
		}
    }

}
