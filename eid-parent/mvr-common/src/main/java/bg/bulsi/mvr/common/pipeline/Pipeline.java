package bg.bulsi.mvr.common.pipeline;

import jakarta.annotation.PostConstruct;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.ApplicationContext;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Component;

import java.util.Collections;
import java.util.List;
import java.util.Objects;
import java.util.UUID;

@Component
public abstract class Pipeline<T extends PipelineEntity> {
    @Autowired
    private ApplicationContext applicationContext;
    @Autowired
    private PipelineConfig pipelineConfig;
    @Autowired
    private JpaRepository<T, UUID> repository;

    private List<Step<T>> steps;

    private boolean repeatable = false;
    
    @PostConstruct
    private void postConstruct() {
        if (Objects.isNull(pipelineConfig)) {
            this.steps = Collections.emptyList();
            return;
        }
        String pipelineName = this.getClass().getSimpleName();
        this.steps = pipelineConfig.getConfig().get(pipelineName)
                .stream()
                .map(applicationContext::getBean)
                .map(e -> (Step<T>) e).toList();
        this.steps.forEach(step -> step.setPipelineClass(this.getClass()));
    }

    public final T process(T input) {
        preProcess(input);
        for (Step<T> step : steps) {
            input.setPipelineStatus(step.getStatus());
            input = step.process(input);
            repository.save(input);
        }
        postProcess(input);
        return input;
    }

    public abstract boolean canProcess(T entity);

    public void postProcess(T entity) {
    }

    public void preProcess(T entity) {
    }

	/**
	 * @return the repeatable
	 */
	public boolean isRepeatable() {
		return repeatable;
	}

	/**
	 * @param repeatable the repeatable to set
	 */
	protected void setRepeatable(boolean repeatable) {
		this.repeatable = repeatable;
	}
}
