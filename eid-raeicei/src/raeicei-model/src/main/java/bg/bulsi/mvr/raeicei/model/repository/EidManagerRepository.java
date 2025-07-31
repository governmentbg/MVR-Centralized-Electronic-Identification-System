package bg.bulsi.mvr.raeicei.model.repository;

import bg.bulsi.mvr.raeicei.model.entity.EidCenter;
import bg.bulsi.mvr.raeicei.model.entity.EidManager;
import bg.bulsi.mvr.raeicei.model.enums.ManagerType;
import bg.bulsi.mvr.raeicei.model.repository.view.*;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;

import java.util.List;
import java.util.Optional;
import java.util.UUID;

@Repository
public interface EidManagerRepository extends JpaRepository<EidManager, UUID> {
    @Query(nativeQuery = true,
            value = "select a.id, a.name, a.name_latin as nameLatin, a.eik_number as eikNumber, a.address, "
            		+ " a.logo_url as logoUrl, a.download_url as downloadUrl, a.home_page as homePage, a.manager_status as managerStatus, a.code, ARRAY_AGG(DISTINCT fo.id) as eidManagerFrontOfficeIds, "
            		+ "ARRAY_AGG(array[fo.id::varchar, fo.name::varchar , fo.region::varchar]) as eidFrontOffices, "
            		+ "ARRAY_AGG(DISTINCT d.id) as deviceIds "
            		+ "	from raeicei.eid_manager a "
            		+ "		left join raeicei.eid_manager_offices fm on a.id = fm.eid_manager_id "
            		+ "		left join raeicei.front_office fo on fm.office_id = fo.id "
            		+ "		left join raeicei.eid_administrator_device fd on a.id = fd.eid_administrator_id "
            		+ "		left join raeicei.devices d on fd.device_id = d.id where a.service_type = 'EID_ADMINISTRATOR' and a.manager_status = 'ACTIVE' "
            		+ "group by a.id, a.name, a.eik_number ")
    List<EidAdministratorView> getAllEidAdministratorsByQuery();

	@Query(nativeQuery = true,
			value = "   WITH "
					+ " stopped_revisions AS ( "
					+ " 	SELECT mr.id, MAX(mr.rev) AS rev "
					+ " 	FROM raeicei.eid_manager_aud mr "
					+ " 	WHERE mr.manager_status IN ('STOP', 'SUSPENDED') "
					+ " 	GROUP BY mr.id "
					+ " ), "
					+ " target_revisions AS ( "
					+ " 	SELECT mr.id, MAX(mr.rev) AS rev "
					+ " 	FROM raeicei.eid_manager_aud mr "
					+ " 	LEFT JOIN (SELECT * FROM stopped_revisions) st ON st.id = mr.id "
					+ " 	WHERE mr.manager_status = 'ACTIVE' AND (st.rev IS NULL OR mr.rev > st.rev) "
					+ " 	GROUP BY mr.id "
					+ " ) "
					+ " 	SELECT a.id, a.name, a.name_latin AS nameLatin, a.eik_number AS eikNumber, a.address, a.email, a.logo_url as logoUrl, "
					+ " 	a.download_url as downloadUrl, a.home_page AS homePage, a.manager_status AS managerStatus, a.code, a.service_type as serviceType, "
					+ " 	ARRAY_AGG(DISTINCT fo.id) FILTER (WHERE fo.id IS NOT NULL) AS eidManagerFrontOfficeIds, "
					+ " 	json_agg(DISTINCT jsonb_strip_nulls(jsonb_build_object('id', fo.id,'name', fo.name, 'region',fo.region))) AS eidFrontOffices, "
					+ " 	ARRAY_AGG(DISTINCT d.id) AS deviceIds, "
					+ " 	json_agg(DISTINCT jsonb_strip_nulls(jsonb_build_object('id', d.id, 'type', d.type, 'name', d.name, 'description', d.description, "
					+ " 	'authorizationLink', d.authorization_link, 'backchannelAuthorizationLink', d.backchannel_authorization_link, 'isActive', d.is_active))) as devices "
					+ " 	FROM raeicei.eid_manager_aud a "
					+ " 	RIGHT JOIN (SELECT * FROM target_revisions) AS rev ON rev.id = a.id AND rev.rev = a.rev "
					+ " 	LEFT JOIN raeicei.eid_manager_offices fm ON a.id = fm.eid_manager_id "
					+ " 	LEFT JOIN raeicei.front_office fo ON fm.office_id = fo.id "
					+ " 	LEFT JOIN raeicei.eid_administrator_device fd ON a.id = fd.eid_administrator_id "
					+ " 	LEFT JOIN raeicei.devices d ON fd.device_id = d.id "
					+ " 	WHERE a.service_type = 'EID_ADMINISTRATOR' AND a.manager_status = 'ACTIVE' "
					+ " 	GROUP BY a.id, a.name, a.name_latin, a.eik_number, a.address, a.email, a.logo_url, a.download_url, a.home_page, a.manager_status, a.code, a.service_type ")
	List<EidAdministratorView> getAllActiveEidAdministratorsFromAuditByQuery();

