package bg.bulsi.mvr.iscei.contract.dto;

import com.fasterxml.jackson.annotation.JsonValue;

public interface ReportRequestsCountDto {
	String getClientId();
	
	String getSystemId();

	String getSystemName();

	String getTotalRequests();
	
    public default String[] toArray() {
        return new String[] { 
        		getClientId(),
        		getSystemId(),
        		getSystemName(),
        		getTotalRequests(),
        		};
    }
}
