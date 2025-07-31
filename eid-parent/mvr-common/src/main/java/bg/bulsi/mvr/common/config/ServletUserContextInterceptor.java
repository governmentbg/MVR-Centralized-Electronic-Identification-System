package bg.bulsi.mvr.common.config;

import bg.bulsi.mvr.common.config.security.UserContext;
import bg.bulsi.mvr.common.config.security.UserContextHolder;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import org.springframework.boot.autoconfigure.condition.ConditionalOnWebApplication;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.security.oauth2.server.resource.authentication.JwtAuthenticationToken;
import org.springframework.stereotype.Component;
import org.springframework.web.servlet.HandlerInterceptor;

import java.util.Base64;
import java.util.Objects;

/*
    If a header with name user-context is found within the request,
    the filter builds the UserContext object from the header.
    If there is no header found in the request it generates
    new UserContext with unique globalCorrelationId

    For Servlet applications
*/
@ConditionalOnWebApplication(type = ConditionalOnWebApplication.Type.SERVLET)
public class ServletUserContextInterceptor implements HandlerInterceptor {
    @Override
    public boolean preHandle(HttpServletRequest request, HttpServletResponse response, Object handler) throws Exception {
        UserContext userContext;
        String userContextJson = request.getHeader(UserContext.USER_CONTEXT_KEY);
        if (Objects.nonNull(userContextJson)) {
            userContext = UserContextHolder.fromJson(
            		new String(Base64.getDecoder().decode(userContextJson.getBytes()))
            		);
        } else {
        	if(SecurityContextHolder.getContext().getAuthentication() instanceof JwtAuthenticationToken) {
                userContext = new UserContext(SecurityContextHolder.getContext().getAuthentication());
        	} else {
        		userContext = UserContextHolder.emptyServletContext();
        	}
        }
        UserContextHolder.setToServletContext(userContext);
        return true;
    }
}
