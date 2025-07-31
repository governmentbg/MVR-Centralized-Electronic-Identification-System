package bg.bulsi.mvr.iscei.gateway.config;

import org.springframework.context.annotation.Configuration;
import org.springframework.web.servlet.config.annotation.InterceptorRegistry;
import org.springframework.web.servlet.config.annotation.WebMvcConfigurer;

import bg.bulsi.mvr.common.config.ServletUserContextInterceptor;

@Configuration
public class WebConfig implements WebMvcConfigurer {

    @Override
    public void addInterceptors(InterceptorRegistry registry) {
        // Register your interceptor for all paths
        registry.addInterceptor(new ServletUserContextInterceptor())
                .addPathPatterns("/**");
    }
}
