package bg.bulsi.mvr.common.config;

import bg.bulsi.mvr.common.config.security.UserContextHolder;
import org.springframework.stereotype.Service;
import reactor.core.publisher.Mono;

import java.util.Arrays;

@Service("authzService")
public class AuthorizationService {
    public boolean hasAnyRole(String... role) {
        return Arrays.stream(role).anyMatch(e -> UserContextHolder.getFromServletContext().getAuthorities().contains(e));
    }

    public Mono<Boolean> hasAnyRoleReactive(String... role) {
        return UserContextHolder.getFromReactiveContext().map(userContext ->
                Arrays.stream(role).anyMatch(e -> userContext.getAuthorities().contains(e)));
    }
}
