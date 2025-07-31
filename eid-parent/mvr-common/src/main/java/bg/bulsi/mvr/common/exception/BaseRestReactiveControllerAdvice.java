package bg.bulsi.mvr.common.exception;

import bg.bulsi.mvr.audit_logger.BaseAuditLogger;
import bg.bulsi.mvr.audit_logger.dto.AuditData;
import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.audit_logger.model.MessageType;
import bg.bulsi.mvr.common.config.security.UserContext;
import bg.bulsi.mvr.common.config.security.UserContextHolder;
import jakarta.validation.ConstraintViolationException;
import lombok.extern.slf4j.Slf4j;
import org.springframework.amqp.core.AmqpReplyTimeoutException;
import org.springframework.beans.TypeMismatchException;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.autoconfigure.condition.ConditionalOnWebApplication;
import org.springframework.context.support.DefaultMessageSourceResolvable;
import org.springframework.core.codec.DecodingException;
import org.springframework.http.*;
import org.springframework.web.bind.annotation.ExceptionHandler;
import org.springframework.web.bind.annotation.RestControllerAdvice;
import org.springframework.web.bind.support.WebExchangeBindException;
import org.springframework.web.reactive.result.method.annotation.ResponseEntityExceptionHandler;
import org.springframework.web.server.MissingRequestValueException;
import org.springframework.web.server.ServerWebExchange;
import org.springframework.web.server.ServerWebInputException;
import reactor.core.publisher.Mono;

import java.net.URI;
import java.util.*;

@Slf4j
@RestControllerAdvice
@ConditionalOnWebApplication(type = ConditionalOnWebApplication.Type.REACTIVE)
public class BaseRestReactiveControllerAdvice extends ResponseEntityExceptionHandler{
	
	@Autowired
	private BaseAuditLogger auditLogger;
	
	@Override
	protected Mono<ResponseEntity<Object>> handleExceptionInternal(Exception ex, Object body, HttpHeaders headers,
			HttpStatusCode status, ServerWebExchange exchange) {
    	log.error(".handleExceptionInternal() [RequestID={}, Exception={}]",exchange.getRequest().getId(), ex);
		return super.handleExceptionInternal(ex, body, headers, status, exchange);
	}

	@Override
	protected Mono<ResponseEntity<Object>> handleWebExchangeBindException(WebExchangeBindException ex, HttpHeaders headers, HttpStatusCode status, ServerWebExchange exchange) {
		HttpStatusCode responseCode = HttpStatus.BAD_REQUEST;
		ProblemDetail problemDetail = ProblemDetail.forStatusAndDetail(responseCode, "Field Validation Failed");
		problemDetail.setTitle("Bad Request");
		try {
			problemDetail.setType(exchange.getRequest().getURI());
		} catch (Exception e) {
			problemDetail.setType(URI.create("/"));
		}
		if (!ex.getFieldErrors().isEmpty()) {
			problemDetail.setProperty("errors", ex.getFieldErrors().stream().map(DefaultMessageSourceResolvable::getDefaultMessage).toList());
		}
		return this.handleExceptionInternal(ex, problemDetail, headers, responseCode, exchange);
	}

	@Override
	protected Mono<ResponseEntity<Object>> handleServerWebInputException(ServerWebInputException ex, HttpHeaders headers, HttpStatusCode status, ServerWebExchange exchange) {
		ProblemDetail problemDetail = ex.getBody();
		if (ex.getCause() instanceof TypeMismatchException e) {
			problemDetail.setProperty("errors", List.of(e.getMessage()));
		} else if (ex.getCause() instanceof DecodingException e) {
			problemDetail.setProperty("errors", List.of(e.getMessage()));
		} else {
			problemDetail.setProperty("errors", List.of(ex.getMessage()));
		}
		return this.handleExceptionInternal(ex, problemDetail, headers, status, exchange);
	}

	@Override
	protected Mono<ResponseEntity<Object>> handleMissingRequestValueException(MissingRequestValueException ex, HttpHeaders headers, HttpStatusCode status, ServerWebExchange exchange) {
		ProblemDetail problemDetail = ex.getBody();
		problemDetail.setProperty("errors", List.of(ex.getReason()));
		return this.handleExceptionInternal(ex, problemDetail, headers, status, exchange);
	}

