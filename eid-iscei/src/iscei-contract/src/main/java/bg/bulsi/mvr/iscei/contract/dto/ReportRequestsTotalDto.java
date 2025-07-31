package bg.bulsi.mvr.iscei.contract.dto;

import com.fasterxml.jackson.annotation.JsonValue;

public interface ReportRequestsTotalDto {

	String getClientId();
	
	int getMonth();

	long getCount();
	
    @JsonValue
    public default String[] toArray() {
        return new String[] {
        		getClientId(),
        		String.valueOf(getMonth()),
        		String.valueOf(getCount())
        		};
    }
}
