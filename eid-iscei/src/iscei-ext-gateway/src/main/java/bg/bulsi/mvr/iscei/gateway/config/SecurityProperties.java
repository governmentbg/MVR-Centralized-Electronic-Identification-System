package bg.bulsi.mvr.iscei.gateway.config;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.autoconfigure.security.oauth2.client.OAuth2ClientProperties;
import org.springframework.boot.autoconfigure.security.oauth2.client.OAuth2ClientProperties.Provider;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

@Configuration
public class SecurityProperties {
	
	public static final String KEYCLOAK_EXTERNAL_PROVIDER = "keycloak-external";
	@Autowired
	private OAuth2ClientProperties oAuth2ClientProperties;
	
	@Bean
	public Provider keycloakExternal() {
		return this.oAuth2ClientProperties.getProvider().get(KEYCLOAK_EXTERNAL_PROVIDER);
	}
	
}
