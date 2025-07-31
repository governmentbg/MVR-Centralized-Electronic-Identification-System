package bg.bulsi.mvr.common.config.security;

import bg.bulsi.mvr.common.exception.FaultMVRException;
import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;
import reactor.core.publisher.Mono;

import java.io.IOException;
import java.util.UUID;

import static bg.bulsi.mvr.common.config.security.UserContext.USER_CONTEXT_KEY;
import static bg.bulsi.mvr.common.exception.ErrorCode.INTERNAL_SERVER_ERROR;

public class UserContextHolder {
	private static final ThreadLocal<UserContext> servletUserContextThreadLocal = new ThreadLocal<>();

	/*
	 * Parse UserContext from JSON
	 */
	public static UserContext fromJson(String json) {
		try {
			return new ObjectMapper().readValue(json, UserContext.class);
		} catch (JsonProcessingException e) {
			throw new FaultMVRException("Cannot convert json to UserContext" + e, INTERNAL_SERVER_ERROR);
		}
	}

	/*
	 * Parse UserContext from JSON
	 */
	public static UserContext fromJson(byte[] json) {
		try {
			return new ObjectMapper().readValue(json, UserContext.class);
		} catch (IOException e) {
			throw new FaultMVRException("Cannot convert json to UserContext" + e, INTERNAL_SERVER_ERROR);
		}
	}

	/*
	 * Parse JSON from UserContext
	 */
	public static String toJson() {
		String json;
		try {
			json = new ObjectMapper().writeValueAsString(UserContextHolder.getFromServletContext());
		} catch (JsonProcessingException e) {
			throw new FaultMVRException("Cannot convert UserContext to json" + e, INTERNAL_SERVER_ERROR);
		}
		return json;
	}

	/*
	 * Parse JSON from UserContext
	 */
	public static String toJson(UserContext userContext) {
		String json;
		try {
			json = new ObjectMapper().writeValueAsString(userContext);
		} catch (JsonProcessingException e) {
			throw new FaultMVRException("Cannot convert UserContext to json" + e, INTERNAL_SERVER_ERROR);
		}
		return json;
	}
	
	/*
	 * Get current user's UserContext from the ThreadLocal object unique to each
	 * thread (For Servlet Web Applications)
	 */
	public static UserContext getFromServletContext() {
		return servletUserContextThreadLocal.get();
	}

	/*
	 * Set current user's UserContext to the ThreadLocal object unique to each
	 * thread (For Servlet Web Applications)
	 */
	public static void setToServletContext(UserContext userContext) {
		servletUserContextThreadLocal.set(userContext);
	}

	/*
	 * Get current user's UserContext from the subscription context (For Reactive
	 * Web Applications)
	 */
	public static Mono<UserContext> getFromReactiveContext() {
		return Mono.deferContextual(contextView -> Mono.just(contextView.get(USER_CONTEXT_KEY)));
	}

	/*
	 * Provide UserContext to every chained subscription as part of a global static
	 * context variable. Exists only within the chain of subsequent subscriptions
	 * (For Reactive Web Applications)
	 */
	public static <T> Mono<T> setToReactiveContext(Mono<T> subscription, Mono<UserContext> userContext) {
		return userContext
				.flatMap(user -> subscription.contextWrite(context -> context.put(UserContext.USER_CONTEXT_KEY, user)));
	}

	public static Mono<UserContext> emptyContext() {
		return Mono.just(UserContext.builder().username("SYSTEM").globalCorrelationId(UUID.randomUUID()).build());
	}

	public static UserContext emptyServletContext() {
		return UserContext.builder().username("SYSTEM").globalCorrelationId(UUID.randomUUID()).build();
	}
}
