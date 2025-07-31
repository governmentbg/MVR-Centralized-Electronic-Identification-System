package bg.bulsi.mvr.raeicei.config;

import org.springframework.context.annotation.Configuration;
import org.springframework.data.jpa.repository.config.EnableJpaAuditing;

@Configuration
@EnableJpaAuditing
public class AuditConfig {
    //using default JPA Auditing configuration
    // If needed, you can customize the JPA auditing behavior by overriding the auditing settings.
    // For example, you can configure the auditor provider or set the DateTimeProvider.
}
