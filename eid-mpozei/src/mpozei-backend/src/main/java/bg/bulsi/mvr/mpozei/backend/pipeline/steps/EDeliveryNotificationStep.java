package bg.bulsi.mvr.mpozei.backend.pipeline.steps;

import bg.bulsi.mvr.common.pipeline.PipelineStatus;
import bg.bulsi.mvr.common.pipeline.Step;
import bg.bulsi.mvr.mpozei.backend.dto.SsevSendMessageDTO;
import bg.bulsi.mvr.mpozei.backend.mapper.ApplicationMapper;
import bg.bulsi.mvr.mpozei.backend.service.SsevNotificationService;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Component;

@Slf4j
@RequiredArgsConstructor
@Component(value = "еDeliveryNotificationStep")
public class EDeliveryNotificationStep extends Step<AbstractApplication> {
    private final SsevNotificationService ssevNotificationService;
    private final ApplicationMapper applicationMapper;

    @Override
    public AbstractApplication process(AbstractApplication application) {
        log.info("Application with id: {} entered EDeliveryNotificationStep", application.getId());
        SsevSendMessageDTO dto = applicationMapper.mapToSsevRequest(application);
        dto =ssevNotificationService.sendMessage(dto);
        application.getParams().setEDeliveryStatus(dto.getEDeliveryStatus());
        application.getParams().setEDeliveryProfileId(dto.getEDeliveryProfileId());

        return application;
    }

    @Override
    public PipelineStatus getStatus() {
        return PipelineStatus.E_DELIVERY_NOTIFICATION;
    }
}
