package bg.bulsi.mvr.raeicei.backend.config;

import org.springframework.cache.Cache;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.data.redis.cache.RedisCacheConfiguration;
import org.springframework.data.redis.cache.RedisCacheManager;
import org.springframework.data.redis.connection.RedisConnectionFactory;

import java.time.Duration;

@Configuration
public class RedisConfig {
	public static final String EID_ADMINISTRATOR_CACHE = "EID_ADMINISTRATOR_CACHE";
	public static final String EID_ADMINISTRATOR_LIST_CACHE = "EID_ADMINISTRATOR_LIST_CACHE";
	public static final String EID_CENTER_CACHE = "EID_CENTER_CACHE";
	public static final String EID_CENTER_LIST_CACHE = "EID_CENTER_LIST_CACHE";
	public static final String PROVIDED_SERVICE_CACHE = "PROVIDED_SERVICE_CACHE";
	public static final String PROVIDED_SERVICE_LIST_CACHE = "PROVIDED_SERVICE_LIST_CACHE";
	public static final String EID_ADMINISTRATOR_OFFICE_CACHE = "EID_ADMINISTRATOR_OFFICE_CACHE";
	public static final String EID_ADMINISTRATOR_OFFICE_LIST_CACHE = "EID_ADMINISTRATOR_OFFICE_LIST_CACHE";
	public static final String DEVICE_CACHE = "DEVICE_CACHE";
	public static final String DEVICE_LIST_CACHE = "DEVICE_LIST_CACHE";

	@Bean
	public RedisCacheManager cacheManager(RedisConnectionFactory connectionFactory) {
	    return RedisCacheManager.builder(connectionFactory)
	    .withCacheConfiguration(EID_ADMINISTRATOR_CACHE, RedisCacheConfiguration.defaultCacheConfig().entryTtl(Duration.ofHours(1)))
	    .withCacheConfiguration(EID_ADMINISTRATOR_LIST_CACHE, RedisCacheConfiguration.defaultCacheConfig().entryTtl(Duration.ofHours(1)))
	    .withCacheConfiguration(EID_CENTER_CACHE, RedisCacheConfiguration.defaultCacheConfig().entryTtl(Duration.ofHours(1)))
	    .withCacheConfiguration(EID_CENTER_LIST_CACHE, RedisCacheConfiguration.defaultCacheConfig().entryTtl(Duration.ofHours(1)))
	    .withCacheConfiguration(PROVIDED_SERVICE_CACHE, RedisCacheConfiguration.defaultCacheConfig().entryTtl(Duration.ofHours(1)))
	    .withCacheConfiguration(PROVIDED_SERVICE_LIST_CACHE, RedisCacheConfiguration.defaultCacheConfig().entryTtl(Duration.ofHours(1)))
	    .withCacheConfiguration(EID_ADMINISTRATOR_OFFICE_CACHE, RedisCacheConfiguration.defaultCacheConfig().entryTtl(Duration.ofHours(1)))
	    .withCacheConfiguration(EID_ADMINISTRATOR_OFFICE_LIST_CACHE, RedisCacheConfiguration.defaultCacheConfig().entryTtl(Duration.ofHours(1)))
	    .withCacheConfiguration(DEVICE_CACHE, RedisCacheConfiguration.defaultCacheConfig().entryTtl(Duration.ofHours(1)))
	    .withCacheConfiguration(DEVICE_LIST_CACHE, RedisCacheConfiguration.defaultCacheConfig().entryTtl(Duration.ofHours(1)))
	    .transactionAware()
		.build();
	}
}
