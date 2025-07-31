package bg.bulsi.mvr.extgateway;

import bg.bulsi.mvr.extgateway.config.SystemProperties;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.boot.autoconfigure.jdbc.DataSourceAutoConfiguration;
import org.springframework.boot.autoconfigure.orm.jpa.HibernateJpaAutoConfiguration;

@SpringBootApplication(exclude = {DataSourceAutoConfiguration.class, HibernateJpaAutoConfiguration.class},
	scanBasePackages = {
            "bg.bulsi.mvr.extgateway",
            "bg.bulsi.mvr.common.config",
            "bg.bulsi.mvr.common.rabbitmq.producer",
            "bg.bulsi.mvr.common.exception",
            "bg.bulsi.mvr.raeicei.extgateway.api",
			"bg.bulsi.mvr.audit_logger"
    })
public class ExtGatewayApplication {

    public static void main(String[] args) {
		SystemProperties.setSystemProperties();
        SpringApplication.run(ExtGatewayApplication.class, args);
    }

}