	@Query(nativeQuery = true,
			value = "   WITH "
					+ " stopped_revisions AS ( "
					+ " 	SELECT mr.id, MAX(mr.rev) AS rev "
					+ " 	FROM raeicei.eid_manager_aud mr "
					+ " 	WHERE mr.manager_status IN ('STOP', 'SUSPENDED') "
					+ " 	GROUP BY mr.id "
					+ " ), "
					+ " target_revisions AS ( "
					+ " 	SELECT mr.id, MAX(mr.rev) AS rev "
					+ " 	FROM raeicei.eid_manager_aud mr "
					+ " 	LEFT JOIN (SELECT * FROM stopped_revisions) st ON st.id = mr.id "
					+ " 	WHERE mr.manager_status = 'ACTIVE' AND (st.rev IS NULL OR mr.rev > st.rev) "
					+ " 	GROUP BY mr.id "
					+ " ) "
					+ " 	SELECT a.name, a.eik_number AS eikNumber, a.address, a.email, a.home_page AS homePage, fo.description as frontOfficeInfo, "
					+ " 	fo.region as frontOfficeRegion, fo.latitude as frontOfficeLatitude, fo.longitude as frontOfficeLongitude, "
					+ " 	fo.location as frontOfficeLocation, fo.contact as FrontOfficePhoneNumber, fo.working_hours as frontOfficeWorkingHours "
					+ " 	FROM raeicei.eid_manager_aud a "
					+ " 	RIGHT JOIN (SELECT * FROM target_revisions) AS rev ON rev.id = a.id AND rev.rev = a.rev "
					+ " 	LEFT JOIN raeicei.eid_manager_offices fm ON a.id = fm.eid_manager_id "
					+ " 	LEFT JOIN raeicei.front_office fo ON fm.office_id = fo.id "
					+ " 	WHERE a.service_type = 'EID_ADMINISTRATOR' AND a.manager_status = 'ACTIVE' AND fo.is_active = 'TRUE' ")
	List<ReportOfEidManagers> getEidAdministratorsReport();

	@Query(nativeQuery = true,
			value = "   WITH "
					+ " stopped_revisions AS ( "
					+ " 	SELECT mr.id, MAX(mr.rev) AS rev "
					+ " 	FROM raeicei.eid_manager_aud mr "
					+ " 	WHERE mr.manager_status IN ('STOP') "
					+ " 	GROUP BY mr.id "
					+ " ), "
					+ " target_revisions AS ( "
					+ " 	SELECT mr.id, MAX(mr.rev) AS rev "
					+ " 	FROM raeicei.eid_manager_aud mr "
					+ " 	LEFT JOIN (SELECT * FROM stopped_revisions) st ON st.id = mr.id "
					+ " 	WHERE mr.manager_status IN ('ACTIVE', 'SUSPENDED') AND (st.rev IS NULL OR mr.rev > st.rev) "
					+ " 	GROUP BY mr.id "
					+ " ) "
					+ " 	SELECT a.id, a.name, a.name_latin AS nameLatin, a.eik_number AS eikNumber, a.logo_url as logoUrl, a.home_page AS homePage, a.manager_status AS managerStatus, a.service_type as serviceType "
					+ " 	FROM raeicei.eid_manager_aud a "
					+ " 	RIGHT JOIN (SELECT * FROM target_revisions) AS rev ON rev.id = a.id AND rev.rev = a.rev "
					+ " 	WHERE a.service_type = 'EID_ADMINISTRATOR' AND a.manager_status IN ('ACTIVE', 'SUSPENDED') "
					+ " 	GROUP BY a.id, a.name, a.name_latin, a.eik_number, a.logo_url, a.home_page, a.manager_status, a.service_type ")
	List<EidAdministratorView> getAllActiveOrSuspendedEidAdministratorsFromAuditByQuery();

