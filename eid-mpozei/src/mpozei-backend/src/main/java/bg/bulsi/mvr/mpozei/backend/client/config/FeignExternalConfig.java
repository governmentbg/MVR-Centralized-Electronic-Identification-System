package bg.bulsi.mvr.mpozei.backend.client.config;

import feign.Logger;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.annotation.Bean;
import org.springframework.security.oauth2.client.OAuth2AuthorizeRequest;
import org.springframework.security.oauth2.client.registration.ClientRegistration;
import org.springframework.security.oauth2.client.registration.ClientRegistrationRepository;

import bg.bulsi.mvr.mpozei.backend.config.WebSecurityConfig;
import feign.Retryer;
import feign.codec.ErrorDecoder;

//Do not use UserContext here
public class FeignExternalConfig {
	
	@Autowired
	private ClientRegistrationRepository clientRegistrationRepository;
	
    @Bean
    public OAuth2FeignRequestInterceptor authRequestInterceptor() {
		ClientRegistration mpozeiClient = clientRegistrationRepository.findByRegistrationId(WebSecurityConfig.MPOZEI_CLIENT_REGISTRATION_ID);

		return new OAuth2FeignRequestInterceptor(OAuth2AuthorizeRequest
				.withClientRegistrationId(mpozeiClient.getRegistrationId())
				.principal(mpozeiClient.getClientName())
				.build());
    }
    
    @Bean
    public ErrorDecoder errorDecoder() {
        return new FeignDigitallErrorDecoder();
    }

    @Bean
    public Retryer feignRetryer() {
        return new Retryer.Default();
    }

    @Bean
    Logger.Level feignLoggerLevel() {
        return Logger.Level.FULL;
    }
}
