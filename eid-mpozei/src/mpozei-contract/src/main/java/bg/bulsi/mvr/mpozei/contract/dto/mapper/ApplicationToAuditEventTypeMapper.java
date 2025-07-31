package bg.bulsi.mvr.mpozei.contract.dto.mapper;

import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationType;

public class ApplicationToAuditEventTypeMapper {

	/**
	 * @param applicationType   - {@link ApplicationType}
	 * @param isInternalGateway - is the request from Public/Citizen or
	 *                          Internal/Admin Gateway
	 * @return
	 */
	public static AuditEventType mapToAuditEventType(ApplicationType applicationType, boolean isInternalGateway) {
		AuditEventType auditEventType = null;
		if (isInternalGateway) {
			auditEventType = switch (applicationType) {
			case ISSUE_EID: {
				yield AuditEventType.CREATE_ISSUE_APPLICATION;
			}
			case RESUME_EID: {
				yield AuditEventType.CREATE_RESUME_APPLICATION;
			}
			case STOP_EID: {
				yield AuditEventType.CREATE_STOP_APPLICATION;
			}
			case REVOKE_EID: {
				yield AuditEventType.CREATE_REVOKE_APPLICATION;
			}
			};
		} else {
			auditEventType = switch (applicationType) {
			case ISSUE_EID: {
				yield AuditEventType.CREATE_ISSUE_ONLINE_APPLICATION;
			}
			case RESUME_EID: {
				yield AuditEventType.CREATE_RESUME_ONLINE_APPLICATION;
			}
			case STOP_EID: {
				yield AuditEventType.CREATE_STOP_ONLINE_APPLICATION;
			}
			case REVOKE_EID: {
				yield AuditEventType.CREATE_REVOKE_ONLINE_APPLICATION;
			}
			};
		}
		return auditEventType;
	}
}
