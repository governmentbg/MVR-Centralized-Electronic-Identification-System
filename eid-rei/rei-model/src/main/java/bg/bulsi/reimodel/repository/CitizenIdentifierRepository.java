package bg.bulsi.reimodel.repository;


import bg.bulsi.reimodel.model.CitizenIdentifier;
import bg.bulsi.reimodel.model.IdentifierType;
import bg.bulsi.reimodel.repository.view.CitizenIdentifierView;
import jakarta.persistence.QueryHint;
import org.hibernate.jpa.HibernateHints;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.QueryHints;
import org.springframework.stereotype.Repository;

import java.util.List;
import java.util.Optional;
import java.util.UUID;

@Repository
public interface CitizenIdentifierRepository extends JpaRepository<CitizenIdentifier,UUID> {

	Optional<CitizenIdentifierView> findViewByEidentityIdAndActiveIsTrue(UUID id);

    Optional<CitizenIdentifier> findByEidentityIdAndActiveIsTrue(UUID id);

    Optional<CitizenIdentifierView> findByNumberAndTypeAndActiveIsTrue(String number, IdentifierType type);


    Optional<CitizenIdentifier> findCitizenIdentifierByNumberAndTypeAndActiveIsTrue(String number, IdentifierType type);

    Optional<CitizenIdentifierView> findFirstByEidentity_IdOrderByCreateDateDesc(UUID eidentityId);

//    Optional<CitizenIdentifier> findFirstByEidentity_IdOrderByCreateDateDesc(UUID eidentityId);

    Optional<CitizenIdentifierView> findFirstByNumberAndTypeOrderByCreateDateDesc(String number, IdentifierType type);

    List<CitizenIdentifierView> findByNumberAndType(String citizenIdentifierNumber, IdentifierType identifierType);
}
