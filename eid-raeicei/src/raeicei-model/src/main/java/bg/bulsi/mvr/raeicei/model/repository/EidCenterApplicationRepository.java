package bg.bulsi.mvr.raeicei.model.repository;

import bg.bulsi.mvr.raeicei.contract.dto.EidApplicationFilter;
import bg.bulsi.mvr.raeicei.model.entity.application.EidCenterApplication;
import bg.bulsi.mvr.raeicei.model.enums.ApplicationType;
import bg.bulsi.mvr.raeicei.model.repository.view.EidApplicationView;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;

import java.util.UUID;

@Repository
public interface EidCenterApplicationRepository extends JpaRepository<EidCenterApplication, UUID> {

    @Query("""
                SELECT
                    app.id as id,
                    app.applicationNumber.id as applicationNumber,
                    app.companyName as eidName,
                    app.applicationType as applicationType,
                    app.status as status,
                    app.createDate as createDate
                FROM EidCenterApplication app
                WHERE (:#{#filter.applicationNumber} IS NULL OR app.applicationNumber.id LIKE CONCAT('%', :#{#filter.applicationNumber}, '%'))
                  AND (:#{#filter.eidName} IS NULL OR app.companyName LIKE CONCAT('%', :#{#filter.eidName}, '%'))
                  AND (:applicationType IS NULL OR app.applicationType = :applicationType)
                  AND (:#{#filter.status} IS NULL OR app.status = :#{#filter.status})
                  AND (:#{#filter.applicantEid} IS NULL OR app.applicant.eIdentity = :#{#filter.applicantEid})
                  AND (:#{#filter.eikNumber} IS NULL OR app.eikNumber = :#{#filter.eikNumber})
            """)
    Page<EidApplicationView> findByFilter(@Param("filter") EidApplicationFilter filter, @Param("applicationType") ApplicationType applicationType, Pageable pageable);
}
