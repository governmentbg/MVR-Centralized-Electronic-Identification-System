package bg.bulsi.mvr.mpozei.backend.client.config;

import feign.Client;
import feign.Feign;
import feign.Retryer;
import feign.codec.ErrorDecoder;
import org.springframework.boot.ssl.SslBundle;
import org.springframework.boot.ssl.SslBundles;
import org.springframework.context.annotation.Bean;
import org.springframework.security.authentication.AnonymousAuthenticationToken;
import org.springframework.security.oauth2.client.OAuth2AuthorizeRequest;

import javax.net.ssl.SSLSocketFactory;

import static org.springframework.security.core.authority.AuthorityUtils.createAuthorityList;

public class FeignEjbcaConfig {
    @Bean
    public ErrorDecoder errorDecoder() {
        return new FeignEjbcaErrorDecoder();
    }

    @Bean
    public Retryer feignRetryer() {
        return new Retryer.Default();
    }

    @Bean
    public Feign.Builder feignBuilder(SslBundles sslBundles){
        SslBundle sslBundle = sslBundles.getBundle("ejbca-m2m");
        SSLSocketFactory sslSocketFactory = sslBundle.createSslContext().getSocketFactory();

        return Feign.builder().client(new Client.Default(sslSocketFactory, null));
    }
}
