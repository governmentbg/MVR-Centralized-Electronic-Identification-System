package bg.bulsi.mvr.extgateway.util;

import lombok.RequiredArgsConstructor;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Component;

@Component
@RequiredArgsConstructor
public class PasswordUtil {
	private final PasswordEncoder passwordEncoder;
	
	public String encodePassword(String rawPassword) {
		if(rawPassword != null) {
			return this.passwordEncoder.encode(rawPassword);
		}
		
		return null;
	}
}
