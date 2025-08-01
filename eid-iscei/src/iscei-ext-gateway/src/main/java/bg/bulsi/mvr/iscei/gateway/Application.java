package bg.bulsi.mvr.iscei.gateway;

import java.security.Security;

import org.bouncycastle.jce.provider.BouncyCastleProvider;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.boot.autoconfigure.domain.EntityScan;
import org.springframework.boot.autoconfigure.web.servlet.error.ErrorMvcAutoConfiguration;
import org.springframework.cloud.openfeign.EnableFeignClients;
import org.springframework.data.redis.repository.configuration.EnableRedisRepositories;
import bg.bulsi.mvr.iscei.gateway.config.SystemProperties;

import org.springframework.data.jpa.repository.config.EnableJpaRepositories;
import org.springframework.data.redis.core.RedisKeyValueAdapter;

@SpringBootApplication(exclude = {ErrorMvcAutoConfiguration.class},
scanBasePackages = {
        "bg.bulsi.mvr.iscei.gateway",
        "bg.bulsi.mvr.iscei.gateway.controller",
//        "bg.bulsi.mvr.common.rabbitmq.producer",
//        "bg.bulsi.mvr.common.config",
        "bg.bulsi.mvr.common.exception",
		"bg.bulsi.mvr.audit_logger",
		"bg.bulsi.mvr.pan_client",
		"bg.bulsi.mvr.iscei.pan",
		"bg.bulsi.mvr.iscei.model.service",
		"bg.bulsi.mvr.iscei.model.config"
})
//@ComponentScan(excludeFilters =  @Filter(type = FilterType.ASSIGNABLE_TYPE, value = ServletUserContextInterceptor.class))
@EntityScan({"bg.bulsi.mvr.iscei.model"})
@EnableJpaRepositories({"bg.bulsi.mvr.iscei.model.repository.jpa"})
@EnableRedisRepositories(basePackages = {"bg.bulsi.mvr.iscei.model.repository.keypair"},  enableKeyspaceEvents = RedisKeyValueAdapter.EnableKeyspaceEvents.ON_STARTUP)
@EnableFeignClients(basePackages = {"bg.bulsi.mvr.iscei.gateway.client", "bg.bulsi.mvr.pan_client"})
public class Application {

	public static void main(String[] args) {
		SystemProperties.setSystemProperties();
		
		Security.addProvider(new BouncyCastleProvider());
		
		SpringApplication.run(Application.class, args);
	}

}
