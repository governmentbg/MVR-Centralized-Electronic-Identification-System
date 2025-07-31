package bg.bulsi.mvr.raeicei.backend.service.impl;

import bg.bulsi.mvr.common.exception.EntityNotFoundException;
import bg.bulsi.mvr.common.exception.ErrorCode;
import bg.bulsi.mvr.common.exception.ValidationMVRException;
import bg.bulsi.mvr.raeicei.backend.mapper.TariffMapper;
import bg.bulsi.mvr.raeicei.backend.service.ProvidedServiceService;
import bg.bulsi.mvr.raeicei.backend.service.TariffService;
import bg.bulsi.mvr.raeicei.contract.dto.*;
import bg.bulsi.mvr.raeicei.model.entity.Discount;
import bg.bulsi.mvr.raeicei.model.entity.EidManager;
import bg.bulsi.mvr.raeicei.model.entity.providedservice.ProvidedService;
import bg.bulsi.mvr.raeicei.model.entity.tariif.DeviceTariff;
import bg.bulsi.mvr.raeicei.model.entity.tariif.ServiceTariff;
import bg.bulsi.mvr.raeicei.model.entity.tariif.Tariff;
import bg.bulsi.mvr.raeicei.model.enums.Currency;
import bg.bulsi.mvr.raeicei.model.repository.*;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Service;

import java.math.BigDecimal;
import java.math.RoundingMode;
import java.time.LocalDate;
import java.util.List;
import java.util.Optional;
import java.util.UUID;

import static bg.bulsi.mvr.common.exception.ErrorCode.EID_ADMINISTRATOR_NOT_FOUND;
import static bg.bulsi.mvr.common.exception.ErrorCode.ENTITY_NOT_FOUND;

@Slf4j
@Service
@RequiredArgsConstructor
public class TariffServiceImpl implements TariffService {

    private static final String ISSUE_APPLICATION_SERVICE_NAME_LATIN = "Issue Application";

    private final TariffRepository tariffRepository;
    private final ServiceTariffRepository serviceTariffRepository;

    private final DeviceRepository deviceRepository;
    private final ProvidedServiceRepository providedServiceRepository;

    private final ProvidedServiceService providedServiceService;
    private final DiscountRepository discountRepository;
    private final EidManagerRepository eidManagerRepository;
    private final TariffMapper mapper;

    @Value("${exchange-rates.bgn-to-eur}")
    private Double bgnToEuroExchangeRate;

    @Value("${exchange-rates.eur-to-bgn}")
    private Double euroToBgnExchangeRate;

    @Override
    public Tariff getTariffByDateAndEidManagerId(TariffDateDTO dto) {
        Tariff entity = tariffRepository
                .findFirstByEidManagerIdAndStartDateLessThanEqualAndIsActiveOrderByStartDateDesc(dto.getEidManagerId(),
                        dto.getDate(), true)
                .orElseThrow(() -> new EntityNotFoundException(ENTITY_NOT_FOUND, dto.getEidManagerId().toString(),
                        dto.getEidManagerId().toString()));
        return entity;
    }

    @Override
    public TariffDoubleCurrencyResponseDTO getTariffDoubleCurrencyByDateAndEidManagerId(TariffDateDTO dto) {
        Tariff entity = tariffRepository
                .findFirstByEidManagerIdAndStartDateLessThanEqualAndIsActiveOrderByStartDateDesc(dto.getEidManagerId(),
                        dto.getDate(), true)
                .orElseThrow(() -> new EntityNotFoundException(ENTITY_NOT_FOUND, dto.getEidManagerId().toString(),
                        dto.getEidManagerId().toString()));
        TariffDoubleCurrencyResponseDTO responseDTO = mapper.mapToTariffDoubleCurrencyResponseDTO(entity);
        setSecondaryPriceAndCurrency(responseDTO);
        return responseDTO;
    }

    @Override
    public Tariff getTariffById(UUID id) {
        return tariffRepository.findById(id).orElseThrow(() -> new EntityNotFoundException(ErrorCode.ENTITY_NOT_FOUND, "Tariff", id.toString()));
    }

    @Override
    public TariffDoubleCurrencyResponseDTO getDoubleCurrencyTariffById(UUID id) {
        Tariff entity = tariffRepository.findById(id).orElseThrow(() -> new EntityNotFoundException(ErrorCode.ENTITY_NOT_FOUND, "Tariff", id.toString()));
        TariffDoubleCurrencyResponseDTO responseDTO = mapper.mapToTariffDoubleCurrencyResponseDTO(entity);
        setSecondaryPriceAndCurrency(responseDTO);
        return responseDTO;
    }

