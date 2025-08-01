package bg.bulsi.mvr.iscei.gateway.controller.v1;

import java.util.Collections;
import java.util.Optional;
import java.util.Set;
import java.util.stream.Collectors;

import org.springframework.beans.factory.annotation.Value;
import jakarta.annotation.PostConstruct;
import lombok.extern.slf4j.Slf4j;

@Slf4j
abstract class BaseAuthorizationController {

	@Value("${mvr.supported-scopes}")
	private Set<String> supportedScopes;
	
    /**
     * Filters the incoming scopes to only include those supported by the system.
     * Unsupported scopes are silently ignored.
     *
     * @param incomingScopes the incoming set of scopes
     */
	protected void filterSupportedScopes(Set<String> incomingScopes) {
		incomingScopes.retainAll(supportedScopes);
	}
	
	protected Set<String> copySet(Set<String> existingSet) {
		return Optional.ofNullable(existingSet).orElse(Collections.emptySet());
	}
	
	protected String joinSet(Set<String> existingSet) {
		return existingSet.stream()
                .filter(e -> e != null)
                .collect(Collectors.joining(" "));
	}
	
	@PostConstruct
	private void init() {
		log.info(".init() [supportedScopes={}]", supportedScopes);
	}
}
