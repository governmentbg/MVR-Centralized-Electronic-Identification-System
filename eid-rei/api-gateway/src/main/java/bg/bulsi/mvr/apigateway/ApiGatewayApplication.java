package bg.bulsi.mvr.apigateway;

import bg.bulsi.mvr.apigateway.config.SystemProperties;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.boot.autoconfigure.jdbc.DataSourceAutoConfiguration;
import org.springframework.boot.autoconfigure.orm.jpa.HibernateJpaAutoConfiguration;

@SpringBootApplication(exclude = {DataSourceAutoConfiguration.class, HibernateJpaAutoConfiguration.class},
	scanBasePackages = {"bg.bulsi.mvr.apigateway.api.v1",
			"bg.bulsi.mvr.common.exception", 
			"bg.bulsi.mvr.common.rabbitmq.producer",
			"bg.bulsi.mvr.common.config",
            "bg.bulsi.mvr.common.exception",
			"bg.bulsi.mvr.audit_logger",
			"bg.bulsi.mvr.apigateway.service",
			"bg.bulsi.mvr.apigateway.config"
			})
public class ApiGatewayApplication {

    public static void main(String[] args) {
		SystemProperties.setSystemProperties();
        SpringApplication.run(ApiGatewayApplication.class, args);
    }

}
