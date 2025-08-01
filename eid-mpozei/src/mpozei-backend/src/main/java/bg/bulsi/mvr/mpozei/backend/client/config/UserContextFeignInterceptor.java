package bg.bulsi.mvr.mpozei.backend.client.config;

import bg.bulsi.mvr.common.config.security.UserContext;
import bg.bulsi.mvr.common.config.security.UserContextHolder;
import java.io.IOException;
import java.util.Base64;
import java.util.Collection;

import feign.InvocationContext;
import feign.RequestInterceptor;
import feign.RequestTemplate;
import feign.ResponseInterceptor;
import lombok.extern.slf4j.Slf4j;

@Slf4j
public class UserContextFeignInterceptor implements RequestInterceptor, ResponseInterceptor {
	@Override
	public void apply(RequestTemplate template) {
		String userContext = UserContextHolder.toJson();
		template.header(UserContext.USER_CONTEXT_KEY, Base64.getEncoder().encodeToString(userContext.getBytes()));
	}

	@Override
	public Object intercept(InvocationContext invocationContext, Chain chain) throws Exception {
		Collection<String> header = invocationContext.response().headers().get(UserContext.USER_CONTEXT_KEY);
		if (header == null) {
			log.info("UserContext cannot be found in response");
			return invocationContext.proceed();
		}
		String userContext = new String(Base64.getDecoder().decode(header.toArray()[0].toString()));

		var newUserContext = UserContextHolder.fromJson(userContext);
		UserContextHolder.setToServletContext(newUserContext);

		return invocationContext.proceed();
	}
}
