package bg.bulsi.mvr.audit_logger.dto;

import java.util.HashMap;
import java.util.Map;

public class EventPayload {
    
    private Map<String, Object> params;
    
    public EventPayload() {
        this.params = new HashMap<>();
    }
    
    public EventPayload(Map<String, Object> params) {
    	this.params = params;
    }
    
    public void setRequestBody(Object requestBody) {
    	this.addAdditionalParam(AuditLoggingKey.REQUEST.getParamName(), requestBody);
    }
    
    public void setRequesterUid(String requesterUid) {
    	this.addAdditionalParam(AuditLoggingKey.REQUESTER_UID.getParamName(), requesterUid);
    }
    
    public void setRequesterUidType(String requesterUidType) {
    	this.addAdditionalParam(AuditLoggingKey.REQUESTER_UID_TYPE.getParamName(),  requesterUidType);
    }
    
    public void setRequesterName(String... names) {
    	this.addAdditionalParam(AuditLoggingKey.REQUESTER_NAME.getParamName(),  String.join(" ", names));
    }
    
    public void setTargetUid(String targetUid) {
    	this.addAdditionalParam(AuditLoggingKey.TARGET_UID.getParamName(),  targetUid);
    }
    
    public void setTargetUidType(String targetUidType) {
    	this.addAdditionalParam(AuditLoggingKey.TARGET_UID_TYPE.getParamName(), targetUidType);
    }
    
    public void setApplicationId(String applicationId) {
    	this.addAdditionalParam(AuditLoggingKey.APPLICATION_ID.getParamName(), applicationId);
    }
    
    public void setApplicationStatus(String applicationStatus) {
    	this.addAdditionalParam(AuditLoggingKey.APPLICATION_STATUS.getParamName(),  applicationStatus);
    }
    
    public void setApplicationType(String applicationType) {
    	this.addAdditionalParam(AuditLoggingKey.APPLICATION_TYPE.getParamName(),  applicationType);
    }
    
    public void setTargetName(String... names) {
    	this.addAdditionalParam(AuditLoggingKey.TARGET_NAME.getParamName(),  String.join(" ", names));
    }
    
    public void setEidentityId(String eidentityId) {
    	this.addAdditionalParam(AuditLoggingKey.TARGET_EIDENTITY_ID.getParamName(),  eidentityId);
    }
    
    public void setCertificateId(String certificateId) {
    	this.addAdditionalParam(AuditLoggingKey.CERTIFICATE_ID.getParamName(),  certificateId);
    }
    
    public void setProfileId(String profileId) {
    	this.addAdditionalParam(AuditLoggingKey.PROFILE_ID.getParamName(),  profileId);
    }
    
    public void addAdditionalParam(String key, Object value) {
    	this.params.put(key, value);
    }

	/**
	 * @return the params
	 */
    public Map<String, Object> getParams() {
		return params;
	}

	/**
	 * @param params the params to set
	 */
	public void setParams(Map<String, Object> params) {
		this.params = params;
	}
}
