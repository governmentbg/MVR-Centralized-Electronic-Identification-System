package bg.bulsi.mvr.iscei.gateway.service;

import java.util.UUID;

import org.apache.commons.lang3.StringUtils;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.http.HttpStatus;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import bg.bulsi.mvr.common.exception.EntityNotFoundException;
import bg.bulsi.mvr.common.exception.ErrorCode;
import bg.bulsi.mvr.common.exception.ValidationMVRException;
import bg.bulsi.mvr.iscei.contract.dto.VerifyOtpDto;
import bg.bulsi.mvr.iscei.model.OtpToken;
import bg.bulsi.mvr.iscei.model.repository.keypair.OtpTokenRepository;
import bg.bulsi.mvr.pan_client.DirectEmailRequest;
import bg.bulsi.mvr.pan_client.NotificationSender;

@Service
public class OtpTokenService {
	
	@Autowired
	private OtpGenerator otpGenerator;
	
	@Autowired
	private OtpTokenRepository otpRepository;
	
	@Autowired
	private NotificationSender notificationSender;
	
	@Value("${mvr.2fa.otp.ttl:null}")
	private Long otpTtl;
	
	@Transactional
	public OtpToken createOtpToken(String email, String clientId ,Object authServerResp) {
		String otpCode = this.otpGenerator.generateToken();
		
		OtpToken otpToken = new OtpToken();
		otpToken.setOtpCode(otpCode);
		otpToken.setAuthServerResp(authServerResp);
		otpToken.setEmail(email);
		otpToken.setClientId(clientId);
		
		if(otpTtl != null) {
			otpToken.setTtl(otpTtl);
		}
		
		this.otpRepository.save(otpToken);
		
		return otpToken; 
	}
	
	public OtpToken getOtpToken(UUID id) {
		return this.otpRepository
				.findById(id)
			  	.orElseThrow(() -> new EntityNotFoundException(ErrorCode.OTP_CODE_DOES_NOT_EXIST));
	}
	
	@Transactional
	public OtpToken verifyOtp(VerifyOtpDto verifyOtpDto) {
		OtpToken otpToken = this.otpRepository
		.findById(verifyOtpDto.getSessionId())
	  	.orElseThrow(() -> new EntityNotFoundException(ErrorCode.SESSION_IS_NOT_VALID));
		
	    // Increment the attempts counter
		otpToken.setAttemptsNumber(otpToken.getAttemptsNumber() + 1);
		
	    // Check if the maximum number of attempts is exceeded and delete OTP if so
		if(otpToken.getAttemptsNumber() > OtpToken.MAX_NUMBER_OF_ATTEMPTS) {
			this.otpRepository.deleteById(otpToken.getId());
			throw new ValidationMVRException(ErrorCode.OTP_CODE_MAX_ATTEMPTS_REACHED, HttpStatus.FORBIDDEN);
		}
		
	    // Validate the OTP code
		if(!StringUtils.equals(otpToken.getOtpCode(), verifyOtpDto.getOtp())) {
			this.otpRepository.save(otpToken);
			
			throw new ValidationMVRException(ErrorCode.OTP_CODE_IS_NOT_VALID);
		}

	    // Successfully verified OTP, delete the OTP token
		this.otpRepository.deleteById(otpToken.getId());
		
		return otpToken;
	}
	
//	@Transactional
//	public OtpToken generateOtp(GenerateOtpDto generateOtp) {
//		OtpToken otpToken = this.otpRepository
//		.findById(generateOtp.getSessionId())
//	  	.orElseThrow(() -> new ValidationMVRException(ErrorCode.OTP_CODE_IS_NOT_VALID));
//		
//		if(otpToken.getTokenRegenerations() + 1 >= OtpToken.DEFAULT_MAX_TOKEN_REGENERATIONS) {
//			throw new ValidationMVRException(ErrorCode.OTP_CODE_IS_NOT_VALID);
//		}
//		
//		otpToken.setOtpCode(this.otpGenerator.generateToken());
//		otpToken.setTokenRegenerations(otpToken.getTokenRegenerations() + 1);
//		otpToken.setTtl(OtpToken.DEFAULT_TTL);
//		
//		return this.otpRepository.save(otpToken);
//	}
	
	public void sendOtpToEmail(OtpToken otpToken) {
		this.notificationSender.sendDirectEmail(new DirectEmailRequest("bg", "Валидационнен код", "Вашият валидационнен код е: " + otpToken.getOtpCode(), otpToken.getEmail()));
	}
}
