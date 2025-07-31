package bg.bulsi.mvr.extgateway.config;

import java.util.UUID;

public class SystemProperties {
    public static void setSystemProperties() {
        System.setProperty("instanceUniqueId", UUID.randomUUID().toString());
        System.setProperty("spring.amqp.deserialization.trust.all", "true");
    }
}
