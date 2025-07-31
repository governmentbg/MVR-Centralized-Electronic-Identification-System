package bg.bulsi.mvr.raeicei.backend.mapper;

import bg.bulsi.mvr.raeicei.contract.dto.NoteDTO;
import bg.bulsi.mvr.raeicei.contract.dto.NoteResponseDTO;
import bg.bulsi.mvr.raeicei.model.entity.Note;
import org.mapstruct.Mapper;
import org.mapstruct.Mapping;
import org.mapstruct.NullValueCheckStrategy;

import java.time.LocalDateTime;
import java.time.OffsetDateTime;
import java.time.ZoneOffset;
import java.util.List;

@Mapper(componentModel = "spring", nullValueCheckStrategy = NullValueCheckStrategy.ALWAYS)
public interface NoteMapper {

    default OffsetDateTime map(LocalDateTime dateTime) {
        return dateTime.atOffset(ZoneOffset.ofHours(0));
    }

    default LocalDateTime map(OffsetDateTime dateTime) {
        return dateTime.atZoneSameInstant(ZoneOffset.UTC).toLocalDateTime();
    }

    @Mapping(target = "outgoing", source = "isOutgoing")
    Note mapToEntity(NoteDTO dto);

    @Mapping(target = "isOutgoing", source = "outgoing")
    NoteResponseDTO mapToDto(Note entity);

    List<Note> mapToEntityList(List<NoteDTO> dtos);

    List<NoteResponseDTO> mapToDtoList(List<Note> entities);
}