    @Override
    public Tariff createTariff(TariffResponseDTO dto) {
        if (tariffRepository.existsByEidManagerIdAndStartDateGreaterThanEqual(dto.getEidManagerId(),
                dto.getStartDate())) {
            throw new ValidationMVRException(ErrorCode.TARIFF_ALREADY_EXISTS_FOR_THE_DATE);
        }

        if (dto.getProvidedServiceId() == null) {
            throw new ValidationMVRException(ErrorCode.INVALID_TARIFF_TYPE);
        }

        ServiceTariff entity = null;
        entity = mapper.mapToServiceTariff(dto);

        ProvidedService providedService = providedServiceRepository.findById(dto.getProvidedServiceId()).orElseThrow();
        entity.setProvidedService(providedService);
        entity.setIsActive(true);

        EidManager eidManager = getEidManagerById(dto.getEidManagerId());

        checkEidManagerCurrentStatus(eidManager);

        entity.setEidManager(eidManager);

        // TariffView entity = mapper.mapToTariffView(dto);
        // entity.setEidManager(eidAdministratorService.getEidManagerById(dto.getEidManagerId()));
        entity = tariffRepository.save(entity);

        return entity;
    }

    @Override
    public List<TariffDoubleCurrencyResponseDTO> getAllTariffsByEidManagerId(UUID id) {
        List<Tariff> entities = tariffRepository.findAllByEidManagerIdAndIsActive(id, true);
        List<TariffDoubleCurrencyResponseDTO> dtos = mapper.mapToListDoubleCurrencyResponseDTO(entities);
        dtos.forEach(this::setSecondaryPriceAndCurrency);
        return dtos;
    }

//  @Override
//  public List<Tariff> getAllDiscountsByEidAdministratorId(UUID id) {
//      return repository.findAllByEidAdministratorId(id);
//  }

    @Override
    public CalculateTariffResultDTO calculateTariff(CalculateTariff dto) {
        TariffDateDTO tariffDateDTO = new TariffDateDTO().eidManagerId(dto.getEidManagerId())
                .date(dto.getCurrentDate());
        Tariff tariff = new ServiceTariff(null, UUID.randomUUID(), LocalDate.now(), BigDecimal.ZERO, Currency.BGN,
                null, true);
        double discountAmount = 0;
        try {
            tariff = getTariffByDateAndEidManagerIdAndProviderServiceId(tariffDateDTO, dto.getProvidedServiceId());
            log.debug("Tariff price: {}", tariff.getPrice());
            discountAmount = discountRepository.findDiscount(dto.getEidManagerId(), dto.getCurrentDate(),
                            dto.getAge().intValue(), dto.getDisability(), dto.getIsOnlineService(), dto.getProvidedServiceId()).map(Discount::getValue)
                    .orElse(0.0);
            log.debug("Discount amount: {}", discountAmount);

        } catch (Exception e) {
            log.error("No tariff Found!", e);
        }
        ProvidedService providedService = this.providedServiceService
                .getProvidedServiceById(dto.getProvidedServiceId());

        CalculateTariffResultDTO result = new CalculateTariffResultDTO();

        // TODO: Discount should be applied to service not global For EidAdministrator
        BigDecimal tariffValue = tariff.getPrice();
        // Issue Application is 415fbe9d-3a12-4fbf-9c50-1114056757cc
        if (tariff instanceof ServiceTariff st
                && (st.getProvidedService() != null && ISSUE_APPLICATION_SERVICE_NAME_LATIN.equals(st.getProvidedService().getNameLatin()))
                && dto.getDeviceId() != null) {
            Optional<DeviceTariff> dt = tariffRepository
                    .findByDeviceIdAndEidManagerId(dto.getDeviceId(), dto.getEidManagerId());

            dt.ifPresentOrElse(value -> {
                        if (Currency.BGN.equals(value.getCurrency())) {
                            result.setDevicePrimaryPrice(value.getPrice().doubleValue());
                            result.setDevicePrimaryCurrency(bg.bulsi.mvr.raeicei.contract.dto.Currency.BGN);
                            result.setDeviceSecondaryPrice(BigDecimal.valueOf(result.getDevicePrimaryPrice() * bgnToEuroExchangeRate)
                                    .setScale(2, RoundingMode.HALF_UP)
                                    .doubleValue());
                            result.setDeviceSecondaryCurrency(bg.bulsi.mvr.raeicei.contract.dto.Currency.EUR);
                        } else {
                            result.setDevicePrimaryPrice(value.getPrice().doubleValue());
                            result.setDevicePrimaryCurrency(bg.bulsi.mvr.raeicei.contract.dto.Currency.EUR);
                            result.setDeviceSecondaryPrice(BigDecimal.valueOf(result.getDevicePrimaryPrice() * euroToBgnExchangeRate)
                                    .setScale(2, RoundingMode.HALF_UP)
                                    .doubleValue());
                            result.setDeviceSecondaryCurrency(bg.bulsi.mvr.raeicei.contract.dto.Currency.BGN);
                        }
                    }
                    , () -> {
                        // TODO: Swap devicePrimaryCurrency and deviceSecondaryCurrency when we are using euro
                        result.setDevicePrimaryPrice(0.0d);
                        result.setDevicePrimaryCurrency(bg.bulsi.mvr.raeicei.contract.dto.Currency.BGN);
                        result.setDeviceSecondaryPrice(0.0d);
                        result.setDeviceSecondaryCurrency(bg.bulsi.mvr.raeicei.contract.dto.Currency.EUR);
                    });
        }

        if (Currency.BGN.equals(tariff.getCurrency())) {
            result.setPrimaryPrice(tariffValue.subtract(BigDecimal.valueOf(discountAmount))
                    .setScale(2, RoundingMode.HALF_UP)
                    .doubleValue());
            result.setPrimaryCurrency(bg.bulsi.mvr.raeicei.contract.dto.Currency.BGN);
            result.setSecondaryPrice(BigDecimal.valueOf(result.getPrimaryPrice() * bgnToEuroExchangeRate)
                    .setScale(2, RoundingMode.HALF_UP)
                    .doubleValue());
            result.setSecondaryCurrency(bg.bulsi.mvr.raeicei.contract.dto.Currency.EUR);
        } else {
            result.setPrimaryPrice(tariffValue.subtract(BigDecimal.valueOf(discountAmount))
                    .setScale(2, RoundingMode.HALF_UP)
                    .doubleValue());
            result.setPrimaryCurrency(bg.bulsi.mvr.raeicei.contract.dto.Currency.EUR);
            result.setSecondaryPrice(BigDecimal.valueOf(result.getPrimaryPrice() * euroToBgnExchangeRate)
                    .setScale(2, RoundingMode.HALF_UP)
                    .doubleValue());
            result.setSecondaryCurrency(bg.bulsi.mvr.raeicei.contract.dto.Currency.BGN);
        }

        return result;
    }

