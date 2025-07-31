package bg.bulsi.mvr.audit_logger;

import java.security.SecureRandom;
import java.security.Security;
import java.util.Base64;
import javax.crypto.Cipher;
import javax.crypto.spec.SecretKeySpec;

import org.bouncycastle.jce.provider.BouncyCastleProvider;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Component;

@Component
public class EncryptionHelper {

	private static final String ENCRYPT_ALGORITHM = "AES/ECB/PKCS7Padding";
	private static final String AES_NAME = "AES";

    @Value("${logging.encryption-key:null}")
    private String secretPassword;
    
    public EncryptionHelper() {
		Security.addProvider(new BouncyCastleProvider());
    }
    
    public String encrypt(Object objectToEncrypt) throws Exception {
        if (secretPassword == null){
            throw new IllegalArgumentException("Encryption key is null");
        }

        if (objectToEncrypt == null){
            return null;
        }
    	
        // secret key from password
		SecretKeySpec secretKeySpec = new SecretKeySpec(secretPassword.getBytes(), AES_NAME);
    	
        Cipher cipher = Cipher.getInstance(ENCRYPT_ALGORITHM);

        cipher.init(Cipher.ENCRYPT_MODE, secretKeySpec);

        byte[] cipherText = cipher.doFinal(objectToEncrypt.toString().getBytes());

        // string representation, base64, send this string to other for decryption.
        return Base64.getEncoder().encodeToString(cipherText);
    }
	
    public Object decrypt(String textToDecrypt) throws Exception {
        if (secretPassword == null){
            throw new IllegalArgumentException("Encryption key is null");
        }

        if (textToDecrypt == null){
            return null;
        }
    	
        byte[] decode = Base64.getDecoder().decode(textToDecrypt.getBytes());

		SecretKeySpec secretKeySpec = new SecretKeySpec(secretPassword.getBytes(), AES_NAME);
    	
        Cipher cipher = Cipher.getInstance(ENCRYPT_ALGORITHM);

        cipher.init(Cipher.DECRYPT_MODE, secretKeySpec);

        return new String(cipher.doFinal(decode));
    }

    public byte[] getRandomNonce(int numBytes) {
        byte[] nonce = new byte[numBytes];
        new SecureRandom().nextBytes(nonce);
        return nonce;
    }
}
