package bg.bulsi.mvr.raeicei.backend.service;

import bg.bulsi.mvr.raeicei.contract.dto.*;
import bg.bulsi.mvr.raeicei.model.entity.tariif.Tariff;

import java.util.List;
import java.util.UUID;

public interface TariffService {

	Tariff getTariffByDateAndEidManagerIdAndProviderServiceId(TariffDateDTO dto, UUID providerServiceId );

	Tariff getTariffByDateAndEidManagerId(TariffDateDTO dto);

	TariffDoubleCurrencyResponseDTO getTariffDoubleCurrencyByDateAndEidManagerId(TariffDateDTO dto);

	Tariff getTariffById(UUID id);

	TariffDoubleCurrencyResponseDTO getDoubleCurrencyTariffById(UUID id);

	Tariff createTariff(TariffResponseDTO dto);

    List<TariffDoubleCurrencyResponseDTO> getAllTariffsByEidManagerId(UUID id);

    CalculateTariffResultDTO calculateTariff(CalculateTariff dto);

	TariffResponseDTO updateTariff(UUID id, TariffDTO tariffDTO, UUID eidManagerId);

    void deleteTariff(UUID id);

  //  getAllDiscountsByEidAdministratorId
}
