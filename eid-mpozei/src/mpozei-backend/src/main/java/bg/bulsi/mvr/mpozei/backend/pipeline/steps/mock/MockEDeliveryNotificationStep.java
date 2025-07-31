//package bg.bulsi.mvr.mpozei.backend.pipeline.steps.mock;
//
//import bg.bulsi.mvr.common.pipeline.PipelineStatus;
//import bg.bulsi.mvr.common.pipeline.Step;
//import bg.bulsi.mvr.mpozei.backend.client.EDeliveryClient;
//import bg.bulsi.mvr.mpozei.backend.client.RueiClient;
//import bg.bulsi.mvr.mpozei.backend.dto.CitizenCertificateDetailsDTO;
//import bg.bulsi.mvr.mpozei.backend.dto.edelivery.EDeliverySearchProfileDTO;
//import bg.bulsi.mvr.mpozei.backend.mapper.ApplicationMapper;
//import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
//import lombok.RequiredArgsConstructor;
//import lombok.extern.slf4j.Slf4j;
//import org.springframework.boot.autoconfigure.condition.ConditionalOnProperty;
//import org.springframework.stereotype.Component;
//
//@Slf4j
//@RequiredArgsConstructor
//@Component(value="еDeliveryNotificationStep")
//@ConditionalOnProperty(prefix = "certificate-creation", name = "dev", havingValue = "true")
//public class MockEDeliveryNotificationStep extends Step<AbstractApplication> {
//    private final EDeliveryClient eDeliveryClient;
//    private final RueiClient rueiClient;
//    private final ApplicationMapper applicationMapper;
//
//    @Override
//    public AbstractApplication process(AbstractApplication application) {
//        log.info("Application with id: {} entered EDeliveryNotificationStep", application.getId());
//        try {
//        CitizenCertificateDetailsDTO certificate = rueiClient.getCitizenCertificateByIssuerAndSN(application.getParams().getIssuerDn(), application.getParams().getCertificateSerialNumber());
//        EDeliverySearchProfileDTO dto = applicationMapper.mapToEDeliveryMessageRequest(certificate);
//        dto.setReceiverUniqueIdentifier(application.getCitizenIdentifierNumber());
//        dto.setSenderUniqueIdentifier("201203809");
//        dto.setOperatorEGN("8206061432");
//        String email = application.getParams().getEmail();
//        if (email == null) {
//            return application;
//        }
//        dto.setReceiverEmail(email);
//        log.info("EDelivery request: " + dto);
//        eDeliveryClient.sendEDeliveryNotification(dto);
//        } catch (Exception e) {
//            log.error("There was a problem during sending the notification \n {}", e.toString());
//        }
//        return application;
//    }
//
//    @Override
//    public PipelineStatus getStatus() {
//        return PipelineStatus.E_DELIVERY_NOTIFICATION;
//    }
//}
