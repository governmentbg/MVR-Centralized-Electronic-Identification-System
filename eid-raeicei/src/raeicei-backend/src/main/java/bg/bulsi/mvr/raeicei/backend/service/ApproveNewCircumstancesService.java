package bg.bulsi.mvr.raeicei.backend.service;

import bg.bulsi.mvr.raeicei.contract.dto.DocumentNoteDTO;
import bg.bulsi.mvr.raeicei.contract.dto.NewCircumstancesStatus;
import bg.bulsi.mvr.raeicei.contract.dto.NoteAndDocumentsDTO;

import java.util.UUID;

public interface ApproveNewCircumstancesService {

    void approveEidAdministrator(UUID id, NewCircumstancesStatus status, NoteAndDocumentsDTO noteAndDocumentsDTO);

    void approveEidCenter(UUID id, NewCircumstancesStatus status, NoteAndDocumentsDTO noteAndDocumentsDTO);

    DocumentNoteDTO getDocumentsNotesForApplication(UUID applicationId);

    DocumentNoteDTO getDocumentsNotesForEidManager(UUID eidManagerId);
}
