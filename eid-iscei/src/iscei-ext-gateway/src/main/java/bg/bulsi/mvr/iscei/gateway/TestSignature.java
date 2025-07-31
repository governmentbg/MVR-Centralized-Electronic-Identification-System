package bg.bulsi.mvr.iscei.gateway;

import java.io.IOException;
import java.security.NoSuchProviderException;
import java.security.Security;
import java.security.cert.CertificateException;
import java.security.cert.X509Certificate;
import java.util.Base64;

import org.bouncycastle.jce.provider.BouncyCastleProvider;

import bg.bulsi.mvr.iscei.gateway.crypto.verification.RsaPssSignatureVerifier;
import bg.bulsi.mvr.iscei.gateway.crypto.verification.SignatureVerifier;
import bg.bulsi.mvr.iscei.gateway.service.CertificateProcessor;

public class TestSignature {

	public static void main(String[] args) throws CertificateException, NoSuchProviderException, IOException {
		CertificateProcessor certificateProcessor = new CertificateProcessor();
		
		SignatureVerifier signatureVerifier = new RsaPssSignatureVerifier();
		Security.addProvider(new BouncyCastleProvider());


		X509Certificate x509Certificate3 = certificateProcessor.extractCertificate(certificate3.getBytes());
		
	    System.out.println("=================> x509Certificate3.getPublicKey() = " + x509Certificate3.getPublicKey());
	    System.out.println("=================> x509Certificate3.getPublicKey().getAlgorithm() = " + x509Certificate3.getPublicKey().getAlgorithm());
	    System.out.println("=================> signature = " + Base64.getDecoder().decode(signature).length);

	    
	    boolean isSignatureValid = signatureVerifier.verifySignature(
        		challenge.getBytes(),
        		Base64.getDecoder().decode(signature.getBytes()), 
        		x509Certificate3);
	    
//		   boolean isSignatureValid = certificateProcessor.verifySignature(
//	        		challenge.getBytes(),
//	        		Base64.getDecoder().decode(signature), 
//	        		x509Certificate3);
	    System.out.println("=================> isSignatureValid = " + isSignatureValid);

	}

}
