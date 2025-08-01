//package bg.bulsi.mvr.iscei.gateway.config;
//
//import java.io.IOException;
//
//import org.springframework.security.core.AuthenticationException;
//import org.springframework.security.web.AuthenticationEntryPoint;
//
//import jakarta.servlet.ServletException;
//import jakarta.servlet.http.HttpServletRequest;
//import jakarta.servlet.http.HttpServletResponse;
//
//public class RedirectAuthenticationEntryPoint implements AuthenticationEntryPoint {
//
//    private final String loginPageUrl;
//
//    public RedirectAuthenticationEntryPoint(String loginPageUrl) {
//        this.loginPageUrl = loginPageUrl;
//    }
//
//    @Override
//    public void commence(HttpServletRequest request,
//                         HttpServletResponse response,
//                         AuthenticationException authException)
//            throws IOException, ServletException {
//        // Redirect the unauthenticated request to the login page
//        response.sendRedirect(loginPageUrl);
//    }
//}
