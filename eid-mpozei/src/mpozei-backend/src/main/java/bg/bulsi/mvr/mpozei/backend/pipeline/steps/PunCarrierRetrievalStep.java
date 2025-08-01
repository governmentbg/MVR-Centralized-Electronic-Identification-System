package bg.bulsi.mvr.mpozei.backend.pipeline.steps;

import bg.bulsi.mvr.common.exception.ErrorCode;
import bg.bulsi.mvr.common.pipeline.PipelineStatus;
import bg.bulsi.mvr.common.pipeline.Step;
import bg.bulsi.mvr.mpozei.backend.client.PunClient;
import bg.bulsi.mvr.mpozei.backend.dto.PunCarrierDTO;
import bg.bulsi.mvr.mpozei.backend.service.RaeiceiService;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationType;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Component;

import java.util.List;

import static bg.bulsi.mvr.common.util.ValidationUtil.assertNotEmpty;

@Slf4j
@Component
@RequiredArgsConstructor
public class PunCarrierRetrievalStep extends Step<AbstractApplication> {
    private final PunClient punClient;
    private final RaeiceiService raeiceiService;

    @Override
    public AbstractApplication process(AbstractApplication application) {
        log.info("Application with id: {} entered PunCarrierRetrievalStep", application.getId());

        //ISSUE_EID does not have PUN Carrier on this step
        if(ApplicationType.ISSUE_EID.equals(application.getApplicationType())) {
        	return application;
        }
        List<PunCarrierDTO> punCarriers = punClient
        		.getPunCarrierByEidentityIdAndCertificateId(application.getEidentityId(), application.getParams().getCertificateId());

        assertNotEmpty(punCarriers, ErrorCode.PUN_CARRIER_CANNOT_BE_NULL);
        PunCarrierDTO punCarrier = punCarriers.get(0);

        application.getParams().setDeviceSerialNumber(punCarrier.getSerialNumber());
     //   DeviceDTO device = raeiceiService.getDeviceById(punCarrier.getPunDeviceId());
//        application.setDeviceId(device.getId());

        return application;
    }

    @Override
    public PipelineStatus getStatus() {
        return PipelineStatus.PUN_CARRIER_RETRIEVAL;
    }
}
