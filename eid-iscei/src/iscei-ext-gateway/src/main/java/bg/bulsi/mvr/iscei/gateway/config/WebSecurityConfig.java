package bg.bulsi.mvr.iscei.gateway.config;


import java.util.Arrays;
import java.util.List;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.context.annotation.Lazy;
import org.springframework.core.annotation.Order;
import org.springframework.http.HttpMethod;
import org.springframework.security.config.Customizer;
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
import bg.bulsi.mvr.common.config.ServletUserContextInterceptor;

@Configuration
@EnableWebSecurity
@EnableMethodSecurity
public class WebSecurityConfig {
	
	@Value("${mvr.iscei_ui_base_url}")
	private String isceiUiBaseUrl;
	
//	@Autowired
//	private TestWebFilter webFilter;
	
	public static final String ISCEI_CLIENT_REGISTRATION_ID = "eid-iscei-m2m";
	
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
        				"/api/v1/code-flow/*",
        		        "/api/v1/auth/citizen",
        		        "/api/v1/auth/basic",
        		        "/api/v1/auth/generate-authentication-challenge",
        		        "/api/v1/auth/mobile/certificate-login",
        		        
        		        "/api/v1/approval-request/token",
        		        "/api/v1/approval-request/auth/citizen",
        		        "/api/v1/approval-request/outcome",
        		        
                        "/api/v1/auth/generate-otp",
                        "/api/v1/auth/verify-otp",
                        
        		        "/swagger-ui/**",
                        "/docs/**",
                        "/v3/api-docs/**",
                        "/actuator/**"
//        		new AntPathRequestMatcher("/test1"),
//new AntPathRequestMatcher("/code-flow/"))
)
		.authorizeHttpRequests(requests -> requests
			//.requestMatchers("/test1").permitAll()
			.anyRequest().permitAll()
			)
		.csrf(AbstractHttpConfigurer::disable)
		.cors(cors -> cors.configurationSource(corsConfigurationSource()))
//		.cors(cors -> cors.configurationSource(request -> {
//	        CorsConfiguration configuration = new CorsConfiguration();
//		    configuration.setAllowedOrigins(Arrays.asList(isceiUiBaseUrl
//		    		//, "http://example.com"
//		    		));
//		    configuration.setAllowedMethods(Arrays.asList(
//	                HttpMethod.GET.name(), 
//	                HttpMethod.POST.name(), 
//	                HttpMethod.OPTIONS.name()));	        
//		    configuration.setAllowedHeaders(Arrays.asList("*"));
//	        return configuration;
//	    }))
//		.csrf(AbstractHttpConfigurer::disable)
		;
        
        return http.build();
	}
	
	@Bean
	@Order(2)
	public SecurityFilterChain securityFilterChain(HttpSecurity http) throws Exception {
		http
//		.securityMatcher("/api/v1/approval-request/user")
//		.securityMatcher("/api/v1/auth/associate-profiles")
		.authorizeHttpRequests(requests -> requests
				.requestMatchers("/api/v1/approval-request/user").authenticated()
				.requestMatchers("/api/v1/auth/associate-profiles").authenticated()
				
				
//				.pathMatchers("/test1").hasAnyAuthority("uma_authorization")
//				.pathMatchers("/test1").hasAnyRole("uma_authorization")
			//.pathMatchers("/test1")
			//.hasAuthority("test2")
			.anyRequest().authenticated()
			)
		.csrf(AbstractHttpConfigurer::disable)
//		.oauth2Client(Customizer.withDefaults())
//		.oauth2Login(Customizer.withDefaults())
		//This is needed
		// .oauth2ResourceServer(c -> c.jwt(c -> c.j))
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

	@Bean
    public OAuth2AuthorizedClientManager authorizedClientManager(final ClientRegistrationRepository clientRegistrationRepository, final OAuth2AuthorizedClientService authorizedClientService) {
        return new AuthorizedClientServiceOAuth2AuthorizedClientManager(clientRegistrationRepository, authorizedClientService);
    }
    
//	
//	@Bean
//	public WebClient rueiWebClient(ClientRegistrationRepository clientRegistrationRepo, 
//	  ServerOAuth2AuthorizedClientRepository authorizedClientRepo) {
//		
//	    ServerOAuth2AuthorizedClientExchangeFilterFunction filter = 
//	      new ServerOAuth2AuthorizedClientExchangeFilterFunction(clientRegistrationRepo, authorizedClientRepo);
//	    
//	    return WebClient.builder().baseUrl(rueiBaseUrl).filter(filter).build();
//	}
//	
//	@Bean
//	public WebClient reiWebClient(ReactiveClientRegistrationRepository clientRegistrationRepo, 
//	  ServerOAuth2AuthorizedClientRepository authorizedClientRepo) {
//		
//	    ServerOAuth2AuthorizedClientExchangeFilterFunction filter = 
//	      new ServerOAuth2AuthorizedClientExchangeFilterFunction(clientRegistrationRepo, authorizedClientRepo);
//	    
//	    return WebClient.builder().filter(filter).build();
//	}
	
//    private Converter<Jwt, Mono<AbstractAuthenticationToken>> grantedAuthoritiesExtractor() {
//        return new ReactiveJwtAuthenticationConverterAdapter(new GrantedAuthoritiesExtractor());
//    }
//
//    @SuppressWarnings("unchecked")
//    static class GrantedAuthoritiesExtractor implements Converter<Jwt, AbstractAuthenticationToken> {
//        public AbstractAuthenticationToken convert(Jwt jwt) {
//            List<SimpleGrantedAuthority> authorities = ((Map<String, Collection<?>>) jwt.getClaims().getOrDefault("realm_access", Collections.emptyMap()))
//                    .getOrDefault("roles", Collections.emptyList())
//                    .stream()
//                    .map(Object::toString)
//                    .map(SimpleGrantedAuthority::new)
//                    .toList();
//            
//            System.out.println("============> authorities = " + authorities);
//            
//            return new JwtAuthenticationToken(jwt, authorities);
//        }
//    }
    
//    @Bean
//    public AuthorizedClientServiceOAuth2AuthorizedClientManager authorizedClientServiceAndManager (
//            ClientRegistrationRepository clientRegistrationRepository,
//            OAuth2AuthorizedClientService authorizedClientService) {
//
//        OAuth2AuthorizedClientProvider authorizedClientProvider =
//                OAuth2AuthorizedClientProviderBuilder.builder()
//                        .clientCredentials()
//                        .build();
//
//        AuthorizedClientServiceOAuth2AuthorizedClientManager authorizedClientManager =
//                new AuthorizedClientServiceOAuth2AuthorizedClientManager(
//                        clientRegistrationRepository, authorizedClientService);
//        authorizedClientManager.setAuthorizedClientProvider(authorizedClientProvider);
//
//        return authorizedClientManager;
//    }
}
