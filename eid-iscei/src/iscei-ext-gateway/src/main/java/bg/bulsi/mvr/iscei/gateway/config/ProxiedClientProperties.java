package bg.bulsi.mvr.iscei.gateway.config;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.springframework.beans.factory.InitializingBean;
import org.springframework.boot.context.properties.ConfigurationProperties;
import org.springframework.boot.context.properties.EnableConfigurationProperties;
import org.springframework.context.annotation.Configuration;
import org.springframework.stereotype.Component;

import bg.bulsi.mvr.iscei.contract.dto.ProxiedClient;
import lombok.Data;

@Data
@Configuration
@EnableConfigurationProperties 
@ConfigurationProperties(prefix = "mvr")
public class ProxiedClientProperties {

	private final Map<String, ProxiedClient> proxiedClients = new HashMap<>();
}
