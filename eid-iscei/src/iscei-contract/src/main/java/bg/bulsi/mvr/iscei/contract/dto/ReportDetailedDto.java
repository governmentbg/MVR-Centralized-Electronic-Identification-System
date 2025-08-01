package bg.bulsi.mvr.iscei.contract.dto;

import java.time.OffsetDateTime;
import java.util.UUID;

import com.fasterxml.jackson.annotation.JsonValue;

public interface ReportDetailedDto {

	OffsetDateTime getCreateDate();

	String getSystemId();

	String getSystemName();

	String getRequesterIpAddress();

	UUID getEidentityId();

	UUID getX509CertificateId();
	
	String getX509CertificateSn();

	String getX509CertificateIssuerDn();
	
	String getLevelOfAssurance();
	
	Boolean getSuccess();
	
    public default String[] toArray() {
        return new String[] { 
        		String.valueOf(getCreateDate()),
        		getSystemId(),
        		getSystemName(),
        		getRequesterIpAddress(),
        		String.valueOf(getEidentityId()).toString(),
        		String.valueOf(getX509CertificateId()).toString(),
        		getX509CertificateSn(),
        		getX509CertificateIssuerDn(),
        		getLevelOfAssurance(),
        		String.valueOf(getSuccess().toString())
        		};
    }
}
