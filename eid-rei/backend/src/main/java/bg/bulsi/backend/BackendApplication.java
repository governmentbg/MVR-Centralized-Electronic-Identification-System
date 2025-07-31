package bg.bulsi.backend;

import bg.bulsi.backend.config.SystemProperties;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;


@SpringBootApplication
public class BackendApplication {
    public static void main(String[] args) {
        SystemProperties.setSystemProperties();
        SpringApplication.run(BackendApplication.class, args);
    }
}
