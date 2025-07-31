package bg.bulsi.mvr.pan_client;

import java.util.List;
import java.util.UUID;

import com.fasterxml.jackson.annotation.JsonInclude;

import bg.bulsi.mvr.common.dto.IdentifierType;
import lombok.Data;

/**
 * Basic DTO for PAN Notifications
 * Only 1 of the 3 sets of properties should be used:
 * - eId
 * - userId
 * - uid and uidType
 */
@Data
@JsonInclude(JsonInclude.Include.NON_NULL)
public class Notification {
	
	private String eventCode;
	
	//eidentity_id
	private UUID eId;
	
	//citizen_profile_id
	private UUID userId;
	
	//EGN or LNCh identifier number
	private String uid;
	
	//EGN or LNCh
	private IdentifierType uidType;
	
	private List<Translation> translations;

	record Translation(String language, String message) {}
}
