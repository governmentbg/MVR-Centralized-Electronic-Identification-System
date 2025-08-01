package bg.bulsi.mvr.iscei.model;

import java.time.OffsetDateTime;
import java.util.UUID;

import org.springframework.data.redis.core.RedisHash;
import org.springframework.data.redis.core.TimeToLive;
import org.springframework.data.redis.core.index.Indexed;

import bg.bulsi.mvr.iscei.contract.dto.ExternalProviderLoginResultDto;
import bg.bulsi.mvr.iscei.contract.dto.LevelOfAssurance;
import bg.bulsi.mvr.iscei.contract.dto.RequestFromDto;
import jakarta.persistence.Id;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import lombok.ToString;

@AllArgsConstructor
@NoArgsConstructor
@Getter
@Setter
@RedisHash("AuthApprovalRequest")
@ToString
public class AuthApprovalRequest {

	@Id
	private UUID id;
	
	//CitizenProfile.id 
	@Indexed
	private String username;
	
	private LevelOfAssurance levelOfAssurance;
	
	private String relyPartyToken;
	
	private RequestFromDto requestFrom;
	
	@Indexed
	private String bindingMessage;
	
	@TimeToLive 
	private long maxTtl;
	
	private OffsetDateTime createDate;
	
	private ExternalProviderLoginResultDto externalProviderLoginResult;
	
	//private Long creationTime = Instant.now().getEpochSecond();
	//private boolean relyPartyCallbackCalled;
}
