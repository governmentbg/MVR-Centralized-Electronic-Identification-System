package bg.bulsi.mvr.audit_logger.dto;

public enum AuditLoggingKey {

	REQUEST("RequestBody", true),
	REQUESTER_UID("RequesterUid", true),
	REQUESTER_UID_TYPE("RequesterUidType", false),
	REQUESTER_NAME("RequesterName", true),
	TARGET_UID("TargetUid", true),
	TARGET_UID_TYPE("TargetUidType", false),
	TARGET_NAME("TargetName", true),
	TARGET_EIDENTITY_ID("TargetEidentityId", false),
	
	APPLICATION_ID("ApplicationId", false),
	APPLICATION_STATUS("ApplicationStatus", false),
	APPLICATION_TYPE("ApplicationType", false),
	
	CERTIFICATE_ID("CertificateId", false),
	
	PROFILE_ID("ProfileId", false);
    ;
    
	private String paramName;
	private boolean requiresEncryption;
	
	AuditLoggingKey(String paramName, boolean requiresEncryption) {
	  this.paramName = paramName;
	  this.requiresEncryption = requiresEncryption;
	}

	/**
	 * @return the paramName
	 */
	public String getParamName() {
		return paramName;
	}

	/**
	 * @return the requiresEncryption
	 */
	public boolean getIsRequiresEncryption() {
		return requiresEncryption;
	}
	
	public static AuditLoggingKey getByParamName(String paramName) {
        for (AuditLoggingKey key : AuditLoggingKey.values()) {
            if (key.getParamName().equalsIgnoreCase(paramName)) {
                return key;
            }
        }
        
		return null;
	}
}
