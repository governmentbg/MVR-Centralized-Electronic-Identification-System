package bg.bulsi.mvr.common.exception;

import java.net.URI;
import java.util.List;
import java.util.Map;
import java.util.Optional;
import java.util.stream.Collectors;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.autoconfigure.condition.ConditionalOnWebApplication;
import org.springframework.http.HttpHeaders;
import org.springframework.http.HttpStatus;
import org.springframework.http.HttpStatusCode;
import org.springframework.http.ProblemDetail;
import org.springframework.http.ResponseEntity;
import org.springframework.validation.FieldError;
import org.springframework.web.bind.MethodArgumentNotValidException;
import org.springframework.web.bind.annotation.ExceptionHandler;
import org.springframework.web.bind.annotation.RestControllerAdvice;
import org.springframework.web.context.request.WebRequest;
import org.springframework.web.reactive.function.client.WebClientResponseException;
import org.springframework.web.servlet.mvc.method.annotation.ResponseEntityExceptionHandler;

import bg.bulsi.mvr.audit_logger.BaseAuditLogger;
import bg.bulsi.mvr.audit_logger.dto.AuditData;
import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.audit_logger.model.MessageType;
import bg.bulsi.mvr.common.config.security.UserContext;
import bg.bulsi.mvr.common.config.security.UserContextHolder;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import lombok.extern.slf4j.Slf4j;

@Slf4j
@RestControllerAdvice
@ConditionalOnWebApplication(type = ConditionalOnWebApplication.Type.SERVLET)
public class BaseRestServletControllerAdvice extends ResponseEntityExceptionHandler {
	
	@Autowired
	private BaseAuditLogger auditLogger;
	
	public BaseRestServletControllerAdvice() {
    	log.info(".BaseRestServletControllerAdvice()");
	}
	
	@Override
	protected ResponseEntity<Object> handleMethodArgumentNotValid(MethodArgumentNotValidException ex,
			HttpHeaders headers, HttpStatusCode status, WebRequest request) {
    	log.error(".handleBindException() [Exception={}]", ex);

    	HttpStatusCode responseCode = HttpStatus.BAD_REQUEST;
        ProblemDetail problemDetail = ProblemDetail.forStatusAndDetail(responseCode, "Field Validation Failed");
//        problemDetail.setTitle(ex.getTitle());
//        try {
//            problemDetail.setType(new URI(request.getRequestURI()));
//        } catch (Exception e) {
//            problemDetail.setType(URI.create("/"));
//        }
		if (ex.getBindingResult() != null && ex.getBindingResult().hasErrors()) {
	        // Group the field errors by field name and collect error messages
	        Map<String, List<String>> errors = ex.getBindingResult().getFieldErrors()
	            .stream()
	            .collect(Collectors.groupingBy(
	                    FieldError::getField,
	                    Collectors.mapping(FieldError::getDefaultMessage, Collectors.toList())
	            ));
			
			
			problemDetail.setProperty("errors", errors);
		}
		
		return new ResponseEntity<>(problemDetail, null, responseCode);
	}
	
	@ExceptionHandler({BaseMVRException.class})
    public ResponseEntity<Object> handleBaseMVRException(BaseMVRException ex, 
    		HttpServletRequest request) {
    	log.error(".handleBaseMVRException() [RequestID={}, Exception={}]", request.getRequestId(), ex);
    	
    	this.logAuditLog(request);
    	
        HttpStatusCode responseCode = Optional.of(HttpStatus.valueOf(ex.getStatus())).orElse(HttpStatus.INTERNAL_SERVER_ERROR);
        ProblemDetail problemDetail = ProblemDetail.forStatusAndDetail(responseCode, ex.getMessage());
        problemDetail.setTitle(ex.getTitle());
        try {
            problemDetail.setType(new URI(request.getRequestURI()));
        } catch (Exception e) {
            problemDetail.setType(URI.create("/"));
        }
		if (ex.getAdditionalProperties() != null && !ex.getAdditionalProperties().isEmpty()) {
			problemDetail.setProperty("errors", ex.getAdditionalProperties());
		}
		
		return new ResponseEntity<>(problemDetail, null, responseCode);
    }
	
