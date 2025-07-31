package bg.bulsi.mvr.raeicei.backend.mapper;

import bg.bulsi.mvr.raeicei.contract.dto.ContactDTO;
import bg.bulsi.mvr.raeicei.model.entity.Contact;
import org.mapstruct.*;

@Mapper(componentModel = "spring", nullValueCheckStrategy = NullValueCheckStrategy.ALWAYS)
public interface AuthorizedPersonMapper {

    @Mappings({
            @Mapping(target = "citizenIdentifierType", expression = "java(bg.bulsi.mvr.raeicei.contract.dto.IdentifierTypeDTO.fromValue(entity.getCitizenIdentifierType().name()))"),
            @Mapping(source = "EIdentity", target = "eIdentity")
    })
    public ContactDTO mapToAuthorizedPersonDto(Contact entity);

    @Mapping(source = "eIdentity", target = "EIdentity")
    public Contact mapToEntity(ContactDTO dto);

    @Mapping(target = "id", ignore = true)
    public Contact mapToEntity(@MappingTarget Contact entity, ContactDTO dto);
}
