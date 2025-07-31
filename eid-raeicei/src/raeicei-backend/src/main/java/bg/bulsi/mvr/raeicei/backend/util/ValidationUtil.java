package bg.bulsi.mvr.raeicei.backend.util;

import bg.bulsi.mvr.common.exception.ErrorCode;
import bg.bulsi.mvr.common.exception.ValidationMVRException;
import jakarta.validation.ConstraintViolation;
import lombok.RequiredArgsConstructor;
import org.apache.commons.lang3.StringUtils;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Pageable;
import org.springframework.data.domain.Sort;
import org.springframework.http.HttpStatus;
import org.springframework.stereotype.Component;
import org.springframework.validation.beanvalidation.LocalValidatorFactoryBean;

import java.util.*;
import java.util.stream.Collectors;

@Component
@RequiredArgsConstructor
public class ValidationUtil {
    private final LocalValidatorFactoryBean validator;

    public void checkValid(Object obj) {
        Set<ConstraintViolation<Object>> violations = validator.validate(obj);
        if (!violations.isEmpty()) {
            Set<String> errors = violations.stream().map(ConstraintViolation::getMessage).collect(Collectors.toSet());
            throw new ValidationMVRException("Provided " + obj.getClass() + " is not valid.",ErrorCode.VALIDATION_ERROR, errors);
        }
    }

    public static void assertEquals(Object a, Object b, ErrorCode errorCode, Object... params) {
        if (!Objects.equals(a, b)) {
            throw new ValidationMVRException(errorCode, params);
        }
    }

    public static void assertTrue(boolean expression, ErrorCode code, Object... params) {
        if (!expression) {
            throw new ValidationMVRException(code, params);
        }
    }

    public static void assertTrue(boolean expression, ErrorCode code, HttpStatus status, Object... params) {
        if (!expression) {
            throw new ValidationMVRException(code, status.value(), params);
        }
    }

    public static void assertFalse(boolean expression, ErrorCode code, Object... params) {
        if (expression) {
            throw new ValidationMVRException(code, params);
        }
    }

    public static void assertNotNull(Object value, ErrorCode errorCode, Object... params) {
        if (value == null) {
            throw new ValidationMVRException(errorCode, params);
        }
    }

    public static void assertNotEmpty(Collection value, ErrorCode code, Object... params) {
        if (value.isEmpty()) {
            throw new ValidationMVRException(code, params);
        }
    }

    public static void assertNull(Object value, ErrorCode code, Object... params) {
        if (value != null) {
            throw new ValidationMVRException(code, params);
        }
    }

    public static void assertNotBlank(String value, ErrorCode code, Object... params) {
        if(StringUtils.isBlank(value)) {
            throw new ValidationMVRException(code, params);
        }
    }

    public static Pageable filterAllowedPageableSort(Pageable pageable, List<String> validFields) {
        List<Sort.Order> orders = pageable.getSort().stream().filter(e -> validFields.contains(e.getProperty())).toList();
        return PageRequest.of(pageable.getPageNumber(), pageable.getPageSize(), Sort.by(orders));
    }
}
