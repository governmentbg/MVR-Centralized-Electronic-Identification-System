package bg.bulsi.mvr.mpozei.backend.service;

import java.util.function.Consumer;

import bg.bulsi.mvr.audit_logger.dto.EventPayload;
import bg.bulsi.mvr.mpozei.contract.dto.InvalidateCitizenEidDTO;
import bg.bulsi.mvr.mpozei.contract.dto.NaifInvalidateEidResponse;
import bg.bulsi.mvr.mpozei.contract.dto.PivrIdchangesResponseDto;

public interface EidentityService {
    NaifInvalidateEidResponse invalidateCitizenEidByNaif(InvalidateCitizenEidDTO dto);

    void updateCitizenIdentifier(PivrIdchangesResponseDto dto, Consumer<EventPayload> auditEventLogger);
}
