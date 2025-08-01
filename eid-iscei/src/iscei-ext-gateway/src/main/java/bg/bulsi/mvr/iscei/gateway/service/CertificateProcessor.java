package bg.bulsi.mvr.iscei.gateway.service;

import java.io.ByteArrayInputStream;
import java.io.File;
import java.io.FileReader;
import java.io.IOException;
import java.security.KeyFactory;
import java.security.KeyStore;
import java.security.KeyStoreException;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.security.NoSuchProviderException;
import java.security.cert.Certificate;
import java.security.cert.CertificateException;
import java.security.cert.CertificateFactory;
import java.security.cert.X509Certificate;
import java.security.interfaces.RSAPublicKey;
import java.security.spec.X509EncodedKeySpec;
import java.util.ArrayList;
import java.util.Base64;
import java.util.Enumeration;
import java.util.List;
import java.util.UUID;
import java.util.stream.Collectors;

import javax.security.auth.x500.X500Principal;

import org.apache.commons.lang3.StringUtils;
import org.apache.commons.lang3.tuple.Pair;
import org.bouncycastle.asn1.x500.RDN;
import org.bouncycastle.asn1.x500.X500Name;
import org.bouncycastle.asn1.x500.style.BCStyle;
import org.bouncycastle.asn1.x500.style.IETFUtils;
import org.bouncycastle.jce.provider.BouncyCastleProvider;
import org.bouncycastle.util.io.pem.PemObject;
import org.bouncycastle.util.io.pem.PemReader;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.boot.ssl.SslBundle;
import org.springframework.boot.ssl.SslBundles;
import org.springframework.stereotype.Component;

import bg.bulsi.mvr.common.exception.ErrorCode;
import bg.bulsi.mvr.common.exception.ValidationMVRException;
import bg.bulsi.mvr.iscei.gateway.crypto.verification.SignatureVerifier;
import eu.europa.esig.dss.diagnostic.DiagnosticData;
import eu.europa.esig.dss.diagnostic.RevocationWrapper;
import eu.europa.esig.dss.enumerations.Indication;
import eu.europa.esig.dss.enumerations.SubIndication;
import eu.europa.esig.dss.enumerations.TokenExtractionStrategy;
import eu.europa.esig.dss.jaxb.object.Message;
import eu.europa.esig.dss.model.x509.CertificateToken;
import eu.europa.esig.dss.service.crl.OnlineCRLSource;
import eu.europa.esig.dss.service.ocsp.OnlineOCSPSource;
import eu.europa.esig.dss.simplecertificatereport.SimpleCertificateReport;
import eu.europa.esig.dss.spi.DSSUtils;
import eu.europa.esig.dss.spi.validation.CertificateVerifier;
import eu.europa.esig.dss.spi.validation.CommonCertificateVerifier;
import eu.europa.esig.dss.spi.validation.RevocationDataVerifier;
import eu.europa.esig.dss.spi.x509.CommonTrustedCertificateSource;
import eu.europa.esig.dss.spi.x509.TrustedCertificateSource;
import eu.europa.esig.dss.validation.CertificateValidator;
import eu.europa.esig.dss.validation.reports.CertificateReports;
import jakarta.annotation.PostConstruct;
import lombok.extern.slf4j.Slf4j;

@Slf4j
@Component
public class CertificateProcessor {

	public static final BouncyCastleProvider BOUNCY_CASTLE_PROVIDER = new BouncyCastleProvider();

	@Value("${mvr.disable-certificate-chain-validation:false}")
	private boolean disableCertificateChainValidation;
	
	@Value("${mvr.check-revocation-for-untrusted-chains:true}")
	private boolean checkRevocationForUntrustedChains;
	
	private TrustedCertificateSource isceiClientTrustoreRoots;
	
	private TrustedCertificateSource isceiClientTrustoreIntermeditates;

	private TrustedCertificateSource isceiClientTrustoreAll;
	
	@Autowired
	private SignatureVerifier signatureVerifier;
	
	@Autowired
	private SslBundles sslBundles;
	
