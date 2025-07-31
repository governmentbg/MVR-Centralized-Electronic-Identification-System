package bg.bulsi.mvr.raeicei.backend.service.impl;

import bg.bulsi.mvr.common.exception.EntityNotFoundException;
import bg.bulsi.mvr.common.exception.ErrorCode;
import bg.bulsi.mvr.common.exception.ValidationMVRException;
import bg.bulsi.mvr.raeicei.backend.mapper.EidManagerFrontOfficeMapper;
import bg.bulsi.mvr.raeicei.backend.service.DeviceService;
import bg.bulsi.mvr.raeicei.backend.service.EidManagerFrontOfficeService;
import bg.bulsi.mvr.raeicei.contract.dto.EidManagerFrontOfficeDTO;
import bg.bulsi.mvr.raeicei.contract.dto.EidManagerFrontOfficeResponseDTO;
import bg.bulsi.mvr.raeicei.contract.dto.EidManagerStatus;
import bg.bulsi.mvr.raeicei.contract.dto.OfficesByRegionDTO;
import bg.bulsi.mvr.raeicei.model.entity.EidManager;
import bg.bulsi.mvr.raeicei.model.entity.EidManagerFrontOffice;
import bg.bulsi.mvr.raeicei.model.enums.Region;
import bg.bulsi.mvr.raeicei.model.repository.EidManagerFrontOfficeRepository;
import bg.bulsi.mvr.raeicei.model.repository.EidManagerRepository;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Service;
import org.springframework.util.StringUtils;

import java.security.SecureRandom;
import java.util.List;
import java.util.UUID;

import static bg.bulsi.mvr.common.exception.ErrorCode.*;

@Slf4j
@Service
@RequiredArgsConstructor
public class EidManagerFrontOfficeServiceImpl implements EidManagerFrontOfficeService {

    private final EidManagerFrontOfficeRepository repository;
    private final DeviceService deviceService;
    private final EidManagerFrontOfficeMapper mapper;
    private final EidManagerRepository eidManagerRepository;

    @Value("${latitude.min-latitude}")
    private Double minLatitude;

    @Value("${latitude.max-latitude}")
    private Double maxLatitude;

    @Value("${longitude.min-longitude}")
    private Double minLongitude;

    @Value("${longitude.max-longitude}")
    private Double maxLongitude;

    @Value("${mvr-uuid.aei}")
    private UUID MVR_AEI_UUID;

    @Value("${mvr-uuid.cei}")
    private UUID MVR_CEI_UUID;

    @Override
    public EidManagerFrontOffice getEidManagerFrontOfficeById(UUID id) {
        return repository.findById(id).orElseThrow(() -> new EntityNotFoundException(ADMINISTRATOR_FRONT_OFFICE_NOT_FOUND, id.toString(), id.toString()));
    }

    @Override
    public List<EidManagerFrontOffice> getAllEidManagerFrontOfficesByEidManagerId(UUID eidManagerId) {
        if (!eidManagerRepository.existsById(eidManagerId)) {
            throw new EntityNotFoundException(ErrorCode.ENTITY_NOT_FOUND, "EidManager", eidManagerId.toString());
        }

        return repository.findAllByEidManagerIdAndIsActive(eidManagerId, true);
    }

    @Override
    public EidManagerFrontOffice createEidManagerFrontOffice(EidManagerFrontOfficeResponseDTO dto) {
        validateAdministratorFrontOfficeResponseDTO(dto);

        if (dto.getCode() == null) {
            String officeCode = generateOfficeCode();
            officeCode = checkGeneratedOfficeCode(officeCode);
            dto.setCode(officeCode);
        }

        EidManagerFrontOffice entity = mapper.mapToEntity(dto);

        EidManager eidManager = getEidManagerById(dto.getEidManagerId());

        checkEidManagerCurrentStatus(eidManager);

        if (!eidManager.getId().equals(MVR_AEI_UUID) && !eidManager.getId().equals(MVR_CEI_UUID)) {
            eidManager.setManagerStatus(EidManagerStatus.PENDING_ATTACHMENTS);
        }

        entity.setEidManager(eidManager);
        entity.setIsActive(null);

        return repository.save(entity);
    }

    @Override
    public EidManagerFrontOffice updateEidManagerFrontOffice(EidManagerFrontOfficeDTO dto, UUID eidManagerId) {
        validateAdministratorFrontOfficeDTO(dto);

        EidManagerFrontOffice entity = getEidManagerFrontOfficeById(dto.getId());
        entity = mapper.mapToEntity(entity, dto);
        entity.setIsActive(null);

        EidManager eidManager = getEidManagerById(eidManagerId);

        checkEidManagerCurrentStatus(eidManager);

        if (!eidManager.getId().equals(MVR_AEI_UUID) && !eidManager.getId().equals(MVR_CEI_UUID)) {
            eidManager.setManagerStatus(EidManagerStatus.PENDING_ATTACHMENTS);
        }

        return repository.save(entity);
    }

