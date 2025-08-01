package bg.bulsi.mvr.mpozei.contract.dto;

import java.io.Serializable;
import java.time.OffsetDateTime;

import lombok.Data;

/**
 * Represents a personal ID change request with old and new identification details.
 * <p>
 * This class can be used to deserialize JSON input containing details of a UID type change,
 * such as from a LNC to an EGN, along with relevant timestamps.
 * </p>
 *
 * Example JSON:
 * <pre>
 * {
 *   "oldPersonalId": "2345678901",
 *   "oldUidType": "LNCh",
 *   "newPersonalId": "7111106259",
 *   "newUidType": "EGN",
 *   "date": "2024-10-10T21:00:00Z",
 *   "createdOn": "2024-11-04T09:25:36.851821Z"
 * }
 * </pre>
 */
@Data
public class PivrIdchangesResponseDto implements Serializable {

	/**
	 * 
	 */
	private static final long serialVersionUID = 246911613078476675L;

	private String oldPersonalId;

	private IdentifierType oldUidType;

	private String newPersonalId;

	private IdentifierType newUidType;

	private OffsetDateTime date;

	private OffsetDateTime createdOn;

}
