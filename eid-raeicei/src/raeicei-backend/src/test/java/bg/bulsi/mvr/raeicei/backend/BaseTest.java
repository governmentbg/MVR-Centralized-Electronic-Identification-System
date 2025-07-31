package bg.bulsi.mvr.raeicei.backend;

import bg.bulsi.mvr.audit_logger.BaseAuditLogger;
import bg.bulsi.mvr.common.rabbitmq.consumer.RabbitConsumerConfig;
import org.mockito.Mockito;
import org.springframework.amqp.rabbit.connection.ConnectionFactory;
import org.springframework.boot.autoconfigure.EnableAutoConfiguration;
import org.springframework.boot.autoconfigure.amqp.RabbitAutoConfiguration;
import org.springframework.boot.autoconfigure.liquibase.LiquibaseAutoConfiguration;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.boot.test.mock.mockito.MockBean;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.data.redis.connection.RedisConnection;
import org.springframework.data.redis.connection.RedisConnectionFactory;
import org.springframework.data.redis.serializer.RedisSerializer;
import org.springframework.security.oauth2.client.OAuth2AuthorizedClientService;
import org.springframework.security.oauth2.client.registration.ClientRegistrationRepository;
import org.springframework.security.oauth2.jwt.JwtDecoder;
import org.springframework.test.context.ActiveProfiles;

@SpringBootTest
@EnableAutoConfiguration(exclude = {LiquibaseAutoConfiguration.class, RabbitAutoConfiguration.class})
@ActiveProfiles({"h2-unit-test", "logging-dev"})
public abstract class BaseTest {

    @MockBean
    JwtDecoder jwtDecoder;
    @MockBean
    RabbitConsumerConfig rabbitConsumerConfig;
    @MockBean
    ClientRegistrationRepository clientRegistrationRepository;
    @MockBean
    OAuth2AuthorizedClientService oAuth2AuthorizedClientService;
    @MockBean
	BaseAuditLogger auditLogger;
    @MockBean
    ConnectionFactory connectionFactory;


    @Configuration
    static class Config
    {
        @Bean
        @SuppressWarnings("unchecked")
        public RedisSerializer<Object> defaultRedisSerializer()
        {
            return Mockito.mock(RedisSerializer.class);
        }


        @Bean
        public RedisConnectionFactory connectionFactory()
        {
            RedisConnectionFactory factory = Mockito.mock(RedisConnectionFactory.class);
            RedisConnection connection = Mockito.mock(RedisConnection.class);
            Mockito.when(factory.getConnection()).thenReturn(connection);

            return factory;
        }
    }
}
