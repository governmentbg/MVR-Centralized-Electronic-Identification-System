package bg.bulsi.mvr.mpozei.backend.pipeline.steps;

import bg.bulsi.mvr.common.pipeline.PipelineStatus;
import bg.bulsi.mvr.common.pipeline.Step;
import bg.bulsi.mvr.mpozei.backend.client.MisepClient;
import bg.bulsi.mvr.mpozei.backend.dto.misep.MisepPaymentRequest;
import bg.bulsi.mvr.mpozei.backend.dto.misep.MisepPaymentResponse;
import bg.bulsi.mvr.mpozei.backend.mapper.ApplicationMapper;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationStatus;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationSubmissionType;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationType;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Component;

import java.util.List;
import java.util.UUID;

import static bg.bulsi.mvr.common.pipeline.PipelineStatus.CREATE_PAYMENT;
import static bg.bulsi.mvr.mpozei.contract.dto.ApplicationSubmissionType.BASE_PROFILE;
import static bg.bulsi.mvr.mpozei.contract.dto.ApplicationSubmissionType.EID;

@Slf4j
@Component
@RequiredArgsConstructor
public class CreatePaymentStep extends Step<AbstractApplication> {
    private final ApplicationMapper applicationMapper;
    private final MisepClient misepClient;

    @Override
    public AbstractApplication process(AbstractApplication application) {
        if (!AbstractApplication.MVR_ADMINISTRATOR_ID.equals(application.getEidAdministratorId())) {
            return application;
        }

        if (application.getParams().getFee() != 0) {
            application.setStatus(ApplicationStatus.PENDING_PAYMENT);
        }
        
        //This is only for the Public part of MPOZEI
        if(application.getParams().getFee() == 0 
        		&& List.of(EID, BASE_PROFILE).contains(application.getSubmissionType())) {
            application.setStatus(ApplicationStatus.PAID);
            return application;
        }

        
        if(AbstractApplication.MVR_ADMINISTRATOR_ID.equals(application.getEidAdministratorId()) && ApplicationSubmissionType.DESK.equals(application.getSubmissionType())) {
            return application;
        }

        createPayment(application);
        return application;
    }

    @Override
    public PipelineStatus getStatus() {
        return CREATE_PAYMENT;
    }

    private void createPayment(AbstractApplication application) {
        MisepPaymentRequest paymentRequest = applicationMapper.mapToMisepPaymentRequest(application);
        MisepPaymentResponse paymentResponse = misepClient.createPaymentRequest(paymentRequest);
        application.getParams().setMisepPaymentId(paymentResponse.getId());
        application.getParams().setPaymentDeadline(paymentResponse.getPaymentDeadline());
        application.getParams().setPaymentAccessCode(paymentResponse.getAccessCode());
    }
}