	@Query(nativeQuery = true,
			value = " SELECT a.id, a.name, a.name_latin AS nameLatin, a.eik_number AS eikNumber, a.logo_url as logoUrl, a.home_page AS homePage, a.manager_status AS managerStatus, a.service_type as serviceType "
					+ " FROM raeicei.eid_manager a "
					+ " WHERE a.service_type = 'EID_ADMINISTRATOR' ")
	List<EidAdministratorView> getAllEidAdministratorsHistoryByQuery();

	@Query(nativeQuery = true,
			value = "select a.id, a.name, a.name_latin as nameLatin, a.eik_number as eikNumber, a.address, a.email, a.service_type as serviceType, "
					+ " a.logo_url as logoUrl, a.download_url as downloadUrl, a.home_page as homePage, a.manager_status as managerStatus, a.code, a.create_date as createDate, "
					+ " ARRAY_AGG(DISTINCT fo.id) FILTER (WHERE fo.id is not null)as eidManagerFrontOfficeIds, "
					+ " json_agg(DISTINCT jsonb_strip_nulls(jsonb_build_object('id', fo.id,'name', fo.name, 'region',fo.region))) as eidFrontOffices, "
					+ " ARRAY_AGG(DISTINCT d.id) as deviceIds "
					+ " from raeicei.eid_manager a "
					+ " left join raeicei.eid_manager_offices fm on a.id = fm.eid_manager_id "
					+ " left join raeicei.front_office fo on fm.office_id = fo.id "
					+ " left join raeicei.eid_administrator_device fd on a.id = fd.eid_administrator_id "
					+ " left join raeicei.devices d on fd.device_id = d.id "
					+ " where a.service_type = 'EID_ADMINISTRATOR' and a.manager_status = :eidManagerStatus "
					+ " group by a.id order by a.create_date desc")
	List<EidAdministratorView> getAllEidAdministratorsByStatus(@Param("eidManagerStatus") String eidManagerStatus);

    @Query(nativeQuery = true,
            value = " select a.id, a.name, a.name_latin as nameLatin, a.eik_number as eikNumber, a.address, a.logo_url as logoUrl, a.download_url as downloadUrl, a.home_page as homePage, a.manager_status as managerStatus, a.code,ARRAY_AGG(DISTINCT fo.id) as eidManagerFrontOfficeIds,  ARRAY_AGG(DISTINCT d.id) as deviceIds " +
                    " from raeicei.eid_manager a " +
            		"        left join raeicei.eid_manager_offices fm on a.id = fm.eid_manager_id " +
            		"        left join raeicei.front_office fo on fm.office_id = fo.id " +
                    " left join raeicei.eid_administrator_device fd on a.id = fd.eid_administrator_id " +
                    " left join raeicei.devices d on fd.device_id = d.id " +
                    " where a.id = :eidManagerId AND a.service_type = 'EID_ADMINISTRATOR' " +
                    " group by a.id, a.name, a.eik_number ")
    Optional<EidAdministratorView> getEidAdministratorByIdWithQuery(@Param("eidManagerId") UUID eidManagerId);


    @Query(nativeQuery = true, value = " select a.id, a.name, a.name_latin as nameLatin, a.eik_number as eikNumber, a.address, "
					+ "        a.logo_url as logoUrl, a.download_url as downloadUrl, a.home_page as homePage, a.manager_status as managerStatus, a.code, "
					+ "        ARRAY_AGG(DISTINCT fo.id) FILTER (WHERE fo.id is not null)as eidManagerFrontOfficeIds, "
					+ "        json_agg(DISTINCT jsonb_strip_nulls(jsonb_build_object('id', fo.id,'name', fo.name, 'region',fo.region))) as eidFrontOffices, "
					+ "        ARRAY_AGG(DISTINCT d.id) as deviceIds, "
					+ "        json_agg(DISTINCT jsonb_strip_nulls(jsonb_build_object('id', d.id, 'type', d.type, 'name', d.name, 'description', d.description, "
					+ "        'authorizationLink', d.authorization_link, 'backchannelAuthorizationLink', d.backchannel_authorization_link, 'isActive', d.is_active))) as devices "
					+ "                from raeicei.eid_manager a "
					+ "                left join raeicei.eid_manager_offices fm on a.id = fm.eid_manager_id "
					+ "                left join raeicei.front_office fo on fm.office_id = fo.id "
					+ "                left join raeicei.eid_administrator_device fd on a.id = fd.eid_administrator_id "
					+ "                left join raeicei.devices d on fd.device_id = d.id "
					+ "		   where a.id = :eidManagerId AND a.service_type = 'EID_ADMINISTRATOR'"
					+ "        group by a.id")
    Optional<EidAdministratorView> getEidAdministratorById(@Param("eidManagerId") UUID eidManagerId);

