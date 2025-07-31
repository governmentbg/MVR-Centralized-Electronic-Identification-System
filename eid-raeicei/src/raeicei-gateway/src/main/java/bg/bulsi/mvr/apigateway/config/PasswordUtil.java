package bg.bulsi.mvr.apigateway.config;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Component;

@Component
public class PasswordUtil {

	@Autowired
	private PasswordEncoder passwordEncoder;
	
	public String encodePassword(String rawPassword) {
		if(rawPassword != null) {
			return this.passwordEncoder.encode(rawPassword);
		}
		
		return null;
	}
}
