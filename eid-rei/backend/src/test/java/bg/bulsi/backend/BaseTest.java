package bg.bulsi.backend;

import bg.bulsi.mvr.audit_logger.BaseAuditLogger;
import bg.bulsi.backend.rabbitmq.ListenerDispatcher;
import bg.bulsi.mvr.common.rabbitmq.consumer.RabbitConsumerConfig;
import bg.bulsi.mvr.reicontract.dto.CitizenDataDTO;
import bg.bulsi.mvr.reicontract.dto.IdentifierTypeDTO;
import bg.bulsi.reimodel.config.AuditorAwareConfig;
import org.springframework.boot.autoconfigure.EnableAutoConfiguration;
import org.springframework.boot.autoconfigure.liquibase.LiquibaseAutoConfiguration;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.boot.test.mock.mockito.MockBean;
import org.springframework.security.oauth2.jwt.JwtDecoder;
import org.springframework.test.context.ActiveProfiles;

@SpringBootTest
@EnableAutoConfiguration(exclude = {LiquibaseAutoConfiguration.class})
@ActiveProfiles({"h2-unit-test", "logging-dev"})
public class BaseTest {
    @MockBean
    AuditorAwareConfig auditorAwareConfig;
    @MockBean
    JwtDecoder jwtDecoder;
    @MockBean
    RabbitConsumerConfig rabbitConsumerConfig;
    @MockBean
	BaseAuditLogger auditLogger;
    @MockBean
    ListenerDispatcher listenerDispatcher;

    protected static CitizenDataDTO createCitizenDataDTO() {
        CitizenDataDTO dto = new CitizenDataDTO();
        dto.setCitizenIdentifierNumber("7608097458");
        //dto.setCitizenIdentifierNumber(String.valueOf((long) Math.floor(Math.random() * 9_000_000_000L) + 1_000_000_000L));
        dto.setCitizenIdentifierType(IdentifierTypeDTO.EGN);
        dto.setFirstName("Първо Име");
        dto.setSecondName("Второ Име");
        dto.setLastName("Трето Име");
        return dto;
    }
}
