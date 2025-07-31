package bg.bulsi.mvr.common.config;

import bg.bulsi.mvr.common.config.security.UserContext;
import bg.bulsi.mvr.common.config.security.UserContextHolder;
import lombok.extern.slf4j.Slf4j;
import org.springframework.web.server.ServerWebExchange;
import org.springframework.web.server.WebFilter;
import org.springframework.web.server.WebFilterChain;
import reactor.core.publisher.Mono;

/*
    If a header with name user-context is found within the request,
    the filter builds the UserContext object from the header.
    If there is no header found in the request it generates
    new UserContext with unique globalCorrelationId

    For Reactive applications
 */
@Slf4j
public class ReactivePublicUserContextFilter implements WebFilter {

	
    @Override
    public Mono<Void> filter(ServerWebExchange exchange, WebFilterChain chain) {
        Mono<UserContext> userContext = UserContextHolder.emptyContext();
        return UserContextHolder.setToReactiveContext(chain.filter(exchange), userContext);
    }
}
