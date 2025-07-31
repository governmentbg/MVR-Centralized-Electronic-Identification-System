package bg.bulsi.mvr.common.crypto;

import bg.bulsi.mvr.common.exception.FaultMVRException;
import org.bouncycastle.asn1.ASN1ObjectIdentifier;
import org.bouncycastle.asn1.x500.AttributeTypeAndValue;
import org.bouncycastle.asn1.x500.RDN;
import org.bouncycastle.asn1.x500.X500Name;
import org.bouncycastle.asn1.x500.style.BCStyle;
import org.bouncycastle.asn1.x500.style.IETFUtils;
import org.bouncycastle.jce.provider.BouncyCastleProvider;
import org.bouncycastle.openssl.PEMParser;
import org.bouncycastle.openssl.jcajce.JcaPEMWriter;
import org.bouncycastle.pkcs.PKCS10CertificationRequest;
import org.bouncycastle.util.io.pem.PemObject;
import org.bouncycastle.util.io.pem.PemReader;
import org.bouncycastle.util.io.pem.PemWriter;
import org.springframework.stereotype.Component;

import javax.security.auth.x500.X500Principal;
import java.io.*;
import java.security.*;
import java.security.cert.Certificate;
import java.security.cert.CertificateException;
import java.security.cert.CertificateFactory;
import java.security.cert.X509Certificate;
import java.security.interfaces.RSAPublicKey;
import java.security.spec.X509EncodedKeySpec;
import java.util.ArrayList;
import java.util.Base64;
import java.util.List;

@Component
public class CertificateProcessor {

	public CertificateProcessor () {
		Security.addProvider(new BouncyCastleProvider());
	}
	
	public X509Certificate extractCertificate(byte[] input) throws CertificateException, NoSuchProviderException {
		CertificateFactory certFactory = CertificateFactory.getInstance("X.509", "BC");
		ByteArrayInputStream is = new ByteArrayInputStream(Base64.getDecoder().decode(input));
		X509Certificate certificate = (X509Certificate) certFactory.generateCertificate(is);
		
		return certificate;
	}
	
	public PKCS10CertificationRequest extractCsr(byte[] input) throws IOException  {
		PKCS10CertificationRequest csr = null;
		//try to parse PEM CSR
		try (PEMParser pem = new PEMParser(new InputStreamReader(new ByteArrayInputStream(input)))) {
	        csr = (PKCS10CertificationRequest) pem.readObject();
		}
		
		//try to parse DER CSR
		if(csr == null) {
			csr = new PKCS10CertificationRequest(input);
		}
		
		return csr;
	}


    public String convertCsrToPEM(PKCS10CertificationRequest csr) throws IOException {
        // Create a StringWriter to capture the PEM output
        StringWriter stringWriter = new StringWriter();

        // Use PemWriter to write the CSR in PEM format
        try (PemWriter pemWriter = new PemWriter(stringWriter)) {
            PemObject pemObject = new PemObject("CERTIFICATE REQUEST", csr.getEncoded());
            pemWriter.writeObject(pemObject);
        }

        // Return the PEM string
        return stringWriter.toString();
    }
	
	public String getFirstRdnValue(X500Name x500Name, ASN1ObjectIdentifier oid) {
		RDN[] rdns =  x500Name.getRDNs(oid);
		
		if (rdns.length > 0) {
		    AttributeTypeAndValue attrTypeAndValue = rdns[0].getFirst();
		    return attrTypeAndValue.getValue().toString();
		}
		
		return null;
	}
	
	public boolean verifySignature(byte[] data, byte[] signature, Certificate certificate) {
		Signature verifier = null;
		try {
			verifier = Signature.getInstance("SHA256withRSA", "BC");
		} catch (NoSuchAlgorithmException | NoSuchProviderException e) {
			System.out.println("Could not inititialize SignatureVerifier");

			e.printStackTrace();
		}
		try {
			verifier.initVerify(certificate);
		} catch (InvalidKeyException e) {
			System.out.println("Invalid PublicKey");
			
			e.printStackTrace();
		}
		
		try {
			verifier.update(data);
			
			return verifier.verify(signature);
		} catch (SignatureException e) {
			System.out.println("Could not verify signature");
			
			e.printStackTrace();
		}
		
		return false;
	}
	
	public RSAPublicKey readX509PublicKey(File file) throws Exception {
		//TODO: How to know which algorithm should we pass, must be provided in the request?
		KeyFactory factory = KeyFactory.getInstance("RSA");

		try (FileReader keyReader = new FileReader(file); PemReader pemReader = new PemReader(keyReader)) {

			PemObject pemObject = pemReader.readPemObject();
			byte[] content = pemObject.getContent();
			X509EncodedKeySpec pubKeySpec = new X509EncodedKeySpec(content);
			return (RSAPublicKey) factory.generatePublic(pubKeySpec);
		}
	}
	
	public boolean compareCertificate(X509Certificate cert1, X509Certificate cert2) {
		if(cert1 == null || cert2 == null) {
			return false;
		}
		
		return cert1.equals(cert2);
	}

	public String getIssuerCN(X509Certificate certificate) {
		X500Principal principal = certificate.getIssuerX500Principal();
		X500Name x500Name = new X500Name(principal.getName());
		RDN[] rdns = x500Name.getRDNs(BCStyle.CN);
		List<String> names = new ArrayList<>();
		for (RDN rdn : rdns) {
			String name = IETFUtils.valueToString(rdn.getFirst().getValue());
			names.add(name);
		}

		return names.get(0);
	}

	public String getSubjectCN(X509Certificate certificate) {
		X500Principal principal = certificate.getSubjectX500Principal();
		X500Name x500Name = new X500Name(principal.getName());
		RDN[] rdns = x500Name.getRDNs(BCStyle.CN);
		List<String> names = new ArrayList<>();
		for (RDN rdn : rdns) {
			String name = IETFUtils.valueToString(rdn.getFirst().getValue());
			names.add(name);
		}

		return names.get(0);
	}
	
	public String getSubjectCN(String subjectDn) {
		X500Name x500Name = new X500Name(subjectDn);
		RDN[] rdns = x500Name.getRDNs(BCStyle.CN);
		List<String> names = new ArrayList<>();
		for (RDN rdn : rdns) {
			String name = IETFUtils.valueToString(rdn.getFirst().getValue());
			names.add(name);
		}

		return names.get(0);
	}
	
	public String getSubjectEid(X509Certificate certificate) {
		  X500Principal principal = certificate.getSubjectX500Principal();
		    X500Name x500Name = new X500Name(principal.getName());
		    RDN[] rdns = x500Name.getRDNs(BCStyle.SERIALNUMBER);
		    List<String> names = new ArrayList<>();
		    for (RDN rdn : rdns) {
		        String name = IETFUtils.valueToString(rdn.getFirst().getValue());
		        names.add(name);
		    }

		 return names.get(0).substring("PI:BG-".length());
	}

	public static String convertDerBase64ToPEM(String base64Cert) {
		StringWriter stringWriter = new StringWriter();
		try (JcaPEMWriter pemWriter = new JcaPEMWriter(stringWriter)) {
			// Decode Base64 DER string
			byte[] derBytes = java.util.Base64.getDecoder().decode(base64Cert);

			// Convert to X509Certificate
			CertificateFactory certFactory = CertificateFactory.getInstance("X.509");
			X509Certificate cert = (X509Certificate) certFactory.generateCertificate(new ByteArrayInputStream(derBytes));

			// Write to PEM string
			pemWriter.writeObject(cert);
		} catch (Exception e) {
			throw new FaultMVRException(e);
		}
		return stringWriter.toString();
	}
}
