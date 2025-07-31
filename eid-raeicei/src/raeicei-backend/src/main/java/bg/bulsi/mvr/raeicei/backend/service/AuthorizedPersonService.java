package bg.bulsi.mvr.raeicei.backend.service;

import bg.bulsi.mvr.raeicei.contract.dto.ContactDTO;
import bg.bulsi.mvr.raeicei.model.entity.Contact;

import java.util.UUID;

public interface AuthorizedPersonService {

    Contact getAuthorizedPersonById(UUID id);

    Contact createAuthorizedPerson(ContactDTO contactDTO, UUID eidManagerId);

    Contact updateAuthorizedPerson(UUID id, ContactDTO contactDTO, UUID eidManagerId);

    void deleteAuthorizedPerson(UUID id, UUID eidManagerId);
}
