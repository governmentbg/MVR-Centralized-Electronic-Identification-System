package bg.bulsi.mvr.iscei.gateway.crypto.verification;

import java.io.IOException;
import java.math.BigInteger;
import java.security.InvalidKeyException;
import java.security.NoSuchAlgorithmException;
import java.security.Signature;
import java.security.SignatureException;
import java.security.cert.Certificate;
import java.util.Arrays;

import org.apache.commons.lang3.ArrayUtils;
import org.bouncycastle.crypto.signers.StandardDSAEncoding;
import org.bouncycastle.jcajce.provider.asymmetric.ec.BCECPublicKey;
import org.bouncycastle.jce.spec.ECParameterSpec;
import org.springframework.stereotype.Component;

import bg.bulsi.mvr.iscei.gateway.service.CertificateProcessor;
import lombok.extern.slf4j.Slf4j;

/**
 * The {@code EcdsaSignatureVerifier} class provides a utility method to verify digital signatures
 * created using the ECDSA algorithm with SHA-256.
 *
 * <p>The ECDSA signature scheme produces a signature that is an ASN.1 DER-encoded sequence containing
 * two integers (r and s). It is critical that the same signing parameters and data are used during verification.
 *
 * <p>This class is designed to work with Elliptic Curve (EC) public keys (for example,
 * {@link java.security.interfaces.ECPublicKey} such as Bouncy Castle's {@code BCECPublicKey}) and
 * expects the signature to be provided as a byte array (for instance, after decoding a Base64-encoded string).
 *
 * <p><b>Note:</b> For correct verification, the algorithm used in this verifier must exactly match
 * the one used during the signature generation process. This implementation assumes that the signature
 * was generated using "SHA256withECDSA".
 *
 * @see java.security.Signature
 * @see java.security.interfaces.ECPublicKey
 * @see org.bouncycastle.jce.provider.BouncyCastleProvider
 */
@Slf4j
@Component
public class EcdsaSignatureVerifier implements SignatureVerifier {

	@Override
	public boolean verifySignature(byte[] data, byte[] signature, Certificate certificate) {
		Signature verifier = null;
		
		BCECPublicKey publicKey = (BCECPublicKey) certificate.getPublicKey();
		ECParameterSpec ecParameterSpec = publicKey.getParameters();
		
		int partLength = signature.length / 2;
		byte[] r = Arrays.copyOfRange(signature, 0, partLength);
		byte[] s = Arrays.copyOfRange(signature, partLength, signature.length);
		
		BigInteger bigIntR = this.parseArrayToBigInteger(r);
		BigInteger bigIntS = this.parseArrayToBigInteger(s);
		
		try {
			signature = StandardDSAEncoding.INSTANCE.encode(ecParameterSpec.getN(), bigIntR, bigIntS);
		} catch (IOException e) {
			log.error(".verifySignature() Unable to encode signature", e);
			
			return false;
		}
		
		try {
			verifier = Signature.getInstance("SHA256withECDSA", CertificateProcessor.BOUNCY_CASTLE_PROVIDER);
			
		} catch (NoSuchAlgorithmException e) {
			log.error(".verifySignature() Could not inititialize SignatureVerifier", e);
			
			return false;
		}
		
		try {
			verifier.initVerify(certificate);
		} catch (InvalidKeyException e) {
			log.error(".verifySignature() Invalid PublicKey from X509 Certificate", e);
			
			return false;
		}
		
		try {
			verifier.update(data);
			
			return verifier.verify(signature);
		} catch (SignatureException e) {
			log.error(".verifySignature() Could not verify signature", e);
			
			return false;
		}
	}
	
    private BigInteger parseArrayToBigInteger(byte[] data) {
        // The first bit is sign -> 1 is minus
        final byte minusSign = (byte) 0b1000_0000;
        if ((data[0] & minusSign) == minusSign){
        	data = ArrayUtils.addFirst(data, (byte) 0); 
        }

        return new BigInteger(data);
    }
}
