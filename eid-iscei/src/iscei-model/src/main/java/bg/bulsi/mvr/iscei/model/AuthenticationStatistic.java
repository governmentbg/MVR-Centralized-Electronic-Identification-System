package bg.bulsi.mvr.iscei.model;

import java.io.Serializable;
import java.time.OffsetDateTime;
import java.util.UUID;

import jakarta.persistence.Column;
import jakarta.persistence.DiscriminatorColumn;
import jakarta.persistence.DiscriminatorType;
import jakarta.persistence.Entity;
import jakarta.persistence.EnumType;
import jakarta.persistence.Enumerated;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.GenerationType;
import jakarta.persistence.Id;
import jakarta.persistence.Inheritance;
import jakarta.persistence.InheritanceType;
import jakarta.persistence.PrePersist;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@Getter
@Setter
@AllArgsConstructor
@NoArgsConstructor
@Entity
@Inheritance(strategy = InheritanceType.SINGLE_TABLE)
@DiscriminatorColumn(name = "statistic_type", discriminatorType = DiscriminatorType.STRING)
public abstract class AuthenticationStatistic implements Serializable {

	/**
	 * 
	 */
	private static final long serialVersionUID = -476657538465748030L;

	@Id
    @GeneratedValue(strategy = GenerationType.UUID)
    @Column(name = "id", nullable = false, unique = true, updatable = false)
	private UUID id;
	
	@Column
    private UUID citizenProfileId;
	
	@Column
	private String sessionId;

	@Column
	private OffsetDateTime createDate;
	
	@Column
	private String clientId;
	
	@Column
	private String systemId;
	
	@Column
	private String systemName;
	
	@Column
	private String systemType;
	
	@Column
	private Boolean isEmployee;
	
	@Column
	private String requesterIpAddress;
	
    @Column(name = "statistic_type", insertable = false, updatable = false)
   // @Enumerated(EnumType.STRING)
    private String statisticType;
	
	@Column
	private String levelOfAssurance;
    
	@Column
    private UUID deviceId;
    
	@Column
    private UUID eidentityId;
	
	@Column
    private UUID x509CertificateId;
	
	@Column
    private String x509CertificateSn;
	
	@Column
    private String x509CertificateIssuerDn;
	
    @PrePersist
    public void prePersist() {
        if (this.clientId == null) {
        	clientId = "<unknown>";
        }
    }
    
//	public static class StatisticType implements Serializable {
//
//    	/**
//		 * 
//		 */
//		private static final long serialVersionUID = 7235182059410125267L;
//		
//		private StatisticType(){}
//
//        public static final String REQUEST = "REQUEST";
//        public static final String RESULT = "RESULT";
//    }
}
