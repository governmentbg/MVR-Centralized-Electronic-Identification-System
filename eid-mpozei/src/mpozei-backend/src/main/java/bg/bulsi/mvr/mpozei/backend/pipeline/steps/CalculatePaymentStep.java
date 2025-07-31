package bg.bulsi.mvr.mpozei.backend.pipeline.steps;

import bg.bulsi.mvr.common.pipeline.PipelineStatus;
import bg.bulsi.mvr.common.pipeline.Step;
import bg.bulsi.mvr.mpozei.backend.client.RaeiceiClient;
import bg.bulsi.mvr.mpozei.backend.dto.CalculateTariffRequest;
import bg.bulsi.mvr.mpozei.backend.dto.CalculateTariffResponse;
import bg.bulsi.mvr.mpozei.backend.mapper.ApplicationMapper;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Component;

import static bg.bulsi.mvr.common.pipeline.PipelineStatus.CALCULATE_PAYMENT;

@Slf4j
@Component
@RequiredArgsConstructor
public class CalculatePaymentStep extends Step<AbstractApplication> {
    private final RaeiceiClient raeiceiClient;
    private final ApplicationMapper applicationMapper;

    @Override
    public AbstractApplication process(AbstractApplication application) {
        log.info("Application with id: {} entered CalculatePaymentStep", application.getId());
    	
        CalculateTariffRequest tariffRequest = applicationMapper.mapToTariffRequest(application);
        CalculateTariffResponse tariffResponse = raeiceiClient.calculateTariff(tariffRequest);
        application.getParams().setFee(tariffResponse.getPrimaryPrice());
        application.getParams().setFeeCurrency(tariffResponse.getPrimaryCurrency().toString());
        application.getParams().setSecondaryFee(tariffResponse.getSecondaryPrice());
        application.getParams().setSecondaryFeeCurrency(tariffResponse.getSecondaryCurrency().toString());
        return application;
    }

    @Override
    public PipelineStatus getStatus() {
        return CALCULATE_PAYMENT;
    }
}
