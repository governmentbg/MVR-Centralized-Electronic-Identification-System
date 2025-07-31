package bg.bulsi.mvr.mpozei.backend.pipeline.steps;

import bg.bulsi.mvr.common.pipeline.PipelineStatus;
import bg.bulsi.mvr.common.pipeline.Step;
import bg.bulsi.mvr.mpozei.backend.client.EjbcaClient;
import bg.bulsi.mvr.mpozei.backend.dto.SearchEndEntityDTO;
import bg.bulsi.mvr.mpozei.backend.dto.ejbca.EjbcaEndEntityRequest;
import bg.bulsi.mvr.mpozei.backend.dto.ejbca.EjbcaEndEntityStatusUpdateRequest;
import bg.bulsi.mvr.mpozei.backend.dto.ejbca.EjbcaSearchEndEntityRequest;
import bg.bulsi.mvr.mpozei.backend.dto.ejbca.EjbcaSearchEndEntityResponse;
import bg.bulsi.mvr.mpozei.backend.mapper.ApplicationMapper;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.boot.autoconfigure.condition.ConditionalOnProperty;
import org.springframework.stereotype.Component;

@Slf4j
@RequiredArgsConstructor
@Component(value="ejbcaEndEntityCreationStep")
@ConditionalOnProperty(prefix = "certificate-creation", name = "dev", havingValue = "false")
public class EjbcaEndEntityCreationStep extends Step<AbstractApplication> {
    private final EjbcaClient ejbcaClient;
    private final ApplicationMapper applicationMapper;

    @Override
    public AbstractApplication process(AbstractApplication application) {
        log.info("Application with id: {} entered EjbcaCertificateRevocationStep", application.getId());
        createEndEntityInEjbca(application);
        return application;
    }

    @Override
    public PipelineStatus getStatus() {
        return PipelineStatus.EJBCA_END_ENTITY_CREATION;
    }

    private void createEndEntityInEjbca(AbstractApplication application) {
        log.info("Sending request to create end entity in ejbca for eidentityId: {}", application.getEidentityId());
        EjbcaEndEntityRequest request = applicationMapper.mapToEjbcaEndEntityRequest(application);

        EjbcaSearchEndEntityRequest searchRequest = new EjbcaSearchEndEntityRequest();
        searchRequest.addEqualsCriteria(EjbcaSearchEndEntityRequest.QUERY, application.getEidentityId().toString());
        EjbcaSearchEndEntityResponse existing = ejbcaClient.searchEndEntity(searchRequest);
        if (!existing.getEndEntities().isEmpty()) {
            log.info("End entity already exists for eidentityId: {}", application.getEidentityId());
            SearchEndEntityDTO existingEndEntity = existing.getEndEntities().get(0);
            EjbcaEndEntityStatusUpdateRequest updateRequest = applicationMapper.map(existingEndEntity, application.getEidentityId());
            ejbcaClient.setEndEntityStatus(existingEndEntity.getUsername(), updateRequest);
            return;
        }
        ejbcaClient.createEndEntity(request);
        log.info("Received success response for end entity creation for eidentityId: {}", application.getEidentityId());
    }
}
