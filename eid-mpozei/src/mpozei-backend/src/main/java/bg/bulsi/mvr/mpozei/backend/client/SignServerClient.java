package bg.bulsi.mvr.mpozei.backend.client;

import bg.bulsi.mvr.mpozei.backend.client.config.SignServerConfig;
import bg.bulsi.mvr.mpozei.backend.dto.SignServerTimestampRequest;
import bg.bulsi.mvr.mpozei.backend.dto.SignServerTimestampResponse;

import org.springframework.cloud.openfeign.FeignClient;
import org.springframework.web.bind.annotation.PostMapping;

@FeignClient(name = "sign-server-client", url = "${services.sign-server-base-url}", configuration = SignServerConfig.class)
public interface SignServerClient {
	
	  @PostMapping("/rest/v1/workers/${services.sign-server-worker-name}/process")
	  SignServerTimestampResponse requestTimeStamp(SignServerTimestampRequest request);
}
