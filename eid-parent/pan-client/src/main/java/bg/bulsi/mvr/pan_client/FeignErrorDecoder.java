package bg.bulsi.mvr.pan_client;

import bg.bulsi.mvr.common.exception.ErrorCode;
import bg.bulsi.mvr.common.exception.FaultMVRException;
import bg.bulsi.mvr.common.exception.ValidationMVRException;
import com.fasterxml.jackson.databind.DeserializationFeature;
import com.fasterxml.jackson.databind.ObjectMapper;
import feign.Response;
import feign.codec.ErrorDecoder;
import lombok.extern.slf4j.Slf4j;
import org.apache.commons.lang3.ObjectUtils;

import java.nio.charset.StandardCharsets;

@Slf4j
public class FeignErrorDecoder implements ErrorDecoder {
	private ObjectMapper mapper;
	
	public FeignErrorDecoder() {
		this.mapper = new ObjectMapper();
        mapper.disable(DeserializationFeature.FAIL_ON_UNKNOWN_PROPERTIES);
	}
	
    @Override
    public Exception decode(String methodKey, Response response) {
        try {
            String responseString = new String(response.body().asInputStream().readAllBytes(), StandardCharsets.UTF_8);
            log.error("Error Response in Feign Client: " + responseString);

            DigitallErrorDTO detail = mapper.readValue(responseString, DigitallErrorDTO.class);
            return new ValidationMVRException(detail.getError(), ErrorCode.VALIDATION_ERROR);
        } catch (Exception e) {
            String error = ObjectUtils.firstNonNull(response.reason(), e.getMessage());
            if (error == null || error.isBlank()) {
                error = "Reason Unknown";
            }
            log.error("Exception: " + e);
            return new FaultMVRException(error, ErrorCode.INTERNAL_SERVER_ERROR);
        }
    }
}