	public X509Certificate extractCertificate(byte[] input) throws CertificateException, IOException, NoSuchProviderException {
		CertificateFactory certFactory = CertificateFactory.getInstance("X.509", BOUNCY_CASTLE_PROVIDER);
		ByteArrayInputStream is = new ByteArrayInputStream(Base64.getDecoder().decode(input));
		X509Certificate certificate = (X509Certificate) certFactory.generateCertificate(is);
		
		return certificate;
	}
	
	public boolean verifySignaturePreHashedChallenge(byte[] data, byte[] signature, Certificate certificate) {
		MessageDigest digest = null;
		try {
			digest = MessageDigest.getInstance("SHA-256", BOUNCY_CASTLE_PROVIDER);
		} catch (NoSuchAlgorithmException e) {
			log.error(".verifySignaturePreHashedChallenge() Cryptographic algorithm is not available", e);

			return false;
		}
		
		byte[] encodedhash = digest.digest(data);
		
		return this.verifySignature(encodedhash, signature, certificate);
	}
	
	public boolean verifySignature(byte[] data, byte[] signatureBytes, Certificate certificate) {
		return this.signatureVerifier.verifySignature(data, signatureBytes, certificate);
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
	
	public void validateCertificateChain(UUID eid, X509Certificate clientCertificate, List<String> certificateChain) {
		if(this.disableCertificateChainValidation) {
			log.info(".validateCertificateChain() " + disableCertificateChainValidation);

			return;
		}
		
		CertificateToken clientCertificateToken = new CertificateToken(clientCertificate);

		//Validate Certificate chain using ISCEI intermid and root Certificates
		this.validateCertificateChain(eid, clientCertificateToken, this.isceiClientTrustoreIntermeditates, this.isceiClientTrustoreRoots);
		
		//Validated External chain
		List<CertificateToken> clientChainTokens = certificateChain.stream()
				.map(DSSUtils::loadCertificateFromBase64EncodedString).toList();
		
		this.isChainInTruststore(eid, clientChainTokens, isceiClientTrustoreAll);
		
	    CommonTrustedCertificateSource clientAdjCertSource = new CommonTrustedCertificateSource();
	    this.splitChain(clientChainTokens).getRight().forEach(clientAdjCertSource::addCertificate);
	    
		//Validate Certificate chain using External Client intermid and ISCEI root Certificates
		this.validateCertificateChain(eid, clientCertificateToken, clientAdjCertSource, this.isceiClientTrustoreRoots);
	}

	private void validateCertificateChain(UUID eid,
			CertificateToken clientCertificateToken,
			TrustedCertificateSource intermidCerts,
			TrustedCertificateSource rootCerts) {
		CertificateVerifier certificateVerifier = new CommonCertificateVerifier();
	    certificateVerifier.setCrlSource(new OnlineCRLSource());
	    certificateVerifier.setOcspSource(new OnlineOCSPSource());
	    // The AIA source is used to collect certificates from external resources (AIA)
	    certificateVerifier.setAIASource(null);
//	    certificateVerifier.setCheckRevocationForUntrustedChains(checkRevocationForUntrustedChains);
	    
		// The adjunct certificate source is used to provide missing intermediate certificates
		// (not trusted certificates)
	    certificateVerifier.addAdjunctCertSources(intermidCerts);
	    
		// The trusted certificate source is used to provide trusted certificates
		// (the trust anchors where the certificate chain building should stop)		
	    certificateVerifier.addTrustedCertSources(rootCerts);
	    
	    RevocationDataVerifier revocationDataVerifier = RevocationDataVerifier.createDefaultRevocationDataVerifier();
	    revocationDataVerifier.setRevocationMaximumRevocationFreshness(31536000l);
//	    revocationDataVerifier.setSignatureMaximumRevocationFreshness(31536000l);
//	    revocationDataVerifier.setTimestampMaximumRevocationFreshness(31536000l);
///	    revocationDataVerifier.setCheckRevocationFreshnessNextUpdate(true);
	    
	    certificateVerifier.setRevocationDataVerifier(revocationDataVerifier);
	    
		// We create an instance of the CertificateValidator with the certificate
		CertificateValidator validator = CertificateValidator.fromCertificate(clientCertificateToken);
//		validator.setValidationContextExecutor(CompleteValidationContextExecutor.INSTANCE);
		validator.setCertificateVerifier(certificateVerifier);
		
		// Allows specifying which tokens need to be extracted in the diagnostic data (Base64).
		// Default : NONE)
		validator.setTokenExtractionStrategy(TokenExtractionStrategy.EXTRACT_CERTIFICATES_AND_REVOCATION_DATA);
		
		// We execute the validation
		CertificateReports certificateReports = validator.validate();
			
//		// We have 3 reports
//		// The diagnostic data which contains all used and static data
		DiagnosticData diagnosticData = certificateReports.getDiagnosticData();
		for (RevocationWrapper rev : diagnosticData.getAllRevocationData()) {
	        log.info(
	            ".validateCertificateChain() RevocationWrapper [CRLsource={}] [type={}] [thisUpdate={}] [nextUpdate={}] [productionDate={}] [sigValid={}] [origin={}] [hashExtPresent={}] [hashExtMatch={}] [expiredOnCRL={}] [archiveCutOff={}]",
	            rev != null ? rev.getSourceAddress() : null,
	            rev != null ? rev.getRevocationType() : null,
	            rev != null ? rev.getThisUpdate() : null,
	            rev != null ? rev.getNextUpdate() : null,
	    	    rev != null ? rev.getProductionDate() : null,
	            rev != null && rev.isSignatureValid(),
	            rev != null ? rev.getOrigin() : null,
	            rev != null && rev.isCertHashExtensionPresent(),
	            rev != null && rev.isCertHashExtensionMatch(),
	            rev != null ? rev.getExpiredCertsOnCRL() : null,
	            rev != null ? rev.getArchiveCutOff() : null
	        );
	    }
//
//		// The detailed report which is the result of the process of the diagnostic data and the validation policy
//		DetailedReport detailedReport = certificateReports.getDetailedReport();

		// The simple report is a summary of the detailed report or diagnostic data (more user-friendly)
		SimpleCertificateReport simpleReport = certificateReports.getSimpleReport();
		Indication clientCertificateIndication = simpleReport.getCertificateIndication(clientCertificateToken.getDSSIdAsString());
		SubIndication clientCertificateSubIndication = simpleReport.getCertificateSubIndication(clientCertificateToken.getDSSIdAsString());

		log.info(
				".validateCertificateChain() [eid={}] [DSSIdAsString={}] [serialNumber={}] [revocationReason={}] [Indication={}] [SubIndication={}]",
				eid,
				clientCertificateToken.getDSSIdAsString(), 
				clientCertificateToken.getSerialNumber(),
				simpleReport.getCertificateRevocationReason(clientCertificateToken.getDSSIdAsString()),
				clientCertificateIndication, 
				clientCertificateSubIndication);
		
		this.logReportValidations(eid, clientCertificateToken, simpleReport);
		
		if(Indication.PASSED != clientCertificateIndication) {
            throw new ValidationMVRException(ErrorCode.CERTIFICATE_CHAIN_GENERAL_FAILURE);
		}
	}
	
	
	private void logReportValidations(UUID eid, CertificateToken certificate, SimpleCertificateReport simpleReport) {
		List<Message> validationErrors = simpleReport.getX509ValidationErrors(certificate.getDSSIdAsString());
		if(!validationErrors.isEmpty()) {
			log.info(".logReportValidations() X509ValidationErrors [eid={}] [DSSIdAsString={}]] [serialNumber={}] [validationErrors={}]",
					eid,
					certificate.getDSSIdAsString(),
					certificate.getSerialNumber(),
					validationErrors.stream().map(m -> StringUtils.join(m.getKey(),"=",m.getValue())).collect(Collectors.joining(";")));
		}
		var validationWarnings = simpleReport.getX509ValidationWarnings(certificate.getDSSIdAsString());
		if(!validationWarnings.isEmpty()) {
			log.info(".logReportValidations() X509ValidationWarnings [eid={}] [DSSIdAsString={}]] [serialNumber={}] [validationWarnings={}]",
					eid,
					certificate.getDSSIdAsString(),
					certificate.getSerialNumber(),
					validationWarnings.stream().map(m -> StringUtils.join(m.getKey(),"=",m.getValue())).collect(Collectors.joining(";")));
		}
		
		var validationInfos= simpleReport.getX509ValidationInfo(certificate.getDSSIdAsString());
		if(!validationInfos.isEmpty()) {
			log.info(".logReportValidations() X509ValidationInfos [eid={}] [DSSIdAsString={}]] [serialNumber={}] [validationInfos={}]",
					eid,
					certificate.getDSSIdAsString(),
					certificate.getSerialNumber(),
					validationInfos.stream().map(m -> StringUtils.join(m.getKey(),"=",m.getValue())).collect(Collectors.joining(";")));		
		}
	}
		
	    /**
	     * Checks if every certificate in the externalChain is present in the truststore.
	     *
	     * @param externalChain the list of external certificate tokens (ordered from end-entity upwards)
	     * @param truststore    the trusted certificate source (e.g. loaded from your truststore)
	     * @return true if all certificates are found in the truststore; false otherwise.
	     */
	    public void isChainInTruststore(UUID eid, List<CertificateToken> externalChain, TrustedCertificateSource truststore) {
	    	if(externalChain.isEmpty()) {
	    		throw new ValidationMVRException(ErrorCode.CERTIFICATE_CHAIN_GENERAL_FAILURE);
	    	}
	    	
	        for (CertificateToken externalCert : externalChain) {
	            boolean found = truststore.getCertificates().stream()
	                    .anyMatch(trustedCert -> externalCert.getDSSId().equals(trustedCert.getDSSId()));
	            if (!found) {
	            	log.info(".isChainInTruststore() Certificate missing from truststore [eid={}] [externalCert={}]", eid, externalCert.getSubject().getPrincipal().getName());

	                throw new ValidationMVRException(ErrorCode.CERTIFICATE_CHAIN_GENERAL_FAILURE);
	            }
	        }
	    }

	    /**
	     * Splits the provided certificate chain into two objects:
	     * one containing self-signed certificate and the other containing non-self-signed certificates.
	     *
	     * @param chain List of CertificateToken representing the certificate chain.
	     * @return a Pair object containing self-signed certificates as the first element,
	     *         and the list of non-self-signed certificates as the second element.
	     */
	    public Pair<CertificateToken, List<CertificateToken>> splitChain(List<CertificateToken> certificateChainTokens) {
	        CertificateToken selfSigned = null;
	        List<CertificateToken> nonSelfSigned = new ArrayList<>();
	        
	        for (CertificateToken token : certificateChainTokens) {
	            X509Certificate cert = token.getCertificate();
	            // Check if self-signed: compare subject and issuer
	            if (cert.getSubjectX500Principal().equals(cert.getIssuerX500Principal())) {
	                selfSigned = token;
	            } else {
	                nonSelfSigned.add(token);
	            }
	        }
	        
	        return Pair.of(selfSigned, nonSelfSigned);
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
	
	@PostConstruct
	private void init() {
		log.info(".init() [disableCertificateChainValidation={}] [checkRevocationForUntrustedChains={}] ", disableCertificateChainValidation, checkRevocationForUntrustedChains);

		if (!disableCertificateChainValidation) {
			SslBundle sslBundle = sslBundles.getBundle("client-login-truststore");
			this.isceiClientTrustoreAll = new CommonTrustedCertificateSource();
			this.isceiClientTrustoreRoots = new CommonTrustedCertificateSource();
			this.isceiClientTrustoreIntermeditates = new CommonTrustedCertificateSource();
			KeyStore keyStore = sslBundle.getStores().getTrustStore();

			Enumeration<String> aliases;
			try {
				aliases = keyStore.aliases();
				while (aliases.hasMoreElements()) {
					String alias = aliases.nextElement();
					if (keyStore.isCertificateEntry(alias)) {
						X509Certificate cert = (X509Certificate) keyStore.getCertificate(alias);
						CertificateToken certificateToken = new CertificateToken(cert);
						if(certificateToken.isSelfIssued()) {
							isceiClientTrustoreRoots.addCertificate(new CertificateToken(cert));
						} else {
							isceiClientTrustoreIntermeditates.addCertificate(new CertificateToken(cert));
						}
						
						isceiClientTrustoreAll.addCertificate(certificateToken);
					}
				}
			} catch (KeyStoreException e) {
				log.error(".CertificateProcessor() Unable to read certificates from Trustore", e);
			}
		}
  }
//	public boolean compareCertificate(X509Certificate cert1, String cert2Str) throws CertificateException, IOException {
//		X509Certificate cert2 = this.extractCertificate(cert2Str);
//		
//		return compareCertificate(cert1, cert2);
//	}
	
//	public void verifyOcspRequest() {
//		  // Load the certificate to be verified and the issuer certificate
//        FileReader certReader = new FileReader("path/to/certificate.pem");
//        PemReader pemCertReader = new PemReader(certReader);
//        PemObject pemCertObject = pemCertReader.readPemObject();
//        CertificateFactory certFactory = CertificateFactory.getInstance("X.509", "BC");
//        X509Certificate cert = (X509Certificate) certFactory.generateCertificate(new ByteArrayInputStream(pemCertObject.getContent()));
//
//        FileReader issuerReader = new FileReader("path/to/issuer_certificate.pem");
//        PemReader pemIssuerReader = new PemReader(issuerReader);
//        PemObject pemIssuerObject = pemIssuerReader.readPemObject();
//        X509Certificate issuerCert = (X509Certificate) certFactory.generateCertificate(new ByteArrayInputStream(pemIssuerObject.getContent()));
//
//        // Generate OCSP request
//        OCSPReqBuilder gen = new OCSPReqBuilder();
//        CertificateID id = new CertificateID(CertificateID.HASH_SHA1, issuerCert, cert.getSerialNumber());
//        gen.addRequest(id);
//        OCSPReq req = gen.build();
//
//        // Send OCSP request
//        String ocspUrl = "http://ocsp.responder.url"; // Replace with actual OCSP responder URL
//        CloseableHttpClient httpClient = HttpClients.createDefault();
//        HttpPost post = new HttpPost(ocspUrl);
//        post.setEntity(new ByteArrayEntity(req.getEncoded()));
//        post.setHeader("Content-Type", "application/ocsp-request");
//
//        HttpResponse response = httpClient.execute(post);
//        byte[] respBytes = response.getEntity().getContent().readAllBytes();
//
//        // Process OCSP response
//        OCSPResp ocspResp = new OCSPResp(respBytes);
//        if (ocspResp.getStatus() == OCSPResp.SUCCESSFUL) {
//            BasicOCSPResp basicResp = (BasicOCSPResp) ocspResp.getResponseObject();
//            SingleResp[] responses = basicResp.getResponses();
//
//            for (SingleResp singleResp : responses) {
//                CertificateID certID = singleResp.getCertID();
//                if (certID.equals(id)) {
//                    Object status = singleResp.getCertStatus();
//                    if (status == CertificateStatus.GOOD) {
//                        System.out.println("The certificate is valid.");
//                    } else if (status instanceof RevokedStatus) {
//                        RevokedStatus revokedStatus = (RevokedStatus) status;
//                        Date revocationDate = revokedStatus.getRevocationTime();
//                        System.out.println("The certificate is revoked since " + revocationDate);
//                    } else {
//                        System.out.println("The certificate status is unknown.");
//                    }
//                }
//            }
//        } else {
//            System.out.println("OCSP request failed with status: " + ocspResp.getStatus());
//        }
//	}

}
