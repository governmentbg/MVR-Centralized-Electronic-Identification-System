package bg.bulsi.mvr.iscei.contract.dto;

import java.util.Map;

import lombok.AllArgsConstructor;
import lombok.Data;

@AllArgsConstructor
@Data
public class RequestFromDto {
	
	private Type type;
	
    private Map<Language, String> system;
	
	enum Type {
		AUTH
	}
	
	enum Language {
		BG,
		EN
	}
}
