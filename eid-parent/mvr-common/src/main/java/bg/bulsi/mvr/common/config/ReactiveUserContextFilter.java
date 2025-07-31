package bg.bulsi.mvr.common.config;

import bg.bulsi.mvr.common.config.security.UserContext;
import bg.bulsi.mvr.common.config.security.UserContextHolder;
import lombok.extern.slf4j.Slf4j;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.web.server.ServerWebExchange;
import org.springframework.web.server.WebFilter;
import org.springframework.web.server.WebFilterChain;

import reactor.core.publisher.Mono;

import java.nio.charset.Charset;
import java.nio.charset.StandardCharsets;
import java.util.Base64;
import java.util.Collections;
import java.util.List;

/*
    If a header with name user-context is found within the request,
    the filter builds the UserContext object from the header.
    If there is no header found in the request it generates
    new UserContext with unique globalCorrelationId

    For Reactive applications
 */
@Slf4j
public class ReactiveUserContextFilter implements WebFilter {

	/*
	 * In some the UserContext should always we created, i.e. we do not use the
	 * UserContext from request header, we create the UserContext from the access token
	 */
	@Value("${mvr.external-service:false}")
	private boolean externalService;
	
    @Override
    public Mono<Void> filter(ServerWebExchange exchange, WebFilterChain chain) {
    	log.info("reuseUserContext => " +externalService);

    	var response = exchange.getResponse();
    	response.beforeCommit(() -> {
    		List<String> userContextJson = response.getHeaders()
		            .getOrDefault(UserContext.USER_CONTEXT_KEY, Collections.emptyList());
    		
		    if (!userContextJson.isEmpty()) {
		    	response.getHeaders().remove(UserContext.USER_CONTEXT_KEY);
		    	
		    	if(!externalService) {
		    		response.getHeaders().add(UserContext.USER_CONTEXT_KEY, Base64.getEncoder().encodeToString(userContextJson.get(0).getBytes()));
		    	}
		    } 
    		
    		return Mono.empty();
    	});
    	
    	
        Mono<UserContext> userContext;
        List<String> userContextJson = exchange
                .getRequest()
                .getHeaders()
                .getOrDefault(UserContext.USER_CONTEXT_KEY, Collections.emptyList());
        
        if (!userContextJson.isEmpty()) {
        	if(!externalService) {
        		userContext = Mono.just(UserContextHolder.fromJson(
        				new String(Base64.getDecoder().decode(userContextJson.get(0).getBytes())))
        				);
        	} else {
        		userContext = exchange.getPrincipal().map(UserContext::new);
        	}
        } else {
            userContext = exchange.getPrincipal().map(UserContext::new);
        }
        
        return UserContextHolder.setToReactiveContext(chain.filter(exchange), userContext);
    }
}
