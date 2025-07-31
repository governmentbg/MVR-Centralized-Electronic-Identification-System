package bg.bulsi.mvr.audit_logger;

import bg.bulsi.mvr.audit_logger.dto.AuditData;
import bg.bulsi.mvr.audit_logger.dto.AuditLoggingKey;
import bg.bulsi.mvr.audit_logger.dto.EventPayload;
import bg.bulsi.mvr.audit_logger.model.AuditLogRecord;
import ch.qos.logback.classic.LoggerContext;
import ch.qos.logback.classic.spi.ILoggingEvent;
import ch.qos.logback.core.Appender;
import ch.qos.logback.core.rolling.RollingFileAppender;
import ch.qos.logback.core.rolling.RollingPolicy;
import lombok.extern.slf4j.Slf4j;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.datatype.jsr310.JavaTimeModule;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.amqp.rabbit.RabbitConverterFuture;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Component;

import javax.crypto.Mac;
import javax.crypto.SecretKey;
import javax.crypto.spec.SecretKeySpec;
import java.lang.reflect.Field;
import java.nio.charset.StandardCharsets;
import java.time.OffsetDateTime;
import java.time.ZoneOffset;
import java.time.format.DateTimeFormatter;
import java.time.format.DateTimeFormatterBuilder;
import java.time.temporal.ChronoField;
import java.util.Arrays;
import java.util.Base64;
import java.util.Iterator;
import java.util.Map;
import java.util.TreeMap;
import java.util.UUID;

@Slf4j
@Component
public class BaseAuditLogger {
    private static final String AUDIT_LOGGER_NAME = "AUDIT_LOGGER";
	private static final Logger classLogger = LoggerFactory.getLogger(BaseAuditLogger.class);
    private static final Logger auditLogger = LoggerFactory.getLogger(AUDIT_LOGGER_NAME);

    private static final DateTimeFormatter FORMATTER = new DateTimeFormatterBuilder()
            // Base date‐time up to seconds
            .appendPattern("yyyy-MM-dd'T'HH:mm:ss")
            // Fraction from 0 to 6 digits, dropping trailing zeros if any
            .appendFraction(ChronoField.MICRO_OF_SECOND, 0, 6, true) 
            // Literal 'Z' for UTC
            .appendLiteral('Z')
            // Use ISO chronology and resolver
            .toFormatter()
            .withZone(ZoneOffset.UTC);
    
    @Autowired
    private EncryptionHelper encryptionHelper;
    
    @Value("${logging.secret-key: #{null}}")
    private String secretKeyEncoded;
    
    @Value("${logging.system-id: #{null}}")
    private String systemId;

    @Value("${logging.module-id: #{null}}")
    private String moduleId;
    
    private ObjectMapper mapper;
    
    public BaseAuditLogger() {
        this.mapper = new ObjectMapper();
        mapper.registerModule(new JavaTimeModule());
    }
    
    public void auditEvent(AuditData auditData) {
    	if(!auditData.getEventType().getIsEnabled()) {
    		return;
    	}

		AuditLogRecord auditLogRecord = new AuditLogRecord();
		auditLogRecord.setEventId(UUID.randomUUID().toString().replace("-", ""));
        auditLogRecord.setCorrelationId(auditData.getCorrelationId());
        auditLogRecord.setEventDate(OffsetDateTime.now().format(FORMATTER));
        auditLogRecord.setEventType(auditData.getEventType().name() + "_" + auditData.getMessageType());
        auditLogRecord.setMessage(createLogMessage(auditData));
        //TODO: https://intracol.atlassian.net/browse/EID-4265 - set proper eventPayload
        //auditLogRecord.setEventPayload(auditData.getPayload());
        auditLogRecord.setEventPayload(this.getEncryptedEventPayload(auditData.getPayload()));
        try {
			auditLogRecord.setRequesterUserId(encryptionHelper.encrypt(auditData.getRequesterUserId()));
		} catch (Exception e) {
			auditLogRecord.setRequesterUserId(null);
		}
        auditLogRecord.setRequesterSystemId(auditData.getRequesterSystemId());
        auditLogRecord.setRequesterSystemName(auditData.getRequesterSystemName());
        auditLogRecord.setTargetUserId(auditData.getTargetUserId());
        auditLogRecord.setSystemId(systemId);
        auditLogRecord.setModuleId(moduleId);
        
        String json = null;
        try {
        	calculateChecksum(auditLogRecord);
			json = mapper.writeValueAsString(auditLogRecord);
		} catch (JsonProcessingException e) {
            classLogger.error(Arrays.toString(e.getStackTrace()));
		}
        
        auditLogger.info(json);
    }

