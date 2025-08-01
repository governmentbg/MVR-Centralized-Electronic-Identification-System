package bg.bulsi.mvr.mpozei.backend.service;

import bg.bulsi.mvr.common.exception.ErrorCode;
import bg.bulsi.mvr.common.pipeline.Pipeline;
import bg.bulsi.mvr.mpozei.backend.dto.CertificateConfirmDTO;
import bg.bulsi.mvr.mpozei.backend.dto.EnrollCertificateDTO;
import bg.bulsi.mvr.mpozei.contract.dto.*;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import bg.bulsi.mvr.mpozei.model.repository.view.ApplicationReportByOffice;
import bg.bulsi.mvr.mpozei.model.repository.view.ApplicationReportByOperators;
import bg.bulsi.mvr.mpozei.model.repository.view.ApplicationReportByRegion;
import bg.bulsi.mvr.mpozei.model.repository.view.ApplicationReportTotal;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;

import java.time.LocalDate;
import java.util.List;
import java.util.UUID;

public interface ApplicationService {
    AbstractApplication createDeskApplication(DeskApplicationRequest request);

    AbstractApplication createOnlineCertStatusApplicationPlain(OnlineCertStatusApplicationRequest request);

	AbstractApplication createOnlineCertStatusApplicationSigned(OnlineApplicationRequest request);
    
    AbstractApplication createPersoCentreApplication(PersoCentreApplicationRequest request);

    AbstractApplication createOnlineApplication(OnlineApplicationRequest request);

    AbstractApplication processApplication(AbstractApplication application);

    <T extends Pipeline> AbstractApplication processApplication(AbstractApplication application, ErrorCode errorCode, Class<T> expectedPipelineClass);
    
    AbstractApplication getAbstractApplicationById(UUID id);

//    AbstractApplication getAbstractApplicationById(UUID id, UUID eidentityId);

    /**
     * Additional data initialisation occurs on the spot i.e. same as FetchType.EAGER
     */
//    AbstractApplication getAbstractApplicationByIdWithAdditionalData(UUID id);
    
    void updateApplication(AbstractApplication application);

    ApplicationStatus denyApplication(DenyApplicationDTO dto);

    ApplicationStatus invalidatePersoCentreApplication(PersoCentreConfirmApplicationRequest request);

    ApplicationStatus updateApplicationStatus(UUID applicationId, ApplicationStatus applicationStatus);

    Page<AbstractApplication> findApplications(ApplicationFilter filter, Pageable pageable);

    Page<AbstractApplication> adminFindApplications(ApplicationFilter filter, Pageable pageable);

    AbstractApplication enrollForCertificate(EnrollCertificateDTO dto);

    AbstractApplication enrollForExtAdminCertificate(CertificateExtAdminRequest dto);

    ApplicationStatus confirmCertificateStorage(CertificateConfirmDTO dto);

    List<AbstractApplication> getDailyUnfinishedApplications();

    boolean getActiveOnlineIssueApplicationsForChipCard(UUID citizenProfileId, UUID eidentityId, UUID currentApplicationId);
    
    void save(AbstractApplication application);

    void saveAll(List<AbstractApplication> application);
    
    void attachCitizenProfileIdToApplications(UUID eidentityId, UUID citizenProfileId);

    List<ApplicationReportByRegion> getApplicationReportByRegion(LocalDate from, LocalDate to);

    List<ApplicationReportByOperators> getJsonApplicationReportByOperators(List<String> operators, LocalDate from, LocalDate to);

    List<ApplicationReportByOperators> getCsvApplicationReportByOperators(List<String> operators, LocalDate from, LocalDate to);

    List<ApplicationReportByOffice> getApplicationReportByOffices(UUID eidAdministratorId, LocalDate from, LocalDate to);

    List<ApplicationReportTotal> getApplicationReportTotal(LocalDate from, LocalDate to);

    byte[] exportConfirmationApplication(AbstractApplication application);
}
