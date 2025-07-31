package bg.bulsi.mvr.iscei.gateway.config;

import java.util.UUID;

public class SystemProperties {
    public static void setSystemProperties() {
        System.setProperty("instanceUniqueId", UUID.randomUUID().toString());
    }
}
