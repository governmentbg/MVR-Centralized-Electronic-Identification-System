package bg.bulsi.mvr.common.pipeline;

import lombok.Getter;
import lombok.Setter;
import org.springframework.boot.context.properties.ConfigurationProperties;
import org.springframework.stereotype.Component;

import java.util.List;
import java.util.Map;

@Component
@Getter
@Setter
@ConfigurationProperties(prefix = "pipeline")
public class PipelineConfig {
    private Map<String, List<String>> config;
}