	@ExceptionHandler({BaseMVRException.class})
    public Mono<ResponseEntity<Object>> handleAbstractMVRException(BaseMVRException ex, ServerWebExchange exchange) {
        HttpStatusCode responseCode = Optional.of(HttpStatus.valueOf(ex.getStatus())).orElse(HttpStatus.INTERNAL_SERVER_ERROR);
        ProblemDetail problemDetail = ProblemDetail.forStatusAndDetail(responseCode, ex.getMessage());
        problemDetail.setTitle(ex.getTitle());
        try {
            problemDetail.setType(exchange.getRequest().getURI());
        } catch (Exception e) {
            problemDetail.setType(URI.create("/"));
        }
		if (ex.getAdditionalProperties() != null && !ex.getAdditionalProperties().isEmpty()) {
			problemDetail.setProperty("errors", ex.getAdditionalProperties());
		}
        return this.handleExceptionInternal(ex, problemDetail, null, responseCode, exchange);
    }
    
    @ExceptionHandler({AmqpReplyTimeoutException.class})
    public Mono<ResponseEntity<Object>> handleAmqpReplyTimeoutException(AmqpReplyTimeoutException ex, ServerWebExchange exchange) {
        HttpStatusCode responseCode = HttpStatus.GATEWAY_TIMEOUT;
        ProblemDetail problemDetail = ProblemDetail.forStatusAndDetail(responseCode, ex.getMessage());
        problemDetail.setTitle("Gateway reply timeout");
        problemDetail.setDetail("Gateway did not get a response in time from the upstream");
        
        return this.handleExceptionInternal(ex, problemDetail, null, responseCode, exchange);
    }
    
	@ExceptionHandler({Exception.class})
	public Mono<ResponseEntity<Object>> handleBaseException(Exception ex, ServerWebExchange exchange) {
		ProblemDetail problemDetail = ProblemDetail.forStatus(HttpStatus.INTERNAL_SERVER_ERROR);
		problemDetail.setTitle("Internal Server Error");
	    problemDetail.setDetail("An unexpected error occurred.");
		
        return this.handleExceptionInternal(ex, problemDetail, null, HttpStatus.INTERNAL_SERVER_ERROR, exchange);
	}
	
	@Override
	protected Mono<ResponseEntity<Object>> createResponseEntity(Object body, HttpHeaders headers, HttpStatusCode status,
			ServerWebExchange exchange) {
		
		boolean hasUserContext = exchange.getResponse().getHeaders().containsKey(UserContext.USER_CONTEXT_KEY);
		if(hasUserContext){
			exchange.getResponse().getHeaders().get(UserContext.USER_CONTEXT_KEY).stream().findFirst().ifPresent(e -> {
				UserContext userContext = UserContextHolder.fromJson(e);
				
				AuditData requestAuditEvent = AuditData.builder().correlationId(userContext.getGlobalCorrelationId().toString())
						.eventType(exchange.getAttribute(AuditEventType.AUDIT_EVENT_TYPE_KEY)).messageType(MessageType.FAIL).payload(null)
						.requesterUserId(userContext.getRequesterUserId())
						.requesterSystemId(userContext.getSystemId())
						.requesterSystemName(userContext.getSystemName())
						.targetUserId(userContext.getTargetUserId()).build();
				
				auditLogger.auditEvent(requestAuditEvent);
	        });
		}
		
		return super.createResponseEntity(body, headers, status, exchange);
	}

	@ExceptionHandler({ConstraintViolationException.class})
	public Mono<ResponseEntity<Object>> handleConstraintViolationException(ConstraintViolationException ex, ServerWebExchange exchange) {
		HttpStatusCode responseCode = HttpStatus.BAD_REQUEST;
		ProblemDetail problemDetail = ProblemDetail.forStatusAndDetail(responseCode, "Field Validation Failed");
		problemDetail.setTitle("Bad Request");
		try {
			problemDetail.setType(exchange.getRequest().getURI());
		} catch (Exception e) {
			problemDetail.setType(URI.create("/"));
		}
		problemDetail.setProperty("errors", ex.getMessage());
		return this.handleExceptionInternal(ex, problemDetail, null, responseCode, exchange);
	}
}
