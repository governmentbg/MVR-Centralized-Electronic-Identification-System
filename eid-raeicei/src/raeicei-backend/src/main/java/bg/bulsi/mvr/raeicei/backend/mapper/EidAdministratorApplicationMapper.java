package bg.bulsi.mvr.raeicei.backend.mapper;

import bg.bulsi.mvr.raeicei.contract.dto.*;
import bg.bulsi.mvr.raeicei.model.entity.Contact;
import bg.bulsi.mvr.raeicei.model.entity.Document;
import bg.bulsi.mvr.raeicei.model.entity.DocumentType;
import bg.bulsi.mvr.raeicei.model.entity.Note;
import bg.bulsi.mvr.raeicei.model.entity.application.ApplicationNumber;
import bg.bulsi.mvr.raeicei.model.entity.application.EidAdministratorApplication;
import bg.bulsi.mvr.raeicei.model.repository.view.EidApplicationView;
import org.mapstruct.*;
import org.mapstruct.factory.Mappers;
import org.springframework.data.domain.Page;

import java.time.LocalDateTime;
import java.time.OffsetDateTime;
import java.time.ZoneOffset;
import java.util.ArrayList;
import java.util.List;
import java.util.UUID;

@Mapper(componentModel = "spring", nullValueCheckStrategy = NullValueCheckStrategy.ALWAYS)
public abstract class EidAdministratorApplicationMapper {

    private final DocumentMapper documentMapper = Mappers.getMapper(DocumentMapper.class);

    private final NoteMapper noteMapper = Mappers.getMapper(NoteMapper.class);

    public OffsetDateTime map(LocalDateTime dateTime) {
        return dateTime.atOffset(ZoneOffset.ofHours(0));
    }

    public LocalDateTime map(OffsetDateTime dateTime) {
        return dateTime.atZoneSameInstant(ZoneOffset.UTC).toLocalDateTime();
    }

    @Mappings({
            @Mapping(source = "applicationType", target = "applicationNumber", qualifiedByName = "appType2Number"),
            @Mapping(target = "attachments", source = "attachments", qualifiedByName = "attachments2Documents"),
            @Mapping(target = "notes", source = "notes", qualifiedByName = "notes2Notes"),
    })
    public abstract EidAdministratorApplication mapToEntity(EidAdministratorApplicationDTO dto);

    public abstract EidAdministratorApplicationResponseDTO mapToDto(EidAdministratorApplication entity);

    @Mapping(source = "eIdentity", target = "EIdentity")
    public abstract Contact mapToEntity(ContactDTO dto);

    @Mapping(source = "EIdentity", target = "eIdentity")
    public abstract ContactDTO mapToDto(Contact entity);

    @Mapping(target = "isOutgoing", source = "outgoing")
    public abstract NoteResponseDTO mapToDto(Note entity);

    @Mapping(target = "isOutgoing", source = "outgoing")
    public abstract DocumentResponseDTO mapToDto(Document entity);

    public abstract EidAdministratorApplication mapToEntity(@MappingTarget EidAdministratorApplication entity, EidAdministratorApplicationDTO dto);

    public abstract List<EidAdministratorApplication> mapToEntityList(List<EidAdministratorApplicationDTO> dtos);

    public abstract List<EidAdministratorApplicationDTO> mapToDtoList(List<EidAdministratorApplication> entities);

    public abstract EidAdministratorApplicationShortDTO viewToDto(EidApplicationView view);

    public Page<EidAdministratorApplicationShortDTO> mapToDtoPage(Page<EidApplicationView> views) {
        return views.map(this::viewToDto);
    }

    public abstract List<EidAdministratorApplicationShortDTO> toDtoList(List<EidApplicationView> views);

    @Named("appType2Number")
    public static ApplicationNumber appType2Number(ApplicationType applicationType) {
        return new ApplicationNumber(bg.bulsi.mvr.raeicei.model.enums.ApplicationType.valueOf(applicationType.name()));
    }

    @Named("attachments2Documents")
    List<Document> mapToDocumentList(List<DocumentDTO> dtos) {
        if (dtos == null || dtos.isEmpty()) {
            return new ArrayList<>();
        }

        List<Document> documents = new ArrayList<>();
        dtos.forEach(dto -> {
            documents.add(documentMapper.mapToEntity(dto));
        });
        return documents;
    }

    @Named("notes2Notes")
    List<Note> mapToNoteList(List<NoteDTO> dtos) {
        if (dtos == null || dtos.isEmpty()) {
            return new ArrayList<>();
        }

        List<Note> notes = new ArrayList<>();
        dtos.forEach(dto -> {
            notes.add(noteMapper.mapToEntity(dto));
        });
        return notes;
    }

    DocumentType mapIdToDocumentType(UUID id) {
        return new DocumentType(id);
    }

    UUID mapDocumentTypeToId(DocumentType value) {
        return value.getId();
    }
}
