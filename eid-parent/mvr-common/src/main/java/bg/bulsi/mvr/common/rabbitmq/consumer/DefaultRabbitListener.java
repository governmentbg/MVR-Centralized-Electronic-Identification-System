package bg.bulsi.mvr.common.rabbitmq.consumer;

import org.springframework.amqp.rabbit.annotation.Queue;
import org.springframework.amqp.rabbit.annotation.QueueBinding;
import org.springframework.amqp.rabbit.annotation.RabbitListener;
import org.springframework.core.annotation.AliasFor;

import java.lang.annotation.ElementType;
import java.lang.annotation.Retention;
import java.lang.annotation.RetentionPolicy;
import java.lang.annotation.Target;

@Target({ElementType.TYPE, ElementType.METHOD, ElementType.ANNOTATION_TYPE})
@Retention(RetentionPolicy.RUNTIME)
@RabbitListener(errorHandler = "rabbitErrorHandler", containerFactory = "simpleRabbitListenerContainerFactory", replyPostProcessor = "rabbitReplyPostProcessor")
public @interface DefaultRabbitListener {
    @AliasFor(annotation = RabbitListener.class)
    String id() default "";

    @AliasFor(annotation = RabbitListener.class)
    String[] queues() default {};

    @AliasFor(annotation = RabbitListener.class)
    Queue[] queuesToDeclare() default {};

    @AliasFor(annotation = RabbitListener.class)
    boolean exclusive() default false;

    @AliasFor(annotation = RabbitListener.class)
    String priority() default "";

    @AliasFor(annotation = RabbitListener.class)
    String admin() default "";

    @AliasFor(annotation = RabbitListener.class)
    QueueBinding[] bindings() default {};

    @AliasFor(annotation = RabbitListener.class)
    String group() default "";

    @AliasFor(annotation = RabbitListener.class)
    String returnExceptions() default "";

    @AliasFor(annotation = RabbitListener.class)
    String concurrency() default "";

    @AliasFor(annotation = RabbitListener.class)
    String autoStartup() default "";

    @AliasFor(annotation = RabbitListener.class)
    String executor() default "";

    @AliasFor(annotation = RabbitListener.class)
    String ackMode() default "";

    @AliasFor(annotation = RabbitListener.class)
    String messageConverter() default "";

    @AliasFor(annotation = RabbitListener.class)
    String replyContentType() default "";

    @AliasFor(annotation = RabbitListener.class)
    String converterWinsContentType() default "true";

    @AliasFor(annotation = RabbitListener.class)
    String batch() default "";
}
