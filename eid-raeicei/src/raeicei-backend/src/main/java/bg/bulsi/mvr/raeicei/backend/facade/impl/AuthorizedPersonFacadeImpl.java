package bg.bulsi.mvr.raeicei.backend.facade.impl;

import bg.bulsi.mvr.raeicei.backend.facade.AuthorizedPersonFacade;
import bg.bulsi.mvr.raeicei.backend.mapper.AuthorizedPersonMapper;
import bg.bulsi.mvr.raeicei.backend.service.AuthorizedPersonService;
import bg.bulsi.mvr.raeicei.contract.dto.ContactDTO;
import bg.bulsi.mvr.raeicei.model.entity.Contact;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Service;

import java.util.UUID;

@Slf4j
@Service
@RequiredArgsConstructor
public class AuthorizedPersonFacadeImpl implements AuthorizedPersonFacade {

    protected final AuthorizedPersonMapper authorizedPersonMapper;

    protected final AuthorizedPersonService authorizedPersonService;

    @Override
    public ContactDTO getAuthorizedPersonById(UUID id) {
        Contact entity = authorizedPersonService.getAuthorizedPersonById(id);
        return authorizedPersonMapper.mapToAuthorizedPersonDto(entity);
    }

    @Override
    public ContactDTO createAuthorizedPerson(ContactDTO dto, UUID eidManagerId) {
        Contact entity = authorizedPersonService.createAuthorizedPerson(dto, eidManagerId);
        return authorizedPersonMapper.mapToAuthorizedPersonDto(entity);
    }

    @Override
    public ContactDTO updateAuthorizedPerson(UUID id, ContactDTO dto, UUID eidManagerId) {
        Contact entity = authorizedPersonService.updateAuthorizedPerson(id, dto, eidManagerId);
        return authorizedPersonMapper.mapToAuthorizedPersonDto(entity);
    }

    @Override
    public void deleteAuthorizedPerson(UUID id, UUID eidManagerId) {
        authorizedPersonService.deleteAuthorizedPerson(id, eidManagerId);
    }
}
