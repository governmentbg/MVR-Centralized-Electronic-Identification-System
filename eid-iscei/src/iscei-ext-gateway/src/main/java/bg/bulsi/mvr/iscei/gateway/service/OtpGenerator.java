package bg.bulsi.mvr.iscei.gateway.service;

import java.security.SecureRandom;

import org.springframework.stereotype.Component;

@Component
public class OtpGenerator {
	
	private SecureRandom random = new SecureRandom();

	
	public String generateToken() {
        int otp = 10_000_000 + random.nextInt(90_000_000);
        
        return Integer.toString(otp);
	}
}
