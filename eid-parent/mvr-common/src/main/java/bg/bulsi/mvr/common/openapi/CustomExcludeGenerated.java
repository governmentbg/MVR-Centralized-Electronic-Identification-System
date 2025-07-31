package bg.bulsi.mvr.common.openapi;

import static java.lang.annotation.ElementType.CONSTRUCTOR;
import static java.lang.annotation.ElementType.METHOD;
import static java.lang.annotation.ElementType.TYPE;
import static java.lang.annotation.RetentionPolicy.RUNTIME;

import java.lang.annotation.Documented;
import java.lang.annotation.Retention;
import java.lang.annotation.Target;

@Documented
@Retention(RUNTIME)
@Target({TYPE, METHOD,CONSTRUCTOR})
/**
 * Used to exclude elements from OpenAPI code generator for code coverage
 */
public @interface CustomExcludeGenerated {
}
