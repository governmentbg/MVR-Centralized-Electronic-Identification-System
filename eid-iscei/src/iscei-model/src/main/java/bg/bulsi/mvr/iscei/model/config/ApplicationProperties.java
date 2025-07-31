package bg.bulsi.mvr.iscei.model.config;

import java.util.Map;

import org.springframework.boot.context.properties.ConfigurationProperties;
import org.springframework.stereotype.Component;

import lombok.Data;

@Data
@Component
@ConfigurationProperties(prefix = "mvr")
public class ApplicationProperties {

	private Map<String, MvrOAuthClient> oauthClients;
}
