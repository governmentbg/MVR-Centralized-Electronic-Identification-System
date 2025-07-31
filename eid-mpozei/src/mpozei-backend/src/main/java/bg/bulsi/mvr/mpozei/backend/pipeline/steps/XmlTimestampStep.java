package bg.bulsi.mvr.mpozei.backend.pipeline.steps;

import bg.bulsi.mvr.common.crypto.CryptoTimestampProcessor;
import bg.bulsi.mvr.common.exception.ErrorCode;
import bg.bulsi.mvr.common.exception.FaultMVRException;
import bg.bulsi.mvr.common.pipeline.PipelineStatus;
import bg.bulsi.mvr.common.pipeline.Step;
import bg.bulsi.mvr.mpozei.backend.client.SignServerClient;
import bg.bulsi.mvr.mpozei.backend.dto.SignServerTimestampRequest;
import bg.bulsi.mvr.mpozei.backend.dto.SignServerTimestampResponse;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationStatus;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;

import org.bouncycastle.tsp.TSPException;
import org.bouncycastle.tsp.TimeStampRequest;
import org.bouncycastle.tsp.TimeStampResponse;
import org.springframework.stereotype.Component;

import java.io.IOException;
import java.security.NoSuchAlgorithmException;
import java.util.Base64;

@Slf4j
@Component
@RequiredArgsConstructor
public class XmlTimestampStep extends Step<AbstractApplication> {
	private final SignServerClient signServerClient;

	private final CryptoTimestampProcessor cryptoTimestampProcessor;

	@Override
	public AbstractApplication process(AbstractApplication application) {
		log.info("Application with id: {}", application.getId());

		SignServerTimestampRequest request = null;
		try {
			TimeStampRequest timeStampRequest = this.cryptoTimestampProcessor
					.parseTimeStampRequest(application.getApplicationXml().getBytes());

			request = new SignServerTimestampRequest(
					new String(Base64.getEncoder().encode(timeStampRequest.getEncoded())));
		} catch (IOException | NoSuchAlgorithmException e) {
			log.error(".process() ", "Could not create TimestampRequest Application with id: {}", application.getId());

			throw new FaultMVRException("TimestampRequest could not be extracted", ErrorCode.INTERNAL_SERVER_ERROR);
		}

		SignServerTimestampResponse response = signServerClient.requestTimeStamp(request);
		TimeStampResponse timeStampResponse = null;
		try {
			timeStampResponse = this.cryptoTimestampProcessor
					.parseTimeStampResponse(Base64.getDecoder().decode(response.getData()));
			
			// getTimeStampToken == CMSSignedData
			application.setDetachedSignature(Base64.getEncoder().encodeToString(timeStampResponse.getTimeStampToken().getEncoded()));
		} catch (TSPException | IOException e) {
			log.error(".process() ", "Could not create Timestamp Response Application with id: {}",
					application.getId());

			throw new FaultMVRException("TimestampResponse could not be extracted", ErrorCode.INTERNAL_SERVER_ERROR);
		}

		application.setStatus(ApplicationStatus.SIGNED);
		return application;
	}

	@Override
	public PipelineStatus getStatus() {
		return PipelineStatus.XML_TIMESTAMP;
	}
}
