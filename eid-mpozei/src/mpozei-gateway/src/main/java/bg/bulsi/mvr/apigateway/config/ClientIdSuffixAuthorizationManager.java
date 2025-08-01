//package bg.bulsi.mvr.apigateway.config;
//
//import org.springframework.security.authorization.AuthorizationDecision;
//import org.springframework.security.authorization.ReactiveAuthorizationManager;
//import org.springframework.security.web.server.authorization.AuthorizationContext;
//
//import org.springframework.security.core.Authentication;
//import org.springframework.security.core.GrantedAuthority;
//import org.springframework.security.oauth2.server.resource.authentication.JwtAuthenticationToken;
//
//import reactor.core.publisher.Mono;
//
//public class ClientIdSuffixAuthorizationManager implements ReactiveAuthorizationManager<AuthorizationContext> {
//	private static final String REQUIRED_SUFFIX = "_m2m";
//	
//    private final String requiredRole;
//    
//    public ClientIdSuffixAuthorizationManager(String requiredRole) {
//    	this.requiredRole = requiredRole;
//    }
//
//    @Override
//    public Mono<AuthorizationDecision> check(Mono<Authentication> authentication, AuthorizationContext context) {
//    	return authentication
//            .filter(JwtAuthenticationToken.class::isInstance)
//            .map(JwtAuthenticationToken.class::cast)
//            .map(JwtAuthenticationToken::getToken)
//            .flatMap(jwt -> {
//            	String clientId = jwt.getClaimAsString("azp") ;// Extract azp/client_id from JWT
//                if (clientId != null && clientId.endsWith(REQUIRED_SUFFIX)) {
//                    return Mono.just(new AuthorizationDecision(true));  // Always allow trusted client
//                }
//                
//                return authentication
//                    .flatMap(auth -> Mono.just(auth.getAuthorities().stream()
//                        .map(GrantedAuthority::getAuthority)
//                        .anyMatch(role -> role.equals("ROLE_" + requiredRole)
//                        )))
//                    .map(AuthorizationDecision::new);
//            })
//            .defaultIfEmpty(new AuthorizationDecision(false)); // Deny access if no client_id
//    }
//}
