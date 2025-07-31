package bg.bulsi.mvr.common.rabbitmq.consumer;

import lombok.AllArgsConstructor;
import org.springframework.amqp.rabbit.annotation.EnableRabbit;
import org.springframework.amqp.rabbit.annotation.RabbitListenerConfigurer;
import org.springframework.amqp.rabbit.config.SimpleRabbitListenerContainerFactory;
import org.springframework.amqp.rabbit.connection.ConnectionFactory;
import org.springframework.amqp.rabbit.listener.RabbitListenerEndpointRegistrar;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.validation.beanvalidation.LocalValidatorFactoryBean;

@Configuration
@EnableRabbit
@AllArgsConstructor
public class RabbitValidatorConfig implements RabbitListenerConfigurer {
    private LocalValidatorFactoryBean validator;
    private PreReceiveRabbitPostProcessor postProcessor;

    @Override
    public void configureRabbitListeners(RabbitListenerEndpointRegistrar registrar) {
        registrar.setValidator(this.validator);
    }

    @Bean
    public SimpleRabbitListenerContainerFactory simpleRabbitListenerContainerFactory(ConnectionFactory connectionFactory) {
        SimpleRabbitListenerContainerFactory factory = new SimpleRabbitListenerContainerFactory();
        factory.setAfterReceivePostProcessors(postProcessor);
        factory.setConnectionFactory(connectionFactory);
        return factory;
    }
}
