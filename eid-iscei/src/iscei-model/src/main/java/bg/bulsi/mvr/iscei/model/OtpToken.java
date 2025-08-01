package bg.bulsi.mvr.iscei.model;

import java.util.UUID;

import org.springframework.data.redis.core.RedisHash;
import org.springframework.data.redis.core.TimeToLive;

import jakarta.persistence.Id;
import lombok.Data;

@Data
@RedisHash("OtpToken")
public class OtpToken {
	//How many times can the citizen try entering the otpCode before it is deleted
	public static final long MAX_NUMBER_OF_ATTEMPTS = 3;
	public static final long DEFAULT_TTL = 60l;
	
	@Id
	private UUID id;
	
	@TimeToLive 
	private Long ttl = DEFAULT_TTL;
	
	private String otpCode;
	
	private Object authServerResp;
	
	private String email;
	
	private int attemptsNumber;
	
	private String clientId;
}