    @Override
    public ServiceTariff getTariffByDateAndEidManagerIdAndProviderServiceId(TariffDateDTO dto, UUID providerServiceId) {

        ServiceTariff entity = serviceTariffRepository
                .findFirstByEidManagerIdAndProvidedServiceIdAndStartDateLessThanEqualOrderByStartDateDesc(
                        dto.getEidManagerId(), providerServiceId, dto.getDate())
                .orElseThrow(() -> new EntityNotFoundException(ENTITY_NOT_FOUND, dto.getEidManagerId().toString(),
                        dto.getEidManagerId().toString()));
        return entity;
    }

    @Override
    public TariffResponseDTO updateTariff(UUID id, TariffDTO tariffDTO, UUID eidManagerId) {
        Tariff entity = getTariffById(id);

        EidManager eidManager = getEidManagerById(eidManagerId);

        checkEidManagerCurrentStatus(eidManager);

        entity = mapper.mapToEntity(entity, tariffDTO);
        entity.setIsActive(true);
        tariffRepository.save(entity);
        return mapper.mapToTariffDto(entity);
    }

    @Override
    public void deleteTariff(UUID id) {
        Tariff tariff = getTariffById(id);

        EidManager eidManager = getEidManagerById(tariff.getEidManager().getId());

        checkEidManagerCurrentStatus(eidManager);

        tariff.setIsActive(false);
    }

    private void setSecondaryPriceAndCurrency(TariffDoubleCurrencyResponseDTO dto) {
        if (bg.bulsi.mvr.raeicei.contract.dto.Currency.BGN.equals(dto.getPrimaryCurrency())) {
            dto.setSecondaryPrice(BigDecimal.valueOf(dto.getPrimaryPrice() * bgnToEuroExchangeRate)
                    .setScale(2, RoundingMode.HALF_UP)
                    .doubleValue());
            dto.setSecondaryCurrency(bg.bulsi.mvr.raeicei.contract.dto.Currency.EUR);
        } else {
            dto.setSecondaryPrice(BigDecimal.valueOf(dto.getPrimaryPrice() * euroToBgnExchangeRate)
                    .setScale(2, RoundingMode.HALF_UP)
                    .doubleValue());
            dto.setSecondaryCurrency(bg.bulsi.mvr.raeicei.contract.dto.Currency.BGN);
        }
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
