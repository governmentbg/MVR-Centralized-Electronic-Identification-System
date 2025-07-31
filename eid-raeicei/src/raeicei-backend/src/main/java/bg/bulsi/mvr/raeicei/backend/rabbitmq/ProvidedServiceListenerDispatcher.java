package bg.bulsi.mvr.raeicei.backend.rabbitmq;

import bg.bulsi.mvr.common.rabbitmq.consumer.DefaultRabbitListener;
import bg.bulsi.mvr.raeicei.backend.facade.ProvidedServiceFacade;
import bg.bulsi.mvr.raeicei.contract.dto.EidServiceTypeDTO;
import bg.bulsi.mvr.raeicei.contract.dto.HttpResponse;
import bg.bulsi.mvr.raeicei.contract.dto.ProvidedServiceRequestDTO;
import bg.bulsi.mvr.raeicei.contract.dto.ProvidedServiceResponseDTO;
import bg.bulsi.mvr.raeicei.model.enums.EidServiceType;
import jakarta.validation.Valid;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.amqp.support.converter.RemoteInvocationResult;
import org.springframework.messaging.handler.annotation.Payload;
import org.springframework.stereotype.Component;
import org.springframework.transaction.annotation.Transactional;

import java.util.List;
import java.util.Map;
import java.util.UUID;

@Slf4j
@Component
@RequiredArgsConstructor
public class ProvidedServiceListenerDispatcher {
	
    private final ProvidedServiceFacade providedServiceFacade;

    @DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.api.v1.providedservice.getall")
    @Transactional
    public RemoteInvocationResult getAllProvidedServices() {
        List<ProvidedServiceResponseDTO> result = providedServiceFacade.getAllProvidedServices();
        return new RemoteInvocationResult(result);
    }
    
    @DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.api.v1.providedservice.get.id")
    @Transactional
    public RemoteInvocationResult getProvidedServiceById(@Payload UUID id) {
    	ProvidedServiceResponseDTO result = providedServiceFacade.getProvidedServiceById(id);
        return new RemoteInvocationResult(result);
    }
    
    @DefaultRabbitListener(queues = "raeicei.rpcqueue.GET.raeicei.api.v1.providedservice.get.applicationtype.type")
    @Transactional
    public RemoteInvocationResult getProvidedServiceByApplicationType(@Payload EidServiceTypeDTO applicationType) {
    	ProvidedServiceResponseDTO result = 
    			providedServiceFacade.getProvidedServiceByApplicationType( EidServiceType.valueOf(applicationType.name()));
        return new RemoteInvocationResult(result);
    }
    
    @DefaultRabbitListener(queues = "raeicei.rpcqueue.POST.raeicei.api.v1.providedservice.create")
    @Transactional
    public RemoteInvocationResult createProvidedService(@Payload ProvidedServiceRequestDTO dto) {
    	ProvidedServiceResponseDTO result = providedServiceFacade.createProvidedService(dto);
        return new RemoteInvocationResult(result);
    }
    
    @DefaultRabbitListener(queues = "raeicei.rpcqueue.PUT.raeicei.api.v1.providedservice.update.id")
    @Transactional
    public RemoteInvocationResult updateProvidedService(@Payload Map<String, Object> messageBody) {
    	UUID id = (UUID) messageBody.get("id");
    	ProvidedServiceRequestDTO providedServiceRequestDTO = (ProvidedServiceRequestDTO) messageBody.get("providedServiceRequestDTO");
    	
    	ProvidedServiceResponseDTO result = providedServiceFacade.updateProvidedService(id, providedServiceRequestDTO);
    	
        return new RemoteInvocationResult(result);
    }

    @Transactional
    @DefaultRabbitListener(queues = "raeicei.rpcqueue.DELETE.raeicei.api.v1.providedservice.delete.id")
    public RemoteInvocationResult deleteProvidedService(@Valid @Payload UUID id) {
        providedServiceFacade.deleteProvidedService(id);
        return new RemoteInvocationResult(HttpResponse.builder().message("SUCCESS").statusCode(200).build());
    }
}
