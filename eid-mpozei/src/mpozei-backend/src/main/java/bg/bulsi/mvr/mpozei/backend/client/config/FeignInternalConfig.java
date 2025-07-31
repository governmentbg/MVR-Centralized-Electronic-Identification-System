package bg.bulsi.mvr.mpozei.backend.client.config;

import feign.Retryer;
import feign.codec.Encoder;
import feign.codec.ErrorDecoder;
import feign.form.spring.SpringFormEncoder;
import org.springframework.beans.factory.ObjectFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.autoconfigure.http.HttpMessageConverters;
import org.springframework.cloud.openfeign.support.SpringEncoder;
import org.springframework.context.annotation.Bean;
import org.springframework.security.oauth2.client.OAuth2AuthorizeRequest;
import org.springframework.security.oauth2.client.registration.ClientRegistration;
import org.springframework.security.oauth2.client.registration.ClientRegistrationRepository;

import bg.bulsi.mvr.mpozei.backend.config.WebSecurityConfig;

public class FeignInternalConfig {
	
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
    public UserContextFeignInterceptor userContextInterceptor() {
        return new UserContextFeignInterceptor();
    }

    @Bean
    public ErrorDecoder errorDecoder() {
        return new FeignErrorDecoder();
    }

    @Bean
    public Retryer feignRetryer() {
        return new Retryer.Default();
    }

    @Bean
    public Encoder encoder(ObjectFactory<HttpMessageConverters> converters) {
        return new SpringFormEncoder(new SpringEncoder(converters));
    }
}
