package bg.bulsi.mvr.common.config;

import org.springframework.core.convert.converter.Converter;
import org.springframework.security.authentication.AbstractAuthenticationToken;
import org.springframework.security.core.authority.SimpleGrantedAuthority;
import org.springframework.security.oauth2.jwt.Jwt;
import org.springframework.security.oauth2.server.resource.authentication.JwtAuthenticationToken;
import org.springframework.stereotype.Component;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collections;
import java.util.List;

@Component
public class GrantedAuthoritiesExtractor implements Converter<Jwt, AbstractAuthenticationToken> {
	
	@Override
    public AbstractAuthenticationToken convert(Jwt jwt) {
        List<SimpleGrantedAuthority> roles = ((List<String>)jwt.getClaimAsMap("realm_access")
                .getOrDefault("roles", Collections.emptyList()))
                .stream()
                .map(Object::toString)
                .map(role -> new SimpleGrantedAuthority("ROLE_" + role))
                .toList();
		
        List<SimpleGrantedAuthority> scopes = Arrays.asList(jwt.getClaimAsString("scope")
        		.split("\\s+"))
                .stream()
                .map(scope -> new SimpleGrantedAuthority("SCOPE_" + scope))
                .toList();

        List<SimpleGrantedAuthority> authorities = new ArrayList<>();
        authorities.addAll(roles);
        authorities.addAll(scopes);
                
        return new JwtAuthenticationToken(jwt, authorities, jwt.getClaim("preferred_username"));
    }
}
