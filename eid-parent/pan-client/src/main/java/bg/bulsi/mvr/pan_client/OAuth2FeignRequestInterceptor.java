package bg.bulsi.mvr.pan_client;

import feign.RequestInterceptor;
import feign.RequestTemplate;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.security.oauth2.client.OAuth2AuthorizeRequest;
import org.springframework.security.oauth2.client.OAuth2AuthorizedClientManager;
import org.springframework.security.oauth2.core.OAuth2AccessToken;

public class OAuth2FeignRequestInterceptor implements RequestInterceptor {

    @Autowired
    private OAuth2AuthorizedClientManager oAuth2AuthorizedClientManager;

    private final OAuth2AuthorizeRequest oAuth2AuthorizeRequest;

    OAuth2FeignRequestInterceptor(OAuth2AuthorizeRequest oAuth2AuthorizeRequest) {
        this.oAuth2AuthorizeRequest = oAuth2AuthorizeRequest;
    }

    @Override
    public void apply(RequestTemplate template) {
        template.header("Authorization", getAuthorizationToken());
    }

    private String getAuthorizationToken() {
        final OAuth2AccessToken accessToken = oAuth2AuthorizedClientManager.authorize(oAuth2AuthorizeRequest).getAccessToken();
        return String.format("%s %s", accessToken.getTokenType().getValue(), accessToken.getTokenValue());
    }

}
