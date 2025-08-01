package bg.bulsi.mvr.iscei.model.repository.jpa;

import java.time.OffsetDateTime;
import java.util.List;
import java.util.Optional;
import java.util.UUID;

import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;

import bg.bulsi.mvr.iscei.contract.dto.ReportRequestsCountDto;
import bg.bulsi.mvr.iscei.contract.dto.ReportRequestsTotalDto;
import bg.bulsi.mvr.iscei.contract.dto.ReportDetailedDto;
import bg.bulsi.mvr.iscei.model.AuthenticationStatistic;

public interface AuthenticationStatisticsRepository extends JpaRepository<AuthenticationStatistic, UUID> { 
//  AND (:createDateFrom IS NULL OR s.createDate >= :createDateFrom)
//  AND (:createDateTo   IS NULL OR s.createDate <= :createDateTo)
	@Query("""
			SELECT
			  s.clientId AS clientId,
			  s.systemId AS systemId, 
			  s.systemName AS systemName, 
			  COUNT(s.id) AS totalRequests 
			FROM AuthenticationStatistic AS s
			WHERE s.statisticType = 'REQUEST'
              AND s.createDate between :createDateFrom and :createDateTo
              AND (:systemType     IS NULL OR s.systemType = :systemType)
              AND (:systemId     IS NULL OR s.systemId = :systemId)
              AND (:clientId     IS NULL OR s.clientId = :clientId)
			GROUP BY s.systemId, s.systemName, s.clientId ORDER BY totalRequests DESC
			""" )
	Optional <List<ReportRequestsCountDto>> requestsCount(@Param(value = "createDateFrom") OffsetDateTime createDateFrom,
			@Param(value = "createDateTo") OffsetDateTime createDateTo,
			@Param(value = "systemType") String systemType,
			@Param(value = "systemId") String systemId,
			@Param(value = "clientId") String clientId);
	
	@Query("""
			SELECT 
			s.createDate AS createDate, 
			s.systemId AS systemId, 
			s.systemName AS systemName, 
			s.requesterIpAddress AS requesterIpAddress, 
			s.eidentityId AS eidentityId,
			s.x509CertificateSn AS x509CertificateSn,
			s.x509CertificateIssuerDn AS x509CertificateIssuerDn, 
			s.x509CertificateId AS x509CertificateId,
			s.levelOfAssurance AS levelOfAssurance, 
			s.success AS success
			FROM AuthenticationStatistic AS s
			WHERE s.statisticType = 'RESULT'
              AND s.createDate between :createDateFrom and :createDateTo
              AND (:systemType     IS NULL OR s.systemType = :systemType)
              AND (:systemId     IS NULL OR s.systemId = :systemId)
              AND (:clientId     IS NULL OR s.clientId = :clientId)
              AND (:isEmployee IS NULL OR s.isEmployee = :isEmployee)
              AND (:success     IS NULL OR s.success = :success)
			ORDER BY s.createDate DESC
			""" )
	Optional <List<ReportDetailedDto>> reportDetailed(@Param(value = "createDateFrom") OffsetDateTime createDateFrom,
			@Param(value = "createDateTo") OffsetDateTime createDateTo,
			@Param(value = "systemType") String systemType,
			@Param(value = "systemId") String systemId,
			@Param(value = "clientId") String clientId,
			@Param(value = "isEmployee") Boolean isEmployee,
			@Param(value = "success") Boolean success);
	
	
	  @Query("""
		      SELECT
		        s.clientId as clientId,
		        MONTH(s.createDate)  AS month, 
		        COUNT(s)            AS count
	  			FROM AuthenticationStatistic AS s
		     WHERE YEAR(s.createDate) = :year
		            AND success = true
		  GROUP BY 
		        s.clientId, MONTH(s.createDate)
		  ORDER BY month
		    """)
	  List<ReportRequestsTotalDto> reportRequestsTotal(@Param(value = "year") int year);
	
}