	@Query(nativeQuery = true, value = " SELECT a.id, a.name, a.name_latin AS nameLatin, a.eik_number AS eikNumber, a.address, a.email, "
			+ " a.logo_url as logoUrl, a.download_url as downloadUrl, a.home_page AS homePage, a.manager_status AS managerStatus, a.code, a.service_type AS serviceType, "
			+ " ARRAY_AGG(DISTINCT fo.id) FILTER (WHERE fo.id is not null) AS eidManagerFrontOfficeIds, "
			+ " ARRAY_AGG(DISTINCT d.id) AS deviceIds, "
			+ " ARRAY_AGG(DISTINCT e.id) AS employeeIds, "
			+ " ARRAY_AGG(DISTINCT doc.id) AS attachmentIds, "
			+ " ARRAY_AGG(DISTINCT n.id) AS noteIds, "
			+ " ARRAY_AGG(DISTINCT ps.id) AS providedServiceIds, "
			+ " json_agg(DISTINCT jsonb_strip_nulls(jsonb_build_object('id', c.id, 'name', c.name, 'nameLatin', c.name_latin, 'isActive', c.is_active, 'phoneNumber', c.phone_number, 'email', c.email, "
			+ " 'citizenIdentifierType', c.citizen_identifier_type, 'citizenIdentifierNumber', c.citizen_identifier_number, 'eIdentity', c.e_identity))) AS authorizedPersons "
			+ " FROM raeicei.eid_manager a "
			+ " LEFT JOIN raeicei.eid_manager_offices fm ON a.id = fm.eid_manager_id "
			+ " LEFT JOIN raeicei.front_office fo ON fm.office_id = fo.id "
			+ " LEFT JOIN raeicei.eid_administrator_device fd ON a.id = fd.eid_administrator_id "
			+ " LEFT JOIN raeicei.devices d ON fd.device_id = d.id "
			+ " LEFT JOIN raeicei.eid_manager_authorized_persons ea ON a.id = ea.eid_manager_id "
			+ " LEFT JOIN raeicei.contact c ON ea.authorized_person_id = c.id "
			+ " LEFT JOIN raeicei.eid_manager_employees ee ON a.id = ee.manager_id "
			+ " LEFT JOIN raeicei.employee e ON ee.employee_id = e.id "
			+ " LEFT JOIN raeicei.eid_manager_attachments ma ON a.id = ma.eid_manager_id "
			+ " LEFT JOIN raeicei.documents doc ON ma.atachment_id = doc.id "
			+ " LEFT JOIN raeicei.eid_manager_notes en ON a.id = en.eid_manager_id "
			+ " LEFT JOIN raeicei.notes n ON en.note_id = n.id "
			+ " LEFT JOIN raeicei.eid_manager_services es ON a.id = es.eid_manager_id "
			+ " LEFT JOIN raeicei.provided_service ps ON es.service_id = ps.id "
			+ " WHERE a.id = :eidManagerId AND a.service_type = 'EID_ADMINISTRATOR' "
			+ " GROUP BY a.id ")
	Optional<EidAdministratorAuthorizedView> getEidAdministratorAuthorizedById(@Param("eidManagerId") UUID eidManagerId);

//    @Query(value = "SELECT em FROM EidManager em WHERE TYPE(em) = EidCenter")
//    List<EidCenter> findAllEidCenters();

  @Query(value = "SELECT em FROM EidManager em WHERE TYPE(em) = EidCenter AND em.id = :eidManagerId")
  Optional<EidCenter> findEidCenterById(@Param("eidManagerId") UUID eidManagerId);

