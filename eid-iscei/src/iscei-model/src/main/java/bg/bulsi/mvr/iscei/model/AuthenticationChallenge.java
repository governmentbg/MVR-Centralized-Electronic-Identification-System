package bg.bulsi.mvr.iscei.model;

import java.util.UUID;

import org.springframework.data.redis.core.RedisHash;
import org.springframework.data.redis.core.TimeToLive;

import bg.bulsi.mvr.iscei.contract.dto.LevelOfAssurance;
import jakarta.persistence.Id;
import lombok.Data;

/**
 * Initial request for authentication from the frontend
 */
@Data
@RedisHash("AuthenticationChallenge")
public class AuthenticationChallenge {

	@Id
	private UUID id;
	
	@TimeToLive 
	private Long expiration;
	
	//example: НАП, портал за граждани, ЕАФТ
	private String requestFrom;
	private LevelOfAssurance levelOfAssurance;
}
