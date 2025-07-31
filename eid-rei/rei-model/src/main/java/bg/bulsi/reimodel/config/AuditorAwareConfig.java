package bg.bulsi.reimodel.config;

import bg.bulsi.mvr.common.config.security.UserContextHolder;
import org.springframework.data.domain.AuditorAware;
import org.springframework.stereotype.Component;

import java.util.Optional;

@Component
public class AuditorAwareConfig implements AuditorAware<String> {
    @Override
    public Optional<String> getCurrentAuditor() {
        return Optional.of(UserContextHolder.getFromServletContext().getUsername());
    }
}
