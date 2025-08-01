package bg.bulsi.mvr.mpozei.backend.service;


import bg.bulsi.mvr.mpozei.contract.dto.*;
import bg.bulsi.mvr.mpozei.model.certificate.CertificateHistory;
import org.springframework.data.domain.Page;

import java.util.List;
import java.util.UUID;

public interface CertificateService {
    CitizenCertificateSummaryResponse getCertificateById(UUID id);

    Page<FindCertificateResponse> findCitizenCertificates(CitizenCertificateFilter filter);

    void createCertificateHistory(CertificateHistory history);

    List<CertificateHistory> getCertificateHistoryByCertificateId(UUID certificateId);

    NaifUpdateCertificateStatusResponse updateCertificateStatusByNaif(NaifUpdateCertificateStatusDTO dto);

    NaifDeliveredCertificateResponse activateCertificateByNaif(NaifDeliveredCertificateDTO dto);

    NaifDeviceHistoryResponse getDeviceHistoryByNaif(NaifDeviceHistoryRequest request);
}
