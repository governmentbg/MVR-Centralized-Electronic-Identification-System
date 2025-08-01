package bg.bulsi.mvr.iscei.gateway.service;

import java.util.Map;

import org.springframework.stereotype.Service;
import org.springframework.util.MultiValueMap;

import bg.bulsi.mvr.iscei.contract.dto.ProxiedClient;
import bg.bulsi.mvr.iscei.gateway.config.ProxiedClientProperties;

@Service
public class ProxiedClientService {

	private Map<String, ProxiedClient> proxiedClients;

	public ProxiedClientService(ProxiedClientProperties proxiedClientProperties) {
		proxiedClients = proxiedClientProperties.getProxiedClients();
	}

	/**
	 * <pre>
	 * Check if the client is one of the Proxied Clients in the config,
	 * if it is add the client secret to the request. This is done because
	 * some frontend clients cannot store secrets, so ISCEI do it on their part.
	 * </pre>
	 * @param clientId
	 * @param request
	 * @return 
	 */
	public ProxiedClient evaluateClient(String clientId, MultiValueMap<String, String> request) {
		ProxiedClient proxiedClient = proxiedClients.get(clientId);
		if (proxiedClient != null) {
			request.add("client_secret", proxiedClient.getClientSecret());
		}
		
		return proxiedClient;
	}
}
