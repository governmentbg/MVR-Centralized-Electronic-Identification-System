package bg.bulsi.mvr.mpozei.model.repository;

import bg.bulsi.mvr.mpozei.contract.dto.ApplicationStatus;
import bg.bulsi.mvr.mpozei.contract.dto.DbApplicationFilter;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import bg.bulsi.mvr.mpozei.model.repository.view.ApplicationReportByOffice;
import bg.bulsi.mvr.mpozei.model.repository.view.ApplicationReportByOperators;
import bg.bulsi.mvr.mpozei.model.repository.view.ApplicationReportByRegion;
import bg.bulsi.mvr.mpozei.model.repository.view.ApplicationReportTotal;

import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.data.jpa.repository.EntityGraph;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;

import java.time.LocalDate;
import java.time.LocalDateTime;
import java.util.List;
import java.util.Optional;
import java.util.UUID;

@Repository
public interface ApplicationRepository<T extends AbstractApplication> extends JpaRepository<T, UUID> {

//	"LEFT JOIN FETCH a.applicationNumber " +
//	"LEFT JOIN FETCH a.reason " +
	String FIND_BY_FILTER_QUERY = 
			"SELECT a FROM AbstractApplication a " +
		            "WHERE (:#{#filter.applicationId} IS NULL OR cast(a.id as string) like concat('%', :#{#filter.applicationId}, '%')) " +
		            "AND (:#{#filter.applicationNumber} IS NULL OR a.applicationNumber.id like concat('%', :#{#filter.applicationNumber}, '%')) " +
		            "AND ((cast(:#{#filter.createdDateFrom} as text) IS NULL AND cast(:#{#filter.createdDateTo} as text) IS NULL)" + 
                    "OR (a.createDate BETWEEN :#{#filter.createdDateFrom} AND :#{#filter.createdDateTo}))  " +
		            "AND (:#{#filter.deviceIds} IS NULL OR a.deviceId in :#{#filter.deviceIds}) " +
		            "AND (:#{#filter.statuses} IS NULL OR a.status in :#{#filter.statuses}) " +
		            "AND (:#{#filter.eidentityId} IS NULL OR a.eidentityId = :#{#filter.eidentityId}) " +
		            "AND (:#{#filter.citizenProfileId} IS NULL OR a.citizenProfileId = :#{#filter.citizenProfileId}) " +
		            "AND (:#{#filter.applicationTypes} IS NULL OR a.applicationType in :#{#filter.applicationTypes}) " +
		            "AND (:#{#filter.submissionTypes} IS NULL OR a.submissionType in :#{#filter.submissionTypes}) " +
		            "AND (:#{#filter.eidAdministratorId} IS NULL OR a.eidAdministratorId = :#{#filter.eidAdministratorId}) " +
		            "AND (:#{#filter.eidAdministratorFrontOfficeId} IS NULL OR a.administratorFrontOfficeId in :#{#filter.eidAdministratorFrontOfficeId}) ";
	
	
    Page<T> findAllByEidentityId(UUID eidentityId, Pageable pageable);

    @Query(FIND_BY_FILTER_QUERY)
    Page<T> findByFilter(@Param("filter") DbApplicationFilter filter, Pageable pageable);

    
    Optional<T> findByIdAndEidentityId(UUID id, UUID eidentityId);

    Optional<T> findByIdAndEidAdministratorId(UUID id, UUID eidAdministratorId);

	@EntityGraph(attributePaths = {"reason", "eidAdministrator"})
	@Query("SELECT a FROM AbstractApplication a where a.id=:id")
	Optional<T> getAbstractApplicationByIdWithAdditionalData(@Param("id") UUID id);

	@Query(value = """
			SELECT count(a) > 0 FROM AbstractApplication a 
			where a.deviceId=:deviceId 
			AND a.submissionType IN ('BASE_PROFILE', 'EID')
			AND a.applicationType='ISSUE_EID'
			AND a.id <> :currentApplicationId
			AND a.status NOT IN ('COMPLETED', 'DENIED', 'PAYMENT_EXPIRED')
			AND (a.citizenProfileId=:citizenProfileId OR a.eidentityId=:eidentityId)
			""")
    boolean getActiveOnlineIssueApplicationsForChipCard(
			@Param("citizenProfileId") UUID citizenProfileId, @Param("eidentityId") UUID eidentityId, @Param("deviceId") UUID deviceId, @Param("currentApplicationId") UUID currentApplicationId);
	
