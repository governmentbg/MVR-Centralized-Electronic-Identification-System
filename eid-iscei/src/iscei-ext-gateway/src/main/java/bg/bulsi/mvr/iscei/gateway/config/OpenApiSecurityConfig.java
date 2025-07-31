package bg.bulsi.mvr.iscei.gateway.config;

import io.swagger.v3.oas.models.Components;
import io.swagger.v3.oas.models.OpenAPI;
import io.swagger.v3.oas.models.info.Info;
import io.swagger.v3.oas.models.security.SecurityRequirement;
import io.swagger.v3.oas.models.security.SecurityScheme;

import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

@Configuration
public class OpenApiSecurityConfig {


    @Bean
    public OpenAPI openAPI() {
        return new OpenAPI().components(new Components()
                        .addSecuritySchemes("ISCEI", createOAuthScheme()))
                .addSecurityItem(new SecurityRequirement().addList("ISCEI"))
                .info(new Info().title("ISCEI HTTP API")
                        .description("")
                        .version("1.0"));
    }

    private SecurityScheme createOAuthScheme() {
        SecurityScheme securityScheme = new SecurityScheme();
        securityScheme.setType(SecurityScheme.Type.HTTP);
        securityScheme.setBearerFormat("JWT");
        securityScheme.setScheme("bearer");
        
        return securityScheme;
    }
}
