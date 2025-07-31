package bg.bulsi.mvr.raeicei.model.entity;

import bg.bulsi.mvr.raeicei.contract.dto.EidManagerStatus;
import bg.bulsi.mvr.raeicei.model.AbstractAudit;
import bg.bulsi.mvr.raeicei.model.entity.providedservice.ProvidedService;
import bg.bulsi.mvr.raeicei.model.enums.ManagerType;
import jakarta.persistence.*;
import jakarta.validation.constraints.NotEmpty;
import lombok.Getter;
import lombok.Setter;
import lombok.ToString;
import org.hibernate.envers.AuditJoinTable;
import org.hibernate.envers.Audited;
import org.hibernate.envers.NotAudited;
import org.hibernate.envers.RelationTargetAuditMode;

import java.util.ArrayList;
import java.util.List;
import java.util.UUID;

@Getter
@Setter
@ToString
@Audited
@Entity
@Inheritance(strategy = InheritanceType.SINGLE_TABLE)
@DiscriminatorColumn(discriminatorType = DiscriminatorType.STRING, name = "service_type")
@NamedNativeQueries({
	@NamedNativeQuery(name="EidManager.findAllEidAdministrators", 
			query="select a.id, a.name, a.name_latin as nameLatin, a.eik_number as eikNumber, a.address,"
					+ "        a.logo_url as logoUrl, a.download_url as downloadUrl, a.home_page as homePage, a.manager_status as managerStatus, a.code,"
					+ "        ARRAY_AGG(DISTINCT fo.id) FILTER (WHERE fo.id is not null)as eidManagerFrontOfficeIds,"
					+ "        json_agg(DISTINCT jsonb_strip_nulls(jsonb_build_object('id', fo.id,'name', fo.name, 'region',fo.region))) as eidFrontOffices,"
					+ "        ARRAY_AGG(DISTINCT d.id) as deviceIds "
					+ "                from raeicei.eid_manager a "
					+ "                left join raeicei.eid_manager_offices fm on a.id = fm.eid_manager_id"
					+ "                left join raeicei.front_office fo on fm.office_id = fo.id"
					+ "                left join raeicei.eid_administrator_device fd on a.id = fd.eid_administrator_id"
					+ "                left join raeicei.devices d on fd.device_id = d.id where a.service_type = 'EID_ADMINISTRATOR' and a.manager_status = 'ACTIVE'"
					+ "        group by a.id", resultSetMapping="EidManagerViewMapping")   
})
public class EidManager extends AbstractAudit<String> {

    private static final long serialVersionUID = -4375951595438908258L;

                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               @Id
    @Column(name = "id", nullable = false, unique = true, updatable = false)
    private UUID id = UUID.randomUUID();

    @Column(unique = true, nullable = false)
    private String name; // МВР
    
    @Column(unique = true, nullable = false)
    private String nameLatin; // MVR
    
    @Column(name ="eik_number", unique = true, nullable = false)
    private String eikNumber;
    
    @Column(name="code",length = 3, unique = true, nullable = false)
    @NotEmpty
    private String code;
    
    @Column
    private String address;

    @OneToMany(cascade = CascadeType.ALL, fetch = FetchType.LAZY)
    @JoinTable(
            name = "eid_manager_authorized_persons",
            joinColumns = @JoinColumn(name = "eid_manager_id"),
            inverseJoinColumns = @JoinColumn(name = "authorized_person_id"))
    @AuditJoinTable(name = "jt_eid_manager_authorized_persons_aud")
    @OrderBy("lastUpdate DESC")
    private List<Contact> authorizedPersons = new ArrayList<>();
   
    @Column
    private String email;
    
    @Column
	private String homePage;
    
    @Column(name = "service_type", insertable = false, updatable = false)
    @Enumerated(EnumType.STRING)
    private ManagerType serviceType;

    @Column
    @Enumerated(EnumType.STRING)
    private EidManagerStatus managerStatus;
    
    @NotAudited
    @Audited(targetAuditMode = RelationTargetAuditMode.NOT_AUDITED)
    @OneToMany(cascade = CascadeType.ALL)
   // @JoinColumn(name = "manager_id", referencedColumnName = "id")
    @JoinTable(
            name = "eid_manager_services",
            joinColumns = @JoinColumn(name = "eid_manager_id"),
            inverseJoinColumns = @JoinColumn(name = "service_id"))
    @AuditJoinTable(name = "jt_eid_manager_services_aud")
    @OrderBy("lastUpdate DESC")
	private List<ProvidedService> providedServices = new ArrayList<>();
    
    @OneToMany(cascade = CascadeType.ALL, fetch = FetchType.LAZY)
    @JoinTable(
            name = "eid_manager_offices",
            joinColumns = @JoinColumn(name = "eid_manager_id"),
            inverseJoinColumns = @JoinColumn(name = "office_id"))
    @AuditJoinTable(name = "jt_eid_manager_offices_aud")
    @OrderBy("lastUpdate DESC")
    private List<EidManagerFrontOffice> eidManagerFrontOffices = new ArrayList<>();
    
    @OneToMany(cascade = CascadeType.ALL, fetch = FetchType.LAZY)
    @JoinTable(
            name = "eid_manager_employees",
            joinColumns = @JoinColumn(name = "manager_id"),
            inverseJoinColumns = @JoinColumn(name = "employee_id"))
    @AuditJoinTable(name = "jt_eid_manager_employees_aud")
    @OrderBy("lastUpdate DESC")
    private List<Employee> employees = new ArrayList<>();

    @OneToMany(cascade = CascadeType.PERSIST, fetch = FetchType.LAZY)
    @JoinTable(
            name = "eid_manager_attachments",
            joinColumns = @JoinColumn(name = "eid_manager_id"),
            inverseJoinColumns = @JoinColumn(name = "atachment_id"))
    @AuditJoinTable(name = "jt_eid_manager_attachments_aud")
    @OrderBy("lastUpdate DESC")
    private List<Document> attachments = new ArrayList<>();

    @OneToMany(cascade = CascadeType.PERSIST, fetch = FetchType.LAZY)
    @JoinTable(
            name = "eid_manager_notes",
            joinColumns = @JoinColumn(name = "eid_manager_id"),
            inverseJoinColumns = @JoinColumn(name = "note_id"))
    @AuditJoinTable(name = "jt_eid_manager_notes_aud")
    @OrderBy("lastUpdate DESC")
    private List<Note> notes = new ArrayList<>();

    @Column
    private String logoUrl;

//	@Lob
//	private byte[] logo;
}
