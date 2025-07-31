package bg.bulsi.mvr.apigateway.service;

import bg.bulsi.mvr.audit_logger.BaseAuditLogger;
import bg.bulsi.mvr.audit_logger.EncryptionHelper;
import bg.bulsi.mvr.common.rabbitmq.producer.EventSender;

import org.mockito.Mock;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

@Configuration
public class TestServiceConfig {
	
	@Bean
	public EIdentityApiDelegateService eIdentityApiDelegateService() {
		return new EIdentityApiDelegateService();
	}
	
	@Bean
	public EventSender eventSender() {
		return new EventSender();
	}
	
	@Bean
	public BaseAuditLogger auditLogger() {
		return new BaseAuditLogger();
	}
	
	@Bean
	public EncryptionHelper encryptionHelper() {
		return new EncryptionHelper();
	}
}
