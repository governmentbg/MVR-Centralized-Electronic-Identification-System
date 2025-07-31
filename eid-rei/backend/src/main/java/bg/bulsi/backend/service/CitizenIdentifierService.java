package bg.bulsi.backend.service;

import bg.bulsi.mvr.reicontract.dto.CitizenDataDTO;
import bg.bulsi.mvr.reicontract.dto.EidentityDTO;
import bg.bulsi.mvr.reicontract.dto.EidentityRequestDTO;
import bg.bulsi.mvr.reicontract.dto.UpdateCitizenIdentifierDTO;
import bg.bulsi.reimodel.model.IdentifierType;
import bg.bulsi.reimodel.repository.view.CitizenIdentifierView;

import java.util.List;
import java.util.UUID;

public interface CitizenIdentifierService {
     CitizenIdentifierView getByEidentityId(UUID id);

     CitizenIdentifierView getByEidentityIdInternal(UUID id);
     CitizenIdentifierView create(CitizenDataDTO dto);
     CitizenIdentifierView update(UUID id, CitizenDataDTO dto);
     EidentityDTO updateActiveByEidentityId(UUID id, Boolean isActive);

     EidentityDTO findByNumberAndType(String number, IdentifierType type);

     EidentityDTO findByNumberAndTypeInternal(String number, IdentifierType type);

     List<EidentityDTO> findAllByNumberAndType(List<EidentityRequestDTO> eidentityIds);

     void updateCitizenIdentifier(UpdateCitizenIdentifierDTO dto);
}
