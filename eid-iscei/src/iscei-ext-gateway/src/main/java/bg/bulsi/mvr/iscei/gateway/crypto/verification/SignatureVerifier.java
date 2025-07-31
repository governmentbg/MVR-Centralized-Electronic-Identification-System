package bg.bulsi.mvr.iscei.gateway.crypto.verification;

import java.security.cert.Certificate;

/**
 * Utility class for verifying digital signatures using a certificate's public key.
 */
public interface SignatureVerifier {

    /**
     * Verifies that the given signature is valid for the provided data using the public key
     * extracted from the specified certificate.
     *
     * @param data the original data bytes that were signed
     * @param signature the digital signature bytes to verify
     * @param certificate the certificate containing the public key for verification
     * @return {@code true} if the signature is valid; {@code false} otherwise
     */
	boolean verifySignature(byte[] data, byte[] signatureBytes, Certificate certificate) ;
}
