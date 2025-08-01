package bg.bulsi.mvr.mpozei.backend;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;

import bg.bulsi.mvr.mpozei.backend.config.SystemProperties;
import org.springframework.boot.autoconfigure.data.elasticsearch.ElasticsearchDataAutoConfiguration;
import org.springframework.boot.autoconfigure.elasticsearch.ElasticsearchRestClientAutoConfiguration;


@SpringBootApplication(exclude = {ElasticsearchDataAutoConfiguration.class, ElasticsearchRestClientAutoConfiguration.class})
public class BackendApplication {
    public static void main(String[] args) {
        SystemProperties.setSystemProperties();
        SpringApplication.run(BackendApplication.class, args);
    }
}
