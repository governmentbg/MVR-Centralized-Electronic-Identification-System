package bg.bulsi.mvr.pan_client;

import feign.Retryer;
import feign.codec.ErrorDecoder;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.boot.autoconfigure.condition.ConditionalOnProperty;
import org.springframework.cloud.openfeign.FeignClient;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.http.MediaType;
import org.springframework.scheduling.annotation.EnableAsync;
import org.springframework.security.oauth2.client.OAuth2AuthorizeRequest;
import org.springframework.security.oauth2.client.registration.ClientRegistration;
import org.springframework.security.oauth2.client.registration.ClientRegistrationRepository;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestParam;


@Configuration
@ConditionalOnProperty(value = "mvr.pan_client.enabled", havingValue = "true")
@EnableAsync
public class PanClientAutoConfiguration {

	@Value("${mvr.pan_client.client_registration_id}")
	private String clientRegistrationId;
	
	@Autowired
	private ClientRegistrationRepository clientRegistrationRepository;
	
    @Bean
    NotificationSender notificationSender() {
		return new NotificationSender();
	}

    @ConditionalOnProperty(value = "mvr.pan_client.register_system", havingValue = "true")
    @Bean
    SystemRegistratorService systemRegistratorService() {
		return new SystemRegistratorService();
	}
	
    class FeignDefaultConfig {
    	
    	@Bean
    	public OAuth2FeignRequestInterceptor authRequestInterceptor() {
    		ClientRegistration panClient = clientRegistrationRepository.findByRegistrationId(clientRegistrationId);

    		return new OAuth2FeignRequestInterceptor(OAuth2AuthorizeRequest
    				.withClientRegistrationId(panClient.getRegistrationId())
    				.principal(panClient.getClientName())
    				.build());
    	}
    	
        @Bean
        ErrorDecoder errorDecoder() {
	        return new FeignErrorDecoder();
	    }

        @Bean
        Retryer feignRetryer() {
	        return new Retryer.Default();
	    }
	}

	@FeignClient(name = "panFeignClient", url = "${mvr.pan_client.base_url}", configuration = FeignDefaultConfig.class)
	public interface PanFeignClient {
		
		  @PostMapping(value = "/api/v1/Notifications/systems", produces = MediaType.APPLICATION_JSON_VALUE, consumes = MediaType.APPLICATION_JSON_VALUE)
		  String createOrUpdateSystem(String system);
		  
		  @PostMapping(value = "/api/v1/Notifications/send", produces = MediaType.APPLICATION_JSON_VALUE, consumes = MediaType.APPLICATION_JSON_VALUE)
		  String sendNotification(Notification notification);

		  @PostMapping("/api/v1/Communications/direct-emails/send")
		  void sendDirectEmail(DirectEmailRequest request);
	}
}
