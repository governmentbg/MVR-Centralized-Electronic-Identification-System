package bg.bulsi.mvr.extgateway.config;


import bg.bulsi.mvr.common.config.GrantedAuthoritiesExtractor;
import bg.bulsi.mvr.common.config.ReactivePublicUserContextFilter;
import bg.bulsi.mvr.common.config.ReactiveUserContextFilter;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.core.Ordered;
import org.springframework.core.annotation.Order;
import org.springframework.core.convert.converter.Converter;
import org.springframework.security.authentication.AbstractAuthenticationToken;
import org.springframework.security.config.annotation.web.reactive.EnableWebFluxSecurity;
import org.springframework.security.config.web.server.SecurityWebFiltersOrder;
import org.springframework.security.config.web.server.ServerHttpSecurity;
import org.springframework.security.oauth2.jwt.Jwt;
import org.springframework.security.oauth2.server.resource.authentication.ReactiveJwtAuthenticationConverterAdapter;
import org.springframework.security.web.server.SecurityWebFilterChain;
import org.springframework.security.web.server.util.matcher.OrServerWebExchangeMatcher;
import org.springframework.security.web.server.util.matcher.PathPatternParserServerWebExchangeMatcher;
import reactor.core.publisher.Mono;

@Configuration
@EnableWebFluxSecurity
public class WebSecurityConfig {
	
	@Autowired
	private GrantedAuthoritiesExtractor grantedAuthoritiesExtractor;
	
    @Bean
    public SecurityWebFilterChain securedFilterChain(ServerHttpSecurity http) {
        return http
                .securityMatcher(new PathPatternParserServerWebExchangeMatcher("/raeicei/external/api/v1/**"))
                .authorizeExchange(exchanges -> exchanges
                        .anyExchange()
                        .authenticated())
                .oauth2ResourceServer(oauth2 -> oauth2
                        .jwt(jwt -> jwt.jwtAuthenticationConverter(grantedAuthoritiesExtractor())))
                .addFilterAfter(new ReactiveUserContextFilter(), SecurityWebFiltersOrder.AUTHORIZATION)
                .cors(ServerHttpSecurity.CorsSpec::disable)
                .csrf(ServerHttpSecurity.CsrfSpec::disable)
                .build();
    }

    @Bean
    @Order(Ordered.HIGHEST_PRECEDENCE)
    public SecurityWebFilterChain publicFilterChain(ServerHttpSecurity http) {
        return http
                .securityMatcher(new OrServerWebExchangeMatcher(
                        new PathPatternParserServerWebExchangeMatcher("/webjars/swagger-ui/**"),
                        new PathPatternParserServerWebExchangeMatcher("/docs/**"),
                        new PathPatternParserServerWebExchangeMatcher("/v3/api-docs/**"),
                        new PathPatternParserServerWebExchangeMatcher("/raeicei/external/api/v1/eidadministrator/**"),
                        new PathPatternParserServerWebExchangeMatcher("/raeicei/external/api/v1/eidcenter/**"),
                        new PathPatternParserServerWebExchangeMatcher("/raeicei/external/api/v1/eidmanagerfrontoffice/**"),
                		new PathPatternParserServerWebExchangeMatcher("/raeicei/external/api/v1/device/**"),
						new PathPatternParserServerWebExchangeMatcher("/raeicei/external/api/v1/providedservice/**")))
                
                .authorizeExchange(exchanges -> exchanges.anyExchange().permitAll())
                .addFilterAfter(new ReactivePublicUserContextFilter(),SecurityWebFiltersOrder.ANONYMOUS_AUTHENTICATION)
                .cors(ServerHttpSecurity.CorsSpec::disable)
                .csrf(ServerHttpSecurity.CsrfSpec::disable)
                .build();
    }

    private Converter<Jwt, Mono<AbstractAuthenticationToken>> grantedAuthoritiesExtractor() {
        return new ReactiveJwtAuthenticationConverterAdapter(this.grantedAuthoritiesExtractor);
    }
}
