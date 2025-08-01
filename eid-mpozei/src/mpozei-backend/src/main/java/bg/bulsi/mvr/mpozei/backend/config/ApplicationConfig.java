package bg.bulsi.mvr.mpozei.backend.config;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.boot.autoconfigure.domain.EntityScan;
import org.springframework.cloud.openfeign.EnableFeignClients;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.ComponentScan;
import org.springframework.context.annotation.Configuration;
import org.springframework.data.elasticsearch.repository.config.EnableElasticsearchRepositories;
import org.springframework.data.jpa.repository.config.EnableJpaRepositories;
import org.springframework.retry.annotation.EnableRetry;
import org.springframework.scheduling.annotation.EnableAsync;
import org.springframework.scheduling.annotation.EnableScheduling;
import org.springframework.transaction.annotation.EnableTransactionManagement;

import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

//TODO: rework this to remove component scanning in place of AutoConfiguration
@Configuration
@EnableTransactionManagement
@ComponentScan(basePackages = {
        "bg.bulsi.mvr.mpozei.backend",
        "bg.bulsi.mvr.common.rabbitmq.consumer",
        "bg.bulsi.mvr.common.config",
        "bg.bulsi.mvr.mpozei.model",
        "bg.bulsi.mvr.mpozei.config",
        "bg.bulsi.mvr.common.pipeline",
        "bg.bulsi.mvr.pdf_generator",
        "bg.bulsi.mvr.audit_logger",
        "bg.bulsi.mvr.pan_client",
        "bg.bulsi.mvr.common.crypto",
        "bg.bulsi.mvr.common.service"
})
@EnableJpaRepositories({"bg.bulsi.mvr.mpozei.model.repository"})
@EnableElasticsearchRepositories({"bg.bulsi.mvr.mpozei.model.repository"})
@EntityScan({"bg.bulsi.mvr.mpozei.model"})
@EnableFeignClients(basePackages = {"bg.bulsi.mvr.mpozei.backend.client", "bg.bulsi.mvr.pan_client"})
@EnableRetry
@EnableScheduling
@EnableAsync
public class ApplicationConfig {
    @Value("${executor.payments-thread-pool}")
    private Integer paymentsThreadPool;

    @Bean("paymentsExecutorService")
    public ExecutorService paymentsExecutorService() {
        return Executors.newFixedThreadPool(paymentsThreadPool);
    }
}