    @Override
    public EidManagerFrontOffice getEidManagerFrontOfficeByName(String name) {
        return repository.findByName(name).orElseThrow(() -> new EntityNotFoundException(ADMINISTRATOR_FRONT_OFFICE_NOT_FOUND_BY_NAME, name));
    }

    @Override
    public List<EidManagerFrontOfficeDTO> getAllEidManagerFrontOfficesByRegion(OfficesByRegionDTO dto) {
        return repository.findAllByRegionAndCreateDateBetween(Region.valueOf(dto.getRegion()), dto.getFrom().atStartOfDay(), dto.getTo().atStartOfDay()).stream().map(mapper::mapToDto).toList();
    }

    private void validateAdministratorFrontOfficeDTO(EidManagerFrontOfficeDTO dto) {
        if (!StringUtils.hasText(dto.getName())) {
            throw new ValidationMVRException(ErrorCode.ADMINISTRATOR_FRONT_OFFICE_NAME_REQUIRED);
        }
        if (!StringUtils.hasText(dto.getLocation())) {
            throw new ValidationMVRException(ErrorCode.ADMINISTRATOR_FRONT_OFFICE_LOCATION_REQUIRED);
        }
        if (!StringUtils.hasText(dto.getContact())) {
            throw new ValidationMVRException(ErrorCode.ADMINISTRATOR_FRONT_OFFICE_CONTACT_REQUIRED);
        }
    }

    private void validateAdministratorFrontOfficeResponseDTO(EidManagerFrontOfficeResponseDTO dto) {
        if (!StringUtils.hasText(dto.getName())) {
            throw new ValidationMVRException(ErrorCode.ADMINISTRATOR_FRONT_OFFICE_NAME_REQUIRED);
        }
        if (dto.getEidManagerId() == null) {
            throw new ValidationMVRException(ErrorCode.ADMINISTRATOR_FRONT_OFFICE_EID_ADMINISTRATOR_ID_REQUIRED);
        }
        if (!StringUtils.hasText(dto.getLocation())) {
            throw new ValidationMVRException(ErrorCode.ADMINISTRATOR_FRONT_OFFICE_LOCATION_REQUIRED);
        }
        if (!StringUtils.hasText(dto.getContact())) {
            throw new ValidationMVRException(ErrorCode.ADMINISTRATOR_FRONT_OFFICE_CONTACT_REQUIRED);
        }

//        double latitude = 0.0;
//        double longitude = 0.0;
//
//        try {
//            latitude = Double.parseDouble(dto.getLatitude());
//            longitude = Double.parseDouble(dto.getLongitude());
//        } catch (Exception e) {
//            throw new ValidationMVRException(ErrorCode.COORDINATES_MUST_BE_DECIMAL_NUMBER);
//        }
//
//
//        if (latitude < minLatitude || latitude > maxLatitude || longitude < minLongitude || longitude > maxLongitude) {
//            throw new ValidationMVRException(ErrorCode.COORDINATES_MUST_BE_WITHIN_BULGARIA);
//        }
    }

    @Override
    public void deleteEidManagerFrontOffice(UUID id) {
        EidManagerFrontOffice office = getEidManagerFrontOfficeById(id);

        EidManager eidManager = eidManagerRepository.findById(office.getEidManager().getId()).orElseThrow(() -> new EntityNotFoundException(ErrorCode.ENTITY_NOT_FOUND, "EidManager", id.toString()));

        checkEidManagerCurrentStatus(eidManager);

        office.setIsActive(false);

        eidManager.getEidManagerFrontOffices().remove(office);
    }

    public String generateOfficeCode() {
        String CHARACTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        SecureRandom RANDOM = new SecureRandom();

        StringBuilder generatedOfficeCode = new StringBuilder(4);
        for (int i = 0; i < 4; i++) {
            int index = RANDOM.nextInt(CHARACTERS.length());
            generatedOfficeCode.append(CHARACTERS.charAt(index));
        }
        return generatedOfficeCode.toString();
    }

    public String checkGeneratedOfficeCode(String generatedOfficeCode) {
        while (repository.existsByCode(generatedOfficeCode)) {
            generatedOfficeCode = generateOfficeCode();
        }
        return generatedOfficeCode;
    }

    private EidManager getEidManagerById(UUID eidManagerId) {
        return eidManagerRepository.findById(eidManagerId).orElseThrow(() ->
                new EntityNotFoundException(EID_ADMINISTRATOR_NOT_FOUND, eidManagerId.toString(), eidManagerId.toString()));
    }

    private static void checkEidManagerCurrentStatus(EidManager eidManager) {
        if (EidManagerStatus.STOP.equals(eidManager.getManagerStatus()) || EidManagerStatus.SUSPENDED.equals(eidManager.getManagerStatus()) || EidManagerStatus.IN_REVIEW.equals(eidManager.getManagerStatus())) {
            throw new ValidationMVRException(ErrorCode.WRONG_CURRENT_EID_MANAGER_STATUS);
        }
    }
}