    @Query(nativeQuery = true,
            value = " select a.id, a.name, a.name_latin as nameLatin, a.eik_number as eikNumber, a.address, a.logo_url as logoUrl, a.home_page as homePage, a.manager_status as managerStatus, a.client_id as clientId, a.client_secret as clientSecret, ARRAY_AGG(DISTINCT fo.id) as eidManagerFrontOfficeIds" +
                    " from raeicei.eid_manager a " +
            		"        left join raeicei.eid_manager_offices fm on a.id = fm.eid_manager_id " +
            		"        left join raeicei.front_office fo on fm.office_id = fo.id " +
                    " left join raeicei.eid_administrator_device fd on a.id = fd.eid_administrator_id " +
                    " where a.service_type = 'EID_CENTER' and a.manager_status = 'ACTIVE' " +
                    " group by a.id, a.name, a.eik_number ")
    List<EidCenterView> getAllEidCentersByQuery();

	@Query(nativeQuery = true,
			value = " WITH " +
					" stopped_revisions AS ( " +
					" 	SELECT mr.id, MAX(mr.rev) AS rev " +
					" 	FROM raeicei.eid_manager_aud mr " +
					" 	WHERE mr.manager_status IN ('STOP', 'SUSPENDED') " +
					" 	GROUP BY mr.id " +
					" ), " +
					" 	target_revisions AS ( " +
					" 	SELECT mr.id, MAX(mr.rev) AS rev " +
					" 	FROM raeicei.eid_manager_aud mr " +
					" 	LEFT JOIN (SELECT * FROM stopped_revisions) st ON st.id = mr.id " +
					" 	WHERE mr.manager_status = 'ACTIVE' AND (st.rev IS NULL OR mr.rev > st.rev) " +
					" 	GROUP BY mr.id " +
					" ) " +
					" 	SELECT a.id, a.name, a.name_latin AS nameLatin, a.eik_number AS eikNumber, a.address, a.email, a.logo_url as logoUrl, a.home_page AS homePage, " +
					" 	a.code, a.manager_status AS managerStatus, a.client_id AS clientId, a.client_secret AS clientSecret, a.service_type as serviceType, " +
					" 	ARRAY_AGG(DISTINCT fo.id) AS eidManagerFrontOfficeIds, " +
					" 	json_agg(DISTINCT jsonb_strip_nulls(jsonb_build_object('id', fo.id,'name', fo.name, 'region',fo.region))) AS eidFrontOffices " +
					" 	FROM raeicei.eid_manager_aud a " +
					" 	RIGHT JOIN (SELECT * FROM target_revisions) AS rev ON rev.id = a.id " +
					" 	LEFT JOIN raeicei.eid_manager_offices fm ON a.id = fm.eid_manager_id " +
					" 	LEFT JOIN raeicei.front_office fo ON fm.office_id = fo.id " +
					" 	WHERE a.service_type = 'EID_CENTER' AND a.manager_status = 'ACTIVE' " +
					" 	GROUP BY a.id, a.name, a.name_latin, a.eik_number, a.address, a.email, a.logo_url, a.home_page, a.manager_status, a.code, a.client_id, a.client_secret, a.service_type ")
	List<EidCenterView> getAllActiveEidCentersFromAuditByQuery();

	@Query(nativeQuery = true,
			value = "   WITH "
					+ " stopped_revisions AS ( "
					+ " 	SELECT mr.id, MAX(mr.rev) AS rev "
					+ " 	FROM raeicei.eid_manager_aud mr "
					+ " 	WHERE mr.manager_status IN ('STOP', 'SUSPENDED') "
					+ " 	GROUP BY mr.id "
					+ " ), "
					+ " target_revisions AS ( "
					+ " 	SELECT mr.id, MAX(mr.rev) AS rev "
					+ " 	FROM raeicei.eid_manager_aud mr "
					+ " 	LEFT JOIN (SELECT * FROM stopped_revisions) st ON st.id = mr.id "
					+ " 	WHERE mr.manager_status = 'ACTIVE' AND (st.rev IS NULL OR mr.rev > st.rev) "
					+ " 	GROUP BY mr.id "
					+ " ) "
					+ " 	SELECT a.name, a.eik_number AS eikNumber, a.address, a.email, a.home_page AS homePage, fo.description as frontOfficeInfo, "
					+ " 	fo.region as frontOfficeRegion, fo.latitude as frontOfficeLatitude, fo.longitude as frontOfficeLongitude, "
					+ " 	fo.location as frontOfficeLocation, fo.contact as FrontOfficePhoneNumber, fo.working_hours as frontOfficeWorkingHours "
					+ " 	FROM raeicei.eid_manager_aud a "
					+ " 	RIGHT JOIN (SELECT * FROM target_revisions) AS rev ON rev.id = a.id AND rev.rev = a.rev "
					+ " 	LEFT JOIN raeicei.eid_manager_offices fm ON a.id = fm.eid_manager_id "
					+ " 	LEFT JOIN raeicei.front_office fo ON fm.office_id = fo.id "
					+ " 	WHERE a.service_type = 'EID_CENTER' AND a.manager_status = 'ACTIVE' AND fo.is_active = 'TRUE' ")
	List<ReportOfEidManagers> getEidCentersReport();

