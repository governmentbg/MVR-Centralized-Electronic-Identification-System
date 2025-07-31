package bg.bulsi.mvr.raeicei.backend.service;

import bg.bulsi.mvr.raeicei.contract.dto.ApplicationStatus;
import bg.bulsi.mvr.raeicei.contract.dto.NoteAndDocumentsDTO;
import bg.bulsi.mvr.raeicei.model.entity.application.AbstractApplication;

import java.util.UUID;

public interface ChangeApplicationStatusService {

    void changeApplicationStatus(UUID id, ApplicationStatus status, String code, NoteAndDocumentsDTO noteAndDocumentsDTO, boolean isInternal);

    AbstractApplication getApplicationById(UUID id);
}
