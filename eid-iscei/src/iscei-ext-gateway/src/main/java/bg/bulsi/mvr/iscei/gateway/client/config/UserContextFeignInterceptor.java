//package bg.bulsi.mvr.iscei.gateway.client.config;
//
//import bg.bulsi.mvr.common.config.security.UserContext;
//import bg.bulsi.mvr.common.config.security.UserContextHolder;
//
//import java.io.ByteArrayInputStream;
//import java.io.IOException;
//import java.io.ObjectInputStream;
//import java.util.UUID;
//
//import com.fasterxml.jackson.databind.ObjectMapper;
//import com.fasterxml.jackson.datatype.jsr310.JavaTimeModule;
//
//import feign.InvocationContext;
//import feign.RequestInterceptor;
//import feign.RequestTemplate;
//import feign.ResponseInterceptor;
//
//public class UserContextFeignInterceptor implements RequestInterceptor, ResponseInterceptor {
//	@Override
//	public void apply(RequestTemplate template) {
//		UserContext context = new UserContext(null, null, null, null, null, null, null, null, null, UUID.randomUUID(),
//				null, null, null, null);
//
//		template.header(UserContext.USER_CONTEXT_KEY, UserContextHolder.toJson(context));
//	}
//
//	@Override
//	public Object aroundDecode(InvocationContext invocationContext) throws IOException {
//		var newUserContext = UserContextHolder.fromJson(
//				invocationContext.response().headers().get(UserContext.USER_CONTEXT_KEY).toArray()[0].toString());
//
//		UserContextHolder.setToServletContext(newUserContext);
//		
//		return invocationContext.proceed();
//	}
//}
