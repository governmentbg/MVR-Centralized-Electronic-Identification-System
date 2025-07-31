package bg.bulsi.mvr.common.crypto;

import java.io.IOException;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.text.ParseException;
import java.util.Collection;
import java.util.Date;
import org.bouncycastle.asn1.ASN1Encodable;
import org.bouncycastle.asn1.ASN1UTCTime;
import org.bouncycastle.asn1.cms.Attribute;
import org.bouncycastle.asn1.cms.AttributeTable;
import org.bouncycastle.asn1.pkcs.PKCSObjectIdentifiers;
import org.bouncycastle.cms.*;
import org.bouncycastle.tsp.TSPAlgorithms;
import org.bouncycastle.tsp.TSPException;
import org.bouncycastle.tsp.TimeStampRequest;
import org.bouncycastle.tsp.TimeStampRequestGenerator;
import org.bouncycastle.tsp.TimeStampResponse;
import org.springframework.stereotype.Component;

/**
 * This class used used to handle {@link CMSSignedData} or {@link TimeStampRequest}/{@link TimeStampResponse}
 */
@Component
public class CryptoTimestampProcessor {

	/**
	 * Parse byte[] to represents a CMSSignedData object.
	 * 
	 * @throws CMSException
	 */
	public CMSSignedData parseCMSSignedData(byte[] input) throws CMSException {
		return new CMSSignedData(input); // Try parsing as CMSSignedData
	}

	/**
	 * Parse byte[] to a TimeStampResponse (TimeStampResp) object.
	 */
	public TimeStampResponse parseTimeStampResponse(byte[] input) throws TSPException, IOException {
		return new TimeStampResponse(input); // Try parsing as TimeStampResponse
	}

	public TimeStampRequest parseTimeStampRequest(byte[] input) throws NoSuchAlgorithmException {
		MessageDigest messageDigest = null;
		messageDigest = MessageDigest.getInstance("SHA-256");
		messageDigest.update(input);

		// Create the Timestamp Request
		TimeStampRequestGenerator tsqGenerator = new TimeStampRequestGenerator();
		tsqGenerator.setCertReq(true); // Request signer certificate in response

		// Generate the request using SHA-256
		return tsqGenerator.generate(TSPAlgorithms.SHA256, messageDigest.digest());
	}

	public Date getSigningTime(CMSSignedData signedData) throws ParseException {
		// Extract all signers
		Collection<SignerInformation> signers = signedData.getSignerInfos().getSigners();
		// Only 1 signer is expected
		AttributeTable signedAttributes = ((SignerInformation) signers.toArray()[0]).getSignedAttributes();
		if (signedAttributes != null) {
			// Get the signing time attribute (OID: 1.2.840.113549.1.9.5)
			Attribute signingTimeAttr = signedAttributes.get(PKCSObjectIdentifiers.pkcs_9_at_signingTime);

			if (signingTimeAttr != null) {
				ASN1Encodable value = signingTimeAttr.getAttrValues().getObjectAt(0);

				if (value instanceof ASN1UTCTime asn1UTCTime) {
					return asn1UTCTime.getDate();
				}
			}
		}
		return null; // Return null if signing time is not found
	}
}
