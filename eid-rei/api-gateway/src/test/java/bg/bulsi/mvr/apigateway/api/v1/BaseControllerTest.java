package bg.bulsi.mvr.apigateway.api.v1;

import bg.bulsi.mvr.audit_logger.BaseAuditLogger;
import bg.bulsi.mvr.common.config.ReactiveUserContextFilter;
import org.springframework.boot.autoconfigure.security.oauth2.resource.reactive.ReactiveOAuth2ResourceServerAutoConfiguration;
import org.springframework.boot.autoconfigure.security.reactive.ReactiveSecurityAutoConfiguration;
import org.springframework.boot.test.autoconfigure.web.reactive.WebFluxTest;
import org.springframework.boot.test.mock.mockito.MockBean;
import org.springframework.context.annotation.ComponentScan;
import org.springframework.context.annotation.FilterType;
import org.springframework.test.context.ActiveProfiles;

import bg.bulsi.mvr.common.rabbitmq.producer.RabbitControllerAdvice;
import bg.bulsi.mvr.common.rabbitmq.producer.RabbitProducerConfig;

@ActiveProfiles("logging-dev")
@WebFluxTest(
        excludeAutoConfiguration = {ReactiveSecurityAutoConfiguration.class, ReactiveOAuth2ResourceServerAutoConfiguration.class},
        //controllers = {EIdentityApi.class}, 
        excludeFilters = {
                @ComponentScan.Filter(type = FilterType.ASSIGNABLE_TYPE, classes = {RabbitControllerAdvice.class, RabbitProducerConfig.class, ReactiveUserContextFilter.class})})
public class BaseControllerTest {

	@MockBean
	protected BaseAuditLogger auditLogger;
}
