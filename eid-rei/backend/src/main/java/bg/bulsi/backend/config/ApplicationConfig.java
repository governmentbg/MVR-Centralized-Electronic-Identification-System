package bg.bulsi.backend.config;

import org.springframework.boot.autoconfigure.domain.EntityScan;
import org.springframework.context.annotation.ComponentScan;
import org.springframework.context.annotation.Configuration;
import org.springframework.data.jpa.repository.config.EnableJpaRepositories;
import org.springframework.transaction.annotation.EnableTransactionManagement;

@Configuration
@EnableTransactionManagement
@ComponentScan(basePackages = {
        "bg.bulsi.reimodel",
        "bg.bulsi.backend",
        "bg.bulsi.mvr.common.rabbitmq.consumer",
        "bg.bulsi.mvr.common.config",
        "bg.bulsi.mvr.common.util",
        "bg.bulsi.mvr.audit_logger"
})
@EnableJpaRepositories({"bg.bulsi.reimodel.repository"})
@EntityScan({"bg.bulsi.reimodel.model"})
public class ApplicationConfig {
}
