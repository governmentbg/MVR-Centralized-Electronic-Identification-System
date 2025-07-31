package bg.bulsi.mvr.raeicei.backend.facade;

import bg.bulsi.mvr.raeicei.contract.dto.ContactDTO;

import java.util.UUID;


public interface AuthorizedPersonFacade {

    ContactDTO getAuthorizedPersonById(UUID id);

    ContactDTO createAuthorizedPerson(ContactDTO dto, UUID eidManagerId);

    ContactDTO updateAuthorizedPerson(UUID id, ContactDTO dto, UUID eidManagerId);

    void deleteAuthorizedPerson(UUID id, UUID eidManagerId);
}