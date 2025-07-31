package bg.bulsi.mvr.apigateway.service;

import org.springframework.amqp.core.DirectExchange;
import org.springframework.amqp.rabbit.AsyncRabbitTemplate;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;
import org.springframework.web.server.ServerWebExchange;

import bg.bulsi.mvr.apigateway.api.v1.ConfigurationsApiDelegate;
import bg.bulsi.mvr.common.openapi.CustomExcludeGenerated;
import bg.bulsi.mvr.reicontract.dto.ConfigurationResultDTO;
import bg.bulsi.mvr.reicontract.dto.UpdateConfigurationRequestDTO;
import reactor.core.publisher.Mono;

@Service
@CustomExcludeGenerated
public class ConfigurationsApiDelegateService implements ConfigurationsApiDelegate{

	@Autowired
	private AsyncRabbitTemplate asyncRabbitTemplate;

	@Autowired
	private DirectExchange rpcExchange;
	
	@Override
	public Mono<ConfigurationResultDTO> getConfigByKey(String key, ServerWebExchange exchange) {
		// TODO Auto-generated method stub
		return ConfigurationsApiDelegate.super.getConfigByKey(key, exchange);
	}

}
