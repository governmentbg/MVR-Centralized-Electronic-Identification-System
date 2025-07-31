package bg.bulsi.mvr.mpozei.model.repository;

import bg.bulsi.mvr.mpozei.model.certificate.CertificateHistory;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.stereotype.Repository;

import java.util.List;
import java.util.UUID;

@Repository
public interface CertificateHistoryRepository extends JpaRepository<CertificateHistory, UUID> {
    @Query(value =
            " select ch from CertificateHistory ch " +
            " where (:isAdmin = true " +
            " or ch.status <> bg.bulsi.mvr.mpozei.contract.dto.CertificateStatus.FAILED) " +
            " and ch.certificateId = :certificateId " + 
            " order by ch.createDate DESC")
    List<CertificateHistory> findAllByCertificateId(UUID certificateId, Boolean isAdmin);
}
