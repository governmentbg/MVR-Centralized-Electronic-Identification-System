package bg.bulsi.mvr.mpozei.backend.client.config;

import feign.Client;
import feign.Feign;
import feign.Retryer;
import feign.codec.ErrorDecoder;
import org.springframework.boot.ssl.SslBundle;
import org.springframework.boot.ssl.SslBundles;
import org.springframework.context.annotation.Bean;

import javax.net.ssl.SSLSocketFactory;

public class SignServerConfig {
    @Bean
    public ErrorDecoder errorDecoder() {
        return new FeignDigitallErrorDecoder();
    }

    @Bean
    public Retryer feignRetryer() {
        return new Retryer.Default();
    }

    @Bean
    public Feign.Builder feignBuilder(SslBundles sslBundles){
        SslBundle sslBundle = sslBundles.getBundle("sign-server-m2m");
        SSLSocketFactory sslSocketFactory = sslBundle.createSslContext().getSocketFactory();

        return Feign.builder().client(new Client.Default(sslSocketFactory, null));
    }
}
