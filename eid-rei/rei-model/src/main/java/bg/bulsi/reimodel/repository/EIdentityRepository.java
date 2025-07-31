package bg.bulsi.reimodel.repository;


import bg.bulsi.reimodel.model.EIdentity;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.UUID;

@Repository
public interface EIdentityRepository  extends JpaRepository<EIdentity, UUID> {

}
