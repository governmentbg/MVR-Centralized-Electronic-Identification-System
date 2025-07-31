package bg.bulsi.mvr.raeicei.backend.config;

import org.springframework.boot.autoconfigure.domain.EntityScan;
import org.springframework.cache.annotation.EnableCaching;
import org.springframework.cloud.openfeign.EnableFeignClients;
import org.springframework.context.annotation.ComponentScan;
import org.springframework.context.annotation.Configuration;
import org.springframework.data.jpa.repository.config.EnableJpaRepositories;
import org.springframework.transaction.annotation.EnableTransactionManagement;

//TODO: rework this to remove component scanning in place of AutoConfiguration
@Configuration
@EnableTransactionManagement
@ComponentScan(basePackages = {
        "bg.bulsi.mvr.common.rabbitmq.consumer",
        "bg.bulsi.mvr.common.config",
        "bg.bulsi.mvr.raeicei.backend",
        "bg.bulsi.mvr.raeicei.model",
        "bg.bulsi.mvr.raeicei.config",
		"bg.bulsi.mvr.pan_client",
        "bg.bulsi.mvr.common.pipeline",
        "bg.bulsi.mvr.audit_logger",
})
@EnableJpaRepositories({"bg.bulsi.mvr.raeicei.model.repository"})
@EntityScan({"bg.bulsi.mvr.raeicei.model"})
@EnableCaching
//@EnableFeignClients(basePackages = {"bg.bulsi.mvr.raeicei.backend.client"})
@EnableFeignClients(basePackages = {"bg.bulsi.mvr.pan_client"})
public class ApplicationConfig {

}
