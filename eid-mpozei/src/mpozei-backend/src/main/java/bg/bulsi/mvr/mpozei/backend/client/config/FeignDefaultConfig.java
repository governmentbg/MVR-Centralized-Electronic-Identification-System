//package bg.bulsi.mvr.mpozei.backend.client.config;
//
//import feign.Retryer;
//import feign.codec.ErrorDecoder;
//import org.springframework.context.annotation.Bean;
//
//
//public class FeignDefaultConfig {
//    @Bean
//    public ErrorDecoder errorDecoder() {
//        return new FeignErrorDecoder();
//    }
//
//    @Bean
//    public Retryer feignRetryer() {
//        return new Retryer.Default();
//    }
//}
