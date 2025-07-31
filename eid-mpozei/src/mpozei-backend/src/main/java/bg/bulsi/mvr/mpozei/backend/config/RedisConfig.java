package bg.bulsi.mvr.mpozei.backend.config;

import org.springframework.cache.Cache;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.data.redis.cache.RedisCacheConfiguration;
import org.springframework.data.redis.cache.RedisCacheManager;
import org.springframework.data.redis.connection.RedisConnectionFactory;

import java.time.Duration;

@Configuration
public class RedisConfig {
	
	public static final String OTP_CODE_CACHE = "OTP_CODE_CACHE";
	public static final String APPLICATION_ID_CACHE = "APPLICATION_ID_CACHE";

	@Bean
	public RedisCacheManager cacheManager(RedisConnectionFactory connectionFactory) {
	    return RedisCacheManager.builder(connectionFactory)
	    .withCacheConfiguration(OTP_CODE_CACHE, RedisCacheConfiguration.defaultCacheConfig().entryTtl(Duration.ofMinutes(30)))
	    .withCacheConfiguration(APPLICATION_ID_CACHE, RedisCacheConfiguration.defaultCacheConfig().entryTtl(Duration.ofMinutes(30)))
	    .cacheDefaults(RedisCacheConfiguration.defaultCacheConfig().entryTtl(Duration.ofMinutes(30)))
	    .transactionAware()
		.build();
	}
	
	@Bean("otpCodeCache")
	public Cache otpCodeCache(RedisCacheManager cacheManager) {
		return cacheManager.getCache(OTP_CODE_CACHE);
	}

	@Bean(name = "applicationIdCache")
	public Cache applicationIdCache(RedisCacheManager cacheManager) {
		return cacheManager.getCache(APPLICATION_ID_CACHE);
	}
}
