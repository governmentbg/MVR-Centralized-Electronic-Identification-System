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
import org.springframework.security.core.authority.SimpleGrantedAuthority;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.security.oauth2.jwt.Jwt;
import org.springframework.security.oauth2.server.resource.authentication.JwtAuthenticationToken;
import org.springframework.security.oauth2.server.resource.authentication.ReactiveJwtAuthenticationConverterAdapter;
import org.springframework.security.web.server.SecurityWebFilterChain;
import org.springframework.security.web.server.util.matcher.OrServerWebExchangeMatcher;
import org.springframework.security.web.server.util.matcher.PathPatternParserServerWebExchangeMatcher;
import reactor.core.publisher.Mono;

import java.util.Collection;
import java.util.Collections;
import java.util.List;
import java.util.Map;
import java.util.stream.Collectors;

@Configuration
@EnableWebFluxSecurity
public class WebSecurityConfig {
	
	@Autowired
	private GrantedAuthoritiesExtractor grantedAuthoritiesExtractor;
	
    @Bean
    public SecurityWebFilterChain securedFilterChain(ServerHttpSecurity http) {
        return http
                .securityMatcher(new PathPatternParserServerWebExchangeMatcher("/mpozei/external/api/v1/**"))
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
                        new PathPatternParserServerWebExchangeMatcher("/mpozei/external/api/v1/citizens/register"),
                        new PathPatternParserServerWebExchangeMatcher("/mpozei/external/api/v1/citizens/forgotten-password"),
                        new PathPatternParserServerWebExchangeMatcher("/mpozei/external/api/v1/citizens/reset-password"),
                        new PathPatternParserServerWebExchangeMatcher("/mpozei/external/api/v1/citizens/update-email/confirm"),
                        new PathPatternParserServerWebExchangeMatcher("/mpozei/external/api/v1/help-pages/**"),
                        new PathPatternParserServerWebExchangeMatcher("/mpozei/external/api/v1/certificates/public-find/**")))
                .authorizeExchange(exchanges -> exchanges.anyExchange().permitAll())
                .addFilterAfter(new ReactivePublicUserContextFilter(), SecurityWebFiltersOrder.ANONYMOUS_AUTHENTICATION)
                .cors(ServerHttpSecurity.CorsSpec::disable)
                .csrf(ServerHttpSecurity.CsrfSpec::disable)
                .build();
    }

    private Converter<Jwt, Mono<AbstractAuthenticationToken>> grantedAuthoritiesExtractor() {
        return new ReactiveJwtAuthenticationConverterAdapter(grantedAuthoritiesExtractor);
    }

//    @SuppressWarnings("unchecked")
//    static class GrantedAuthoritiesExtractor implements Converter<Jwt, AbstractAuthenticationToken> {
//        public AbstractAuthenticationToken convert(Jwt jwt) {
//            List<SimpleGrantedAuthority> authorities = ((Map<String, Collection<?>>) jwt.getClaims().getOrDefault("realm_access", Collections.emptyMap()))
//                    .getOrDefault("roles", Collections.emptyList())
//                    .stream()
//                    .map(Object::toString)
//                    .map(SimpleGrantedAuthority::new)
//                    .collect(Collectors.toList());
//            String username = (String) jwt.getClaims().getOrDefault("preferred_username", null);
//            return new JwtAuthenticationToken(jwt, authorities, username);
//        }
//    }

    @Bean
    public PasswordEncoder passwordEncoder() {
        return new BCryptPasswordEncoder();
    }
}