	@Query(nativeQuery = true,
			value = " WITH " +
					" stopped_revisions AS ( " +
					" 	SELECT mr.id, MAX(mr.rev) AS rev " +
					" 	FROM raeicei.eid_manager_aud mr " +
					" 	WHERE mr.manager_status IN ('STOP') " +
					" 	GROUP BY mr.id " +
					" ), " +
					" 	target_revisions AS ( " +
					" 	SELECT mr.id, MAX(mr.rev) AS rev " +
					" 	FROM raeicei.eid_manager_aud mr " +
					" 	LEFT JOIN (SELECT * FROM stopped_revisions) st ON st.id = mr.id " +
					" 	WHERE mr.manager_status IN ('ACTIVE', 'SUSPENDED') AND (st.rev IS NULL OR mr.rev > st.rev) " +
					" 	GROUP BY mr.id " +
					" ) " +
					" 	SELECT a.id, a.name, a.name_latin AS nameLatin, a.eik_number AS eikNumber, a.logo_url as logoUrl, a.home_page AS homePage, a.manager_status AS managerStatus, a.service_type as serviceType " +
					" 	FROM raeicei.eid_manager_aud a " +
					" 	RIGHT JOIN (SELECT * FROM target_revisions) AS rev ON rev.id = a.id AND rev.rev = a.rev " +
					" 	WHERE a.service_type = 'EID_CENTER' AND a.manager_status IN ('ACTIVE', 'SUSPENDED') " +
					" 	GROUP BY a.id, a.name, a.name_latin, a.eik_number, a.logo_url, a.home_page, a.manager_status, a.service_type ")
	List<EidCenterView> getAllActiveOrSuspendedEidCentersFromAuditByQuery();

	@Query(nativeQuery = true,
			value = " SELECT a.id, a.name, a.name_latin AS nameLatin, a.eik_number AS eikNumber, a.logo_url as logoUrl, a.home_page AS homePage, a.manager_status AS managerStatus, a.service_type as serviceType "
					+ " FROM raeicei.eid_manager a "
					+ " WHERE a.service_type = 'EID_CENTER' ")
	List<EidCenterView> getAllEidCentersHistoryByQuery();

	@Query(nativeQuery = true,
			value = " select a.id, a.name, a.name_latin as nameLatin, a.eik_number as eikNumber, a.address, a.email, a.logo_url as logoUrl, a.home_page as homePage, " +
					" a.code, a.create_date as createDate, a.manager_status as managerStatus, a.client_id as clientId, a.client_secret as clientSecret, a.service_type as serviceType, " +
					" ARRAY_AGG(DISTINCT fo.id) FILTER (WHERE fo.id is not null) as eidManagerFrontOfficeIds, " +
					" json_agg(DISTINCT jsonb_strip_nulls(jsonb_build_object('id', fo.id,'name', fo.name, 'region',fo.region))) as eidFrontOffices " +
					" from raeicei.eid_manager a " +
					" left join raeicei.eid_manager_offices fm on a.id = fm.eid_manager_id " +
					" left join raeicei.front_office fo on fm.office_id = fo.id " +
					" left join raeicei.eid_administrator_device fd on a.id = fd.eid_administrator_id " +
					" where a.service_type = 'EID_CENTER' and a.manager_status = :eidManagerStatus " +
					" group by a.id, a.name, a.eik_number order by a.create_date desc ")
	List<EidCenterView> getAllEidCentersByStatus(@Param("eidManagerStatus") String eidManagerStatus);

