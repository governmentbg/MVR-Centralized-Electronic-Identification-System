package bg.bulsi.mvr.apigateway.service;

import bg.bulsi.mvr.mpozei.contract.dto.ConfigurationResult;
import bg.bulsi.mvr.mpozei.gateway.api.v1.ConfigurationsApiDelegate;
import org.springframework.amqp.core.DirectExchange;
import org.springframework.amqp.rabbit.AsyncRabbitTemplate;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.stereotype.Service;
import org.springframework.web.server.ServerWebExchange;
import reactor.core.publisher.Mono;

@Service
public class ConfigurationsApiDelegateService implements ConfigurationsApiDelegate{

	@Autowired
	private AsyncRabbitTemplate asyncRabbitTemplate;

	@Autowired
	private DirectExchange rpcExchange;
	
	@Override
//    @PreAuthorize("@authzService.hasAnyRoleReactive('ROLE_AEI_admin')")
	public Mono<ConfigurationResult> getConfigByKey(String key, ServerWebExchange exchange) {
		// TODO Auto-generated method stub
		return ConfigurationsApiDelegate.super.getConfigByKey(key, exchange);
	}

}
