package bg.bulsi.mvr.iscei.model.repository.keypair;

import java.util.List;
import java.util.UUID;

import org.springframework.data.domain.Sort;
import org.springframework.data.repository.CrudRepository;

import bg.bulsi.mvr.iscei.model.AuthApprovalRequest;

public interface AuthApprovalRequestRepository extends CrudRepository<AuthApprovalRequest, UUID> {
	AuthApprovalRequest findByUsernameAndBindingMessage(String username, String bindingMessage);
	
	List<AuthApprovalRequest> findAllByUsername(String username, Sort sort);
}