    @ExceptionHandler(WebClientResponseException.class)
    public ResponseEntity<Object> handleWebClientResponseException(
    		WebClientResponseException ex, 
    		HttpServletRequest request,
    		HttpServletResponse response) {
    	log.error(".handleWebClientResponseException() [RequestID={}, Exception={}]", request.getRequestId(), ex);
    	
    	this.logAuditLog(request);
    	
        HttpStatusCode responseCode = Optional.of(ex.getStatusCode()).orElse(HttpStatus.INTERNAL_SERVER_ERROR);
        ProblemDetail problemDetail = ProblemDetail.forStatusAndDetail(responseCode, ex.getResponseBodyAsString());
        try {
            problemDetail.setType(new URI(request.getRequestURI()));
        } catch (Exception e) {
            problemDetail.setType(URI.create("/"));
        }
//		if (ex.getAdditionalProperties() != null && !ex.getAdditionalProperties().isEmpty()) {
//			problemDetail.setProperty("errors", ex.getAdditionalProperties());
//		}
        
		return new ResponseEntity<>(problemDetail, null, responseCode);
    	
       //return ResponseEntity.status(e.getStatusCode().value()).body(e.getResponseBodyAsString());
    }
    
	@ExceptionHandler({Exception.class})
	public ResponseEntity<Object> handleException(Exception ex, HttpServletRequest request) {
    	log.error(".handleException() [RequestID={}, Exception={}]", request.getRequestId(), ex);
    	
    	this.logAuditLog(request);
    	
		HttpStatusCode responseCode = HttpStatus.INTERNAL_SERVER_ERROR;
        ProblemDetail problemDetail = ProblemDetail.forStatusAndDetail(responseCode, ex.getMessage());

		return new ResponseEntity<>(problemDetail, responseCode);
	}
	
	private void logAuditLog(HttpServletRequest request) {
    	UserContext userContext = UserContextHolder.getFromServletContext();
		
    	AuditEventType auditEventType = (AuditEventType) request.getAttribute(AuditEventType.AUDIT_EVENT_TYPE_KEY);
    	//May not exists in curtain cases
    	if(auditEventType == null) {
    		return;
    	}
    	
		AuditData requestAuditEvent = AuditData.builder()
				.correlationId(userContext.getGlobalCorrelationId().toString())
				.eventType(auditEventType)
				.messageType(MessageType.FAIL)
				.payload(null)
				.requesterUserId(userContext.getRequesterUserId())
				.requesterSystemId(userContext.getSystemId())
				.requesterSystemName(userContext.getSystemName())
				.targetUserId(userContext.getTargetUserId()).build();
		
		auditLogger.auditEvent(requestAuditEvent);
	}
	
//	@ExceptionHandler({ValidationMVRException.class})
//    public ResponseEntity<Object> handleBaseMVRException(ValidationMVRException ex) {
//        HttpStatusCode responseCode = Optional.of(HttpStatus.valueOf(ex.getStatus())).orElse(HttpStatus.INTERNAL_SERVER_ERROR);
//        ProblemDetail problemDetail = ProblemDetail.forStatusAndDetail(responseCode, ex.getMessage());
//        problemDetail.setTitle(ex.getTitle());
//        try {
//            problemDetail.setType(exchange.getRequest().getURI());
//        } catch (Exception e) {
//            problemDetail.setType(URI.create("/"));
//        }
//		if (ex.getAdditionalProperties() != null && !ex.getAdditionalProperties().isEmpty()) {
//			problemDetail.setProperty("errors", ex.getAdditionalProperties());
//		}
//		
//		return new ResponseEntity<>(problemDetail, null, responseCode);
//    }
}
