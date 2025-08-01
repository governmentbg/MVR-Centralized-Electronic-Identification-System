//package bg.bulsi.mvr.iscei.gateway.config;
//
//import org.springframework.security.web.savedrequest.HttpSessionRequestCache;
//import org.springframework.stereotype.Component;
//
//import jakarta.annotation.PostConstruct;
//import jakarta.servlet.http.HttpServletRequest;
//import jakarta.servlet.http.HttpServletResponse;
//import lombok.extern.slf4j.Slf4j;
//
//@Slf4j
//@Component
//public class SpecificRequestCache extends HttpSessionRequestCache {
//
//	@Override
//	public void saveRequest(HttpServletRequest request, HttpServletResponse response) {
//		log.info(".saveRequest; " + request);
//		if (!WebSecurityConfig.v2AuthorizeEndpoint.matches(request)) {
//			return; // Skip caching for other requests
//		}
//		super.saveRequest(request, response);
//	}
//	
//	@PostConstruct
//	private void init() {
//		this.setMatchingRequestParameterName(null);
//	}
//}
