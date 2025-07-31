package bg.bulsi.mvr.common.rabbitmq.producer;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.annotation.Configuration;
import org.springframework.http.codec.ServerCodecConfigurer;
import org.springframework.http.codec.json.Jackson2JsonDecoder;
import org.springframework.http.codec.json.Jackson2JsonEncoder;
import org.springframework.web.reactive.config.EnableWebFlux;
import org.springframework.web.reactive.config.WebFluxConfigurer;

import com.fasterxml.jackson.databind.ObjectMapper;

@Configuration
@EnableWebFlux
public class WebfluxConfig implements WebFluxConfigurer {

    @Autowired
    private ObjectMapper objectMapper;
	
	// This is need for proper DateTime type binding
    @Override
    public void configureHttpMessageCodecs(ServerCodecConfigurer configurer) {
        configurer.defaultCodecs().jackson2JsonEncoder(
            new Jackson2JsonEncoder(objectMapper)
        );
        configurer.defaultCodecs().jackson2JsonDecoder(
            new Jackson2JsonDecoder(objectMapper)
        );
    }
}
