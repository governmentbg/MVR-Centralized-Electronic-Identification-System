package bg.bulsi.mvr.iscei.model.repository.keypair;

import java.util.UUID;

import org.springframework.data.repository.CrudRepository;

import bg.bulsi.mvr.iscei.model.OtpToken;

public interface OtpTokenRepository extends CrudRepository<OtpToken, UUID>{
}