    private void calculateChecksum(AuditLogRecord auditLogRecord) {
        try {
            // Създаваме инстанция на HMACSHA512 с ключа за шифроване
            Mac hmacSha512 = Mac.getInstance("HmacSHA512");
            // Декодиране на ключа
            byte[] decodedKey = Base64.getDecoder().decode(secretKeyEncoded);
            // Пресъздаване на ключа
            SecretKey key = new SecretKeySpec(decodedKey, 0, decodedKey.length, "HmacSHA512");
            SecretKeySpec secretKey = new SecretKeySpec(key.getEncoded(), "HmacSHA512");
            hmacSha512.init(secretKey);
            // Сериализираме обекта „record“ в JSON стринг
            String hashString = mapper.writeValueAsString(auditLogRecord);
            // Конвертираме JSON стринга в байтове, като използваме UTF-8 кодиране
            byte[] data = hashString.getBytes(StandardCharsets.UTF_8);
            // Изчисляваме HMAC-SHA512 хеша
            byte[] checksum = hmacSha512.doFinal(data);
            // Конвертираме checksum към Base64 стринг
            String base64Checksum = Base64.getEncoder().encodeToString(checksum);
            // Сетваме пропъртито Checksum на обекта 'record'
            auditLogRecord.setChecksum(base64Checksum);
        } catch (Exception e) {
            // Прихващаме евентуални грешки
            classLogger.error(Arrays.toString(e.getStackTrace()));
        }
    }

    private Integer getLoggerFileIndex() {
        LoggerContext context = (LoggerContext) LoggerFactory.getILoggerFactory();
        ch.qos.logback.classic.Logger logger = context.getLogger(AUDIT_LOGGER_NAME);

        for (Iterator<Appender<ILoggingEvent>> index = logger.iteratorForAppenders(); index.hasNext(); ) {
            Appender<ILoggingEvent> appender = index.next();
            if(appender instanceof RollingFileAppender<ILoggingEvent> rollingFileAppender) {
                RollingPolicy rollingPolicy = rollingFileAppender.getRollingPolicy();
                String fileName = rollingPolicy.getActiveFileName().split("\\.")[0];
                String[] fileNameParts = fileName.split("_");
                return Integer.valueOf(fileNameParts[fileNameParts.length -1]);
            }
        }
        return null;
    }

    private String getLoggerFileName() {
        LoggerContext context = (LoggerContext) LoggerFactory.getILoggerFactory();
        ch.qos.logback.classic.Logger logger = context.getLogger(AUDIT_LOGGER_NAME);

        for (Iterator<Appender<ILoggingEvent>> index = logger.iteratorForAppenders(); index.hasNext(); ) {
            Appender<ILoggingEvent> appender = index.next();
            if(appender instanceof RollingFileAppender<ILoggingEvent> rollingFileAppender) {
                RollingPolicy rollingPolicy = rollingFileAppender.getRollingPolicy();
                return rollingPolicy.getActiveFileName();
            }
        }
        return null;
    }

    public static String extractCorrelationId(RabbitConverterFuture<?> future) {
        try {
            Field corellationIdField = future.getClass().getSuperclass().getDeclaredField("correlationId");
            corellationIdField.setAccessible(true);
            return (String) corellationIdField.get(future);
        } catch (NoSuchFieldException | IllegalAccessException e) {
            classLogger.error(e.toString());
        }
        return null;
    }

	private String createLogMessage(AuditData data) {
		return switch (data.getMessageType()) {
			case REQUEST -> data.getEventType().getRequestMessageCyrillic();
			case SUCCESS -> data.getEventType().getSuccessMessageCyrillic();
			case FAIL -> data.getEventType().getFailMessageCyrillic();
			default -> throw new IllegalArgumentException("Unexpected value: " + data.getMessageType());
			};
	}
    
	private Map<String, Object> getEncryptedEventPayload(EventPayload eventPayload) {
		if (eventPayload == null) {
			return null;
		}

		Map<String, Object> result = new TreeMap<>(eventPayload.getParams());
		if(result == null || result.isEmpty()) {
			return null;
		}
		
		
		for (Map.Entry<String, Object> entry : result.entrySet()) {
			String key = entry.getKey();
			Object value = entry.getValue();
			try {
				AuditLoggingKey auditLoggingKey = AuditLoggingKey.getByParamName(key);
				Object processedValue = value;
				if(auditLoggingKey == null) {
					result.put(key, processedValue);
					continue;
				}
				
				if (AuditLoggingKey.REQUEST.equals(auditLoggingKey)) {
					processedValue = encryptionHelper.encrypt(mapper.writeValueAsString(value));
				} else if (auditLoggingKey.getIsRequiresEncryption()) {
					processedValue = encryptionHelper.encrypt(value);
				}

				result.put(key, processedValue);
			} catch (Exception e) {
				classLogger.error("Error processing key {}: {}", key, e.getMessage(), e);
			}
		}

		return result;
    }
}
