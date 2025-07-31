//package bg.bulsi.mvr.raeicei.backend.service.impl;
//
//import bg.bulsi.mvr.common.exception.EntityNotFoundException;
//import bg.bulsi.mvr.common.exception.ErrorCode;
//import bg.bulsi.mvr.common.exception.ValidationMVRException;
//import bg.bulsi.mvr.raeicei.backend.mapper.DeviceTariffMapper;
//import bg.bulsi.mvr.raeicei.backend.service.DeviceService;
//import bg.bulsi.mvr.raeicei.backend.service.DeviceTariffService;
//import bg.bulsi.mvr.raeicei.backend.service.EidAdministratorService;
//import bg.bulsi.mvr.raeicei.contract.dto.DeviceTariffDTO;
//import bg.bulsi.mvr.raeicei.model.entity.DeviceTariff;
//import bg.bulsi.mvr.raeicei.model.entity.EidAdministrator;
//import bg.bulsi.mvr.raeicei.model.repository.DeviceTariffRepository;
//import lombok.RequiredArgsConstructor;
//import lombok.extern.slf4j.Slf4j;
//import org.springframework.stereotype.Service;
//
//import java.util.List;
//import java.util.UUID;
//
//import static bg.bulsi.mvr.common.exception.ErrorCode.ENTITY_NOT_FOUND;
//
//@Slf4j
//@Service
//@RequiredArgsConstructor
//public class DeviceTariffServiceImpl implements DeviceTariffService {
//
//    private final DeviceTariffRepository repository;
//    private final DeviceTariffMapper mapper;
//    private final EidAdministratorService eidAdministratorService;
//    private final DeviceService deviceService;
//
//
//    @Override
//    public DeviceTariff createDeviceTariff(DeviceTariffDTO dto) {
//        if (repository.existsByEidAdministratorIdAndStartDateGreaterThanEqual(dto.getEidAdministratorId(), dto.getStartDate())) {
//            throw new ValidationMVRException(ErrorCode.TARIFF_ALREADY_EXISTS_FOR_THE_DATE);
//        }
//
//        DeviceTariff entity = mapper.mapToEntity(dto);
//        entity.setEidAdministrator((EidAdministrator) eidAdministratorService.getEidManagerById(dto.getEidAdministratorId()));
//        entity.setDevice(deviceService.getDeviceById(dto.getDeviceId()));
//        return repository.save(entity);
//    }
//
//    @Override
//    public DeviceTariff getDiscountByDateAndEidAdministratorId(DeviceTariffDTO dto) {
//        return repository.findFirstByEidAdministratorIdAndStartDateLessThanEqualOrderByStartDateDesc(dto.getEidAdministratorId(),dto.getStartDate()).orElseThrow(()
//                -> new EntityNotFoundException(ENTITY_NOT_FOUND, dto.getEidAdministratorId().toString(), dto.getEidAdministratorId().toString()));
//    }
//
//    @Override
//    public List<DeviceTariff> getAllDiscountsByEidAdministratorId(UUID id) {
//        return repository.findAllByEidAdministratorId(id);
//    }
//}
