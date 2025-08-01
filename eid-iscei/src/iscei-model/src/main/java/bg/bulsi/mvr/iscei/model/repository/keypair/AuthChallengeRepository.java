package bg.bulsi.mvr.iscei.model.repository.keypair;

import java.util.UUID;

import org.springframework.data.repository.CrudRepository;

import bg.bulsi.mvr.iscei.model.AuthenticationChallenge;

public interface AuthChallengeRepository extends CrudRepository<AuthenticationChallenge, UUID> {

}
