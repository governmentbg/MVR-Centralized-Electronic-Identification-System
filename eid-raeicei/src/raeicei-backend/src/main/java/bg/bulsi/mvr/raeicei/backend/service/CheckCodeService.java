package bg.bulsi.mvr.raeicei.backend.service;

import bg.bulsi.mvr.raeicei.contract.dto.ApplicationStatus;
import bg.bulsi.mvr.raeicei.contract.dto.NoteDTO;

import java.util.UUID;

public interface CheckCodeService {

    void checkCode(String code, Boolean isOffice);
}
