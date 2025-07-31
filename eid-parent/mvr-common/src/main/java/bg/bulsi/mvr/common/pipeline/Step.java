package bg.bulsi.mvr.common.pipeline;

import lombok.Getter;
import lombok.Setter;

public abstract class Step<T> {
    public abstract T process(T input);

    public abstract PipelineStatus getStatus();

    @Setter
    @Getter
    Class<? extends Pipeline> pipelineClass;
}
