package bg.bulsi.mvr.iscei.gateway.config;


import java.util.Arrays;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.context.annotation.Lazy;
import org.springframework.core.annotation.Order;
import org.springframework.http.HttpMethod;
import org.springframework.security.config.annotation.method.configuration.EnableMethodSecurity;
import org.springframework.security.config.annotation.web.builders.HttpSecurity;
import org.springframework.security.config.annotation.web.configuration.EnableWebSecurity;
import org.springframework.security.config.annotation.web.configurers.AbstractHttpConfigurer;
import org.springframework.security.oauth2.client.AuthorizedClientServiceOAuth2AuthorizedClientManager;
import org.springframework.security.oauth2.client.OAuth2AuthorizedClientManager;
import org.springframework.security.oauth2.client.OAuth2AuthorizedClientService;
import org.springframework.security.oauth2.client.registration.ClientRegistrationRepository;
import org.springframework.security.web.SecurityFilterChain;
import org.springframework.web.cors.CorsConfiguration;
import org.springframework.web.cors.CorsConfigurationSource;
import org.springframework.web.cors.UrlBasedCorsConfigurationSource;

import bg.bulsi.mvr.common.config.GrantedAuthoritiesExtractor;

@Configuration
@EnableWebSecurity
//@EnableMethodSecurity
public class WebSecurityConfig {
	
	@Lazy
	@Autowired
	private GrantedAuthoritiesExtractor grantedAuthoritiesExtractor;

	//Cannot scan the package "bg.bulsi.mvr.common.config"
	//So we need to create the {@link GrantedAuthoritiesExtractor} like this
	@Bean
	public GrantedAuthoritiesExtractor grantedAuthoritiesExtractor() {
		return new GrantedAuthoritiesExtractor();
	};
	
	@Bean
	@Order(1)
	public SecurityFilterChain publicSecurityFilterChain(HttpSecurity http) throws Exception {
	    
		//TODO: add /api/v1
		http
        .securityMatcher( 
		        		"/api/v1/approval-request/rely-party",
        		
        		        "/swagger-ui/**",
                        "/docs/**",
                        "/v3/api-docs/**",
                        "/actuator/**")
		.authorizeHttpRequests(requests -> requests
			.anyRequest().permitAll()
			)
		.csrf(AbstractHttpConfigurer::disable)
		.cors(cors -> cors.configurationSource(corsConfigurationSource()))
		;
        
        return http.build();
	}
	
	@Bean
	@Order(2)
	public SecurityFilterChain securityFilterChain(HttpSecurity http) throws Exception {
		http
		.authorizeHttpRequests(requests -> requests
			.anyRequest().authenticated()
			)
		.csrf(AbstractHttpConfigurer::disable)
       .oauth2ResourceServer(oauth2 -> oauth2
    		   //.opaqueToken(Customizer.withDefaults())
    		   .jwt(jwt -> jwt.jwtAuthenticationConverter(grantedAuthoritiesExtractor))
    		   )
       

		;
		   
		return http.build();
	}
	
//    private Converter<Jwt, AbstractAuthenticationToken> grantedAuthoritiesExtractor() {
//        return new JwtGrantedAuthoritiesConverter(new GrantedAuthoritiesExtractor());
//    }
//
//    
	  // @Bean
	   CorsConfigurationSource corsConfigurationSource() {
	        CorsConfiguration configuration = new CorsConfiguration();
	        
	        configuration.setAllowedOriginPatterns(Arrays.asList("*"));

	        // Set allowed origins (adjust as needed)
	        //configuration.setAllowedOrigins(Arrays.asList(isceiUiBaseUrl));
	        // Set allowed HTTP methods
	        configuration.setAllowedMethods(Arrays.asList(
	                HttpMethod.GET.name(), 
	                HttpMethod.POST.name(), 
	                HttpMethod.PUT.name(), 
	                HttpMethod.DELETE.name(), 
	                HttpMethod.OPTIONS.name()));
	        // Set allowed headers
	        configuration.setAllowedHeaders(Arrays.asList("*"));
	        // Allow credentials (cookies, authorization headers, etc.)
	        configuration.setAllowCredentials(true);

	        UrlBasedCorsConfigurationSource source = new UrlBasedCorsConfigurationSource();
	        // Apply CORS configuration for all endpoints
	        source.registerCorsConfiguration("/**", configuration);
	        return source;
	    }
}
