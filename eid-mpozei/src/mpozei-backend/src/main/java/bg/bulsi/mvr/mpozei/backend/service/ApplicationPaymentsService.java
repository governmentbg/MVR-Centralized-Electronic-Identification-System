package bg.bulsi.mvr.mpozei.backend.service;

import bg.bulsi.mvr.audit_logger.dto.EventPayload;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import java.util.List;
import java.util.function.Consumer;

public interface ApplicationPaymentsService {

    List<AbstractApplication> getPendingPaymentApplications();

    void checkApplicationsPaymentStatus(List<AbstractApplication> applications, Consumer<EventPayload> auditEventLogger);
}