	@Query(value = """
			         SELECT a FROM AbstractApplication a
			         WHERE ( a.status = bg.bulsi.mvr.mpozei.contract.dto.ApplicationStatus.PENDING_PAYMENT and a.lastUpdate <= :unpaidStartDate )
			                 OR (a.status NOT IN (bg.bulsi.mvr.mpozei.contract.dto.ApplicationStatus.COMPLETED,
			                                      bg.bulsi.mvr.mpozei.contract.dto.ApplicationStatus.DENIED,
			                                      bg.bulsi.mvr.mpozei.contract.dto.ApplicationStatus.PAID,
			                                      bg.bulsi.mvr.mpozei.contract.dto.ApplicationStatus.CERTIFICATE_STORED,
			                                      bg.bulsi.mvr.mpozei.contract.dto.ApplicationStatus.GENERATED_CERTIFICATE)
			                 AND a.createDate <= :uncompletedStartDate)
			        """)
    List<T> getUnfinishedApplications(@Param("unpaidStartDate") LocalDateTime unpaidStartDate, @Param("uncompletedStartDate") LocalDateTime uncompletedStartDate);


    @Query(nativeQuery = true,
            value = " select a.params->>'region' as region, count(*) AS certificatesAmount from mpozei.application a " +
                    " where a.status = 'COMPLETED' " +
                    "       and a.application_type = 'ISSUE_EID' " +
                    "       and a.create_date between :from and :to " +
                    " group by a.params->>'region' ")
    List<ApplicationReportByRegion> getReportByRegion(@Param("from") LocalDate from, @Param("to") LocalDate to);

    @Query(nativeQuery = true,
            value = " select a.administrator_front_office_id as administratorFrontOfficeId, count(*) AS certificatesAmount from mpozei.application a " +
                    " where a.eid_administrator_id = :administratorId " +
                    "       and a.status = 'COMPLETED' " +
                    "       and a.application_type = 'ISSUE_EID' " +
                    "       and a.create_date between :from and :to " +
                    " group by administratorFrontOfficeId ")
    List<ApplicationReportByOffice> getReportByOffice(@Param("administratorId") UUID administratorId, @Param("from") LocalDate from, @Param("to") LocalDate to);

    @Query(nativeQuery = true,
            value = " SELECT " +
                    " updated_by AS operatorUsername, " +
                    " COUNT(CASE WHEN status = 'SUBMITTED' THEN 1 END) AS submitted, " +
                    " COUNT(CASE WHEN status = 'SIGNED' THEN 1 END) AS signed, " +
                    " COUNT(CASE WHEN status = 'PAID' THEN 1 END) AS paid, " +
                    " COUNT(CASE WHEN status = 'APPROVED' THEN 1 END) AS approved, " +
                    " COUNT(CASE WHEN status = 'GENERATED_CERTIFICATE' THEN 1 END) AS generatedCertificate, " +
                    " COUNT(CASE WHEN status = 'COMPLETED' THEN 1 END) AS completed, " +
                    " COUNT(CASE WHEN status = 'DENIED' THEN 1 END) AS denied " +
                    " FROM mpozei.application_aud " +
                    " WHERE updated_by IN (:operators) " +
                    " AND last_update BETWEEN :from AND :to " +
                    " GROUP BY updated_by ")
    List<ApplicationReportByOperators> getReportByOperators(@Param("operators") List<String> operators, @Param("from") LocalDate from, @Param("to") LocalDate to);

    @Query(nativeQuery = true,
            value = " select count(*) as applicationsCount, a.application_type as applicationType from mpozei.application a " +
                    " where a.create_date between :from and :to " +
                    " group by a.application_type ")
    List<ApplicationReportTotal> getReportTotal(@Param("from") LocalDate from, @Param("to") LocalDate to);

    @Query(FIND_BY_FILTER_QUERY)
    List<T> findByFilter(@Param("filter") DbApplicationFilter filter);
    
    @Query(value = " SELECT a "
            		+ "FROM AbstractApplication a "
            		+ "WHERE jsonb_extract_path_text(a.params, 'misepPaymentId') IS NOT NULL "
            		+ "AND a.status = :applicationStatus")
    List<T> findAllByStatusAndMisepPaymentIdNotNull(@Param("applicationStatus")ApplicationStatus applicationStatus);
}
