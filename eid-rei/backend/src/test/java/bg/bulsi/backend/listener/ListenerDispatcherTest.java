package bg.bulsi.backend.listener;

import bg.bulsi.backend.BaseTest;
import bg.bulsi.backend.rabbitmq.ListenerDispatcher;
import bg.bulsi.backend.service.CitizenIdentifierService;
import bg.bulsi.mvr.reicontract.dto.CitizenDataDTO;
import bg.bulsi.reimodel.repository.view.CitizenIdentifierView;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.test.context.ActiveProfiles;

import java.util.HashMap;
import java.util.Map;

@ActiveProfiles({"h2-unit-test", "logging-dev"})
public class ListenerDispatcherTest extends BaseTest {
    @Autowired
    private CitizenIdentifierService citizenIdentifierService;
    @Autowired
    private ListenerDispatcher listenerDispatcher;

    @Test
    public void createCitizenIdentifier_Should_Succeed() {
        // Arrange
        CitizenDataDTO dto = createCitizenDataDTO();
        // Act // Assert
        listenerDispatcher.createCitizenIdentifier(dto);
    }

    @Test
    public void updateCitizenIdentifier_Should_Succeed() {
        // Arrange
        CitizenDataDTO dto = createCitizenDataDTO();
        CitizenIdentifierView saved = citizenIdentifierService.create(dto);
        Map<String, Object> map = new HashMap<>();
        map.put("id", saved.getEidentityId());
        map.put("citizenDataDTO", dto);
        // Act // Assert
        listenerDispatcher.updateCitizenIdentifier(map);
    }

    @Test
    public void updateCitizenIdentifierActive_Should_Succeed() {
        // Arrange
        CitizenIdentifierView initial = citizenIdentifierService.create(createCitizenDataDTO());
        Map<String, Object> map = new HashMap<>();
        map.put("id", initial.getEidentityId());
        map.put("isActive", false);
        // Act Assert
        listenerDispatcher.updateCitizenIdentifierActive(map);
    }

    @Test
    public void getCitizenIdentifier_Should_Succeed() {
        // Arrange
        CitizenIdentifierView initial = citizenIdentifierService.create(createCitizenDataDTO());
        // Act Assert
        listenerDispatcher.getCitizenIdentifier(initial.getEidentityId());
    }
}
