package bg.bulsi.mvr.common.pipeline;

public interface PipelineEntity {
    PipelineStatus getPipelineStatus();

    void setPipelineStatus(PipelineStatus status);
}