    @Query(nativeQuery = true,
            value = " select a.id, a.name, a.name_latin as nameLatin, a.eik_number as eikNumber, a.address, a.logo_url as logoUrl, a.home_page as homePage, a.manager_status as managerStatus, a.client_id as clientId, a.client_secret as clientSecret, ARRAY_AGG(DISTINCT fo.id) as eidManagerFrontOfficeIds " +
                    " from raeicei.eid_manager a " +
            		"        left join raeicei.eid_manager_offices fm on a.id = fm.eid_manager_id " +
            		"        left join raeicei.front_office fo on fm.office_id = fo.id " +
                    " left join raeicei.eid_administrator_device fd on a.id = fd.eid_administrator_id " +
                    " where a.id = :eidManagerId AND a.service_type = 'EID_CENTER' " +
                    " group by a.id, a.name, a.eik_number ")
    Optional<EidCenterView> getEidCenterByIdWithQuery(@Param("eidManagerId") UUID eidManagerId);

	@Query(nativeQuery = true, value = " SELECT a.id, a.name, a.name_latin AS nameLatin, a.eik_number AS eikNumber, a.address, a.email, "
			+ " a.logo_url as logoUrl, a.home_page AS homePage, a.manager_status AS managerStatus, a.code, a.service_type AS serviceType, a.client_id AS clientId, a.client_secret AS clientSecret, "
			+ " ARRAY_AGG(DISTINCT fo.id) FILTER (WHERE fo.id is not null) AS eidManagerFrontOfficeIds, "
			+ " ARRAY_AGG(DISTINCT e.id) AS employeeIds, "
			+ " ARRAY_AGG(DISTINCT doc.id) AS attachmentIds, "
			+ " ARRAY_AGG(DISTINCT n.id) AS noteIds, "
			+ " ARRAY_AGG(DISTINCT ps.id) AS providedServiceIds, "
			+ " json_agg(DISTINCT jsonb_strip_nulls(jsonb_build_object('id', c.id, 'name', c.name, 'nameLatin', c.name_latin, 'isActive', c.is_active, 'phoneNumber', c.phone_number, 'email', c.email, "
			+ " 'citizenIdentifierType', c.citizen_identifier_type, 'citizenIdentifierNumber', c.citizen_identifier_number, 'eIdentity', c.e_identity))) AS authorizedPersons "
			+ " FROM raeicei.eid_manager a "
			+ " LEFT JOIN raeicei.eid_manager_offices fm ON a.id = fm.eid_manager_id "
			+ " LEFT JOIN raeicei.front_office fo ON fm.office_id = fo.id "
			+ " LEFT JOIN raeicei.eid_manager_authorized_persons ea ON a.id = ea.eid_manager_id "
			+ " LEFT JOIN raeicei.contact c ON ea.authorized_person_id = c.id "
			+ " LEFT JOIN raeicei.eid_manager_employees ee ON a.id = ee.manager_id "
			+ " LEFT JOIN raeicei.employee e ON ee.employee_id = e.id "
			+ " LEFT JOIN raeicei.eid_manager_attachments ma ON a.id = ma.eid_manager_id "
			+ " LEFT JOIN raeicei.documents doc ON ma.atachment_id = doc.id "
			+ " LEFT JOIN raeicei.eid_manager_notes en ON a.id = en.eid_manager_id "
			+ " LEFT JOIN raeicei.notes n ON en.note_id = n.id "
			+ " LEFT JOIN raeicei.eid_manager_services es ON a.id = es.eid_manager_id "
			+ " LEFT JOIN raeicei.provided_service ps ON es.service_id = ps.id "
			+ " WHERE a.id = :eidManagerId AND a.service_type = 'EID_CENTER' "
			+ " GROUP BY a.id ")
	Optional<EidCenterAuthorizedView> getEidCenterAuthorizedById(@Param("eidManagerId") UUID eidManagerId);

    List<EidAdministratorView> findAllEidAdministrators();

	Optional<EidManager> findByEikNumber(@Param("eikNumber") String eikNumber);

	Optional<EidManager> findByEikNumberAndServiceType(String eikNumber, ManagerType serviceType);

	Boolean existsByCode(String code);
}
