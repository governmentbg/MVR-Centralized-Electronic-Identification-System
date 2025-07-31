package bg.bulsi.mvr.raeicei.backend.service.impl;

import bg.bulsi.mvr.common.exception.EntityNotFoundException;
import bg.bulsi.mvr.common.exception.ErrorCode;
import bg.bulsi.mvr.common.exception.ValidationMVRException;
import bg.bulsi.mvr.raeicei.backend.mapper.AuthorizedPersonMapper;
import bg.bulsi.mvr.raeicei.backend.service.AuthorizedPersonService;
import bg.bulsi.mvr.raeicei.contract.dto.ContactDTO;
import bg.bulsi.mvr.raeicei.contract.dto.EidManagerStatus;
import bg.bulsi.mvr.raeicei.model.entity.Contact;
import bg.bulsi.mvr.raeicei.model.entity.EidManager;
import bg.bulsi.mvr.raeicei.model.repository.ContactRepository;
import bg.bulsi.mvr.raeicei.model.repository.EidManagerRepository;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Service;

import java.util.UUID;

import static bg.bulsi.mvr.common.exception.ErrorCode.AUTHORIZED_PERSON_NOT_FOUND;
import static bg.bulsi.mvr.common.exception.ErrorCode.EID_ADMINISTRATOR_NOT_FOUND;

@Slf4j
@Service
@RequiredArgsConstructor
public class AuthorizedPersonServiceImpl implements AuthorizedPersonService {

    private final ContactRepository repository;
    private final EidManagerRepository managerRepository;
    private final AuthorizedPersonMapper mapper;

    @Override
    public Contact getAuthorizedPersonById(UUID id) {
        return repository.findById(id).orElseThrow(() -> new EntityNotFoundException(AUTHORIZED_PERSON_NOT_FOUND, id.toString(), id.toString()));
    }

    @Override
    public Contact createAuthorizedPerson(ContactDTO dto, UUID eidManagerId) {
        Contact entity = mapper.mapToEntity(dto);
        entity.setIsActive(null);

        EidManager eidManager = getEidManagerById(eidManagerId);

        checkEidManagerCurrentStatus(eidManager);

        eidManager.setManagerStatus(EidManagerStatus.PENDING_ATTACHMENTS);
        eidManager.getAuthorizedPersons().add(entity);

        return repository.save(entity);
    }

    @Override
    public Contact updateAuthorizedPerson(UUID id, ContactDTO contactDTO, UUID eidManagerId) {
        Contact entity = getAuthorizedPersonById(id);
        entity = mapper.mapToEntity(entity, contactDTO);
        entity.setIsActive(null);

        EidManager eidManager = getEidManagerById(eidManagerId);

        checkEidManagerCurrentStatus(eidManager);

        eidManager.setManagerStatus(EidManagerStatus.PENDING_ATTACHMENTS);

        return repository.save(entity);
    }

    @Override
    public void deleteAuthorizedPerson(UUID id, UUID eidManagerId) {
        Contact authorizedPerson = getAuthorizedPersonById(id);
        authorizedPerson.setIsActive(false);

        EidManager eidManager = getEidManagerById(eidManagerId);

        checkEidManagerCurrentStatus(eidManager);

        eidManager.getAuthorizedPersons().remove(authorizedPerson);
    }

    private EidManager getEidManagerById(UUID eidManagerId) {
        return managerRepository.findById(eidManagerId).orElseThrow(() ->
                new EntityNotFoundException(EID_ADMINISTRATOR_NOT_FOUND, eidManagerId.toString(), eidManagerId.toString()));
    }

    private static void checkEidManagerCurrentStatus(EidManager eidManager) {
        if (EidManagerStatus.STOP.equals(eidManager.getManagerStatus()) || EidManagerStatus.SUSPENDED.equals(eidManager.getManagerStatus()) || EidManagerStatus.IN_REVIEW.equals(eidManager.getManagerStatus())) {
            throw new ValidationMVRException(ErrorCode.WRONG_CURRENT_EID_MANAGER_STATUS);
        }
    }
}
