package bg.bulsi.mvr.mpozei.backend;

import bg.bulsi.mvr.audit_logger.BaseAuditLogger;
import bg.bulsi.mvr.common.config.security.UserContext;
import bg.bulsi.mvr.common.config.security.UserContextHolder;
import bg.bulsi.mvr.common.rabbitmq.consumer.RabbitConsumerConfig;
import bg.bulsi.mvr.mpozei.config.AuditorAwareConfig;
import bg.bulsi.mvr.mpozei.backend.client.config.OAuth2FeignRequestInterceptor;
import bg.bulsi.mvr.pan_client.PanClientAutoConfiguration.PanFeignClient;
import jakarta.annotation.PostConstruct;

import static bg.bulsi.mvr.mpozei.contract.dto.CertificateStatus.CREATED;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.ArgumentMatchers.anyString;
import static org.mockito.Mockito.mock;
import static org.mockito.Mockito.when;

import org.junit.jupiter.api.BeforeAll;
import org.springframework.amqp.rabbit.connection.ConnectionFactory;
import org.springframework.boot.autoconfigure.EnableAutoConfiguration;
import org.springframework.boot.autoconfigure.amqp.RabbitAutoConfiguration;
import org.springframework.boot.autoconfigure.liquibase.LiquibaseAutoConfiguration;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.boot.test.context.TestConfiguration;
import org.springframework.boot.test.mock.mockito.MockBean;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Primary;
import org.springframework.security.oauth2.client.OAuth2AuthorizedClientService;
import org.springframework.security.oauth2.client.registration.ClientRegistrationRepository;
import org.springframework.security.oauth2.jwt.JwtDecoder;
import org.springframework.test.context.ActiveProfiles;
import org.springframework.test.context.event.annotation.BeforeTestClass;

@SpringBootTest
@EnableAutoConfiguration(exclude = {LiquibaseAutoConfiguration.class, RabbitAutoConfiguration.class})
@ActiveProfiles({"h2-unit-test", "logging-dev"})
public abstract class BaseTest {
    @MockBean
    AuditorAwareConfig auditorAwareConfig;
    @MockBean
    JwtDecoder jwtDecoder;
    @MockBean
    RabbitConsumerConfig rabbitConsumerConfig;
    @MockBean
    OAuth2AuthorizedClientService oAuth2AuthorizedClientService;
    @MockBean
	BaseAuditLogger auditLogger;
    @MockBean
    ConnectionFactory connectionFactory;
    
	@MockBean
	protected PanFeignClient panFeignClient;
	
	protected UserContext emptyUserContext = UserContextHolder.emptyServletContext();
	
    @TestConfiguration
    public static class BaseTestConfig {

        @Bean
        @Primary
        protected ClientRegistrationRepository clientRegistrationRepository(){
        	ClientRegistrationRepository clientRegistrationRepository = mock(ClientRegistrationRepository.class);
        	when(clientRegistrationRepository.findByRegistrationId(anyString())).thenReturn(Factory.clientRegistration());
        	
        	return clientRegistrationRepository;
        }

    }
}
