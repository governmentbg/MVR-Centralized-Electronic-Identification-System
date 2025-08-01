package bg.bulsi.mvr.mpozei.model.pivr;

import java.util.Map;

import lombok.Data;

@Data
public class RegiXResult {
	
	private Map<String, Object> response;
	
	private boolean hasFailed;
	
	private String error;
	
//	@Data
//	//@JsonRootName("response")
//	public static class ResponseWrapper <T>{
//		
//		private T identityInfo;
//	}
}
