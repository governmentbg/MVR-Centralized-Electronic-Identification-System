package bg.bulsi.mvr.iscei.gateway;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.boot.autoconfigure.domain.EntityScan;
import org.springframework.boot.autoconfigure.web.servlet.error.ErrorMvcAutoConfiguration;
import org.springframework.data.jpa.repository.config.EnableJpaRepositories;
import org.springframework.data.redis.core.RedisKeyValueAdapter;
import org.springframework.data.redis.repository.configuration.EnableRedisRepositories;

import bg.bulsi.mvr.iscei.gateway.config.SystemProperties;

@SpringBootApplication(exclude = {ErrorMvcAutoConfiguration.class},
scanBasePackages = {
        "bg.bulsi.mvr.iscei.gateway",
        "bg.bulsi.mvr.common.exception",
		"bg.bulsi.mvr.audit_logger",
		"bg.bulsi.mvr.iscei.model.service",
		"bg.bulsi.mvr.iscei.model.config"
})
//@ComponentScan(excludeFilters =  @Filter(type = FilterType.ASSIGNABLE_TYPE, value = ServletUserContextInterceptor.class))
@EnableJpaRepositories({"bg.bulsi.mvr.iscei.model.repository.jpa"})
@EnableRedisRepositories(basePackages = {"bg.bulsi.mvr.iscei.model.repository.keypair"},  enableKeyspaceEvents = RedisKeyValueAdapter.EnableKeyspaceEvents.ON_STARTUP)
@EntityScan({"bg.bulsi.mvr.iscei.model"})
public class Application {

	public static void main(String[] args) {
		SystemProperties.setSystemProperties();
		
		SpringApplication.run(Application.class, args);
	}
}
