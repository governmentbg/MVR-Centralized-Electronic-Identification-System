package bg.bulsi.mvr.raeicei.backend.mapper;

import bg.bulsi.mvr.raeicei.contract.dto.Currency;
import bg.bulsi.mvr.raeicei.contract.dto.TariffDTO;
import bg.bulsi.mvr.raeicei.contract.dto.TariffDoubleCurrencyResponseDTO;
import bg.bulsi.mvr.raeicei.contract.dto.TariffResponseDTO;
import bg.bulsi.mvr.raeicei.model.entity.tariif.DeviceTariff;
import bg.bulsi.mvr.raeicei.model.entity.tariif.ServiceTariff;
import bg.bulsi.mvr.raeicei.model.entity.tariif.Tariff;
import org.mapstruct.Mapper;
import org.mapstruct.Mapping;
import org.mapstruct.MappingTarget;
import org.mapstruct.NullValueCheckStrategy;

import java.util.List;

@Mapper(componentModel = "spring", nullValueCheckStrategy = NullValueCheckStrategy.ALWAYS)
public abstract class TariffMapper {

//  @Mapping(target = "eidManager", ignore = true)
//  public abstract TariffView mapToTariffView(TariffDTO dto);

  public abstract DeviceTariff mapToDeviceTariff(TariffDTO dto);

  public abstract ServiceTariff mapToServiceTariff(TariffResponseDTO dto);

	public TariffResponseDTO mapToTariffDto(Tariff tariff) {
		TariffResponseDTO tariffDTO = new TariffResponseDTO();
		tariffDTO.setId(tariff.getId());
		tariffDTO.setEidManagerId(tariff.getEidManager().getId());
		tariffDTO.setPrice(tariff.getPrice().doubleValue());
		tariffDTO.setCurrency(TariffResponseDTO.CurrencyEnum.valueOf(tariff.getCurrency().name()));
		tariffDTO.setStartDate(tariff.getStartDate());

		if (tariff instanceof ServiceTariff st) {
			tariffDTO.setProvidedServiceId(st.getProvidedService().getId());
		}

		return tariffDTO;
	}

    public TariffDoubleCurrencyResponseDTO mapToTariffDoubleCurrencyResponseDTO(Tariff tariff) {
        TariffDoubleCurrencyResponseDTO tariffDTO = new TariffDoubleCurrencyResponseDTO();
        tariffDTO.setId(tariff.getId());
        tariffDTO.setEidManagerId(tariff.getEidManager().getId());
        tariffDTO.setPrimaryPrice(tariff.getPrice().doubleValue());
        tariffDTO.setPrimaryCurrency(Currency.valueOf(tariff.getCurrency().toString()));
        tariffDTO.setStartDate(tariff.getStartDate());

        if (tariff instanceof ServiceTariff st) {
            tariffDTO.setProvidedServiceId(st.getProvidedService().getId());
        }

        return tariffDTO;
    }

//  public abstract TariffDTO mapFromViewToTariffDto(TariffView dto);

  @Mapping(target = "eidManager", ignore = true)
  public abstract List<TariffDTO> mapToEntityListDto(List<Tariff> dtos);

  @Mapping(target = "eidManager", ignore = true)
  public abstract List<TariffResponseDTO> mapToEntityListResponseDto(List<Tariff> dtos);

  @Mapping(target = "eidManager", ignore = true)
  public abstract List<TariffDoubleCurrencyResponseDTO> mapToListDoubleCurrencyResponseDTO(List<Tariff> dtos);

//    @Mapping(target = "eidManager", ignore = true)
//    public abstract TariffView mapToTariffView(TariffDTO dto);
//    
//    @Mapping(target = "eidManager", ignore = true)
//    public  abstract  TariffView mapToDeviceTariff(TariffDTO dto);
//    
//    @Mapping(target = "eidManagerId", ignore = true)
//    public  abstract  TariffView mapToDto(Tariff entity);
//    
//    @Mapping(target = "eidManager", ignore = true)
//    public abstract  Tariff mapToEntity(@MappingTarget Tariff entity, TariffView dto);
//    
//    @Mapping(target = "eidManager", ignore = true)
//    public abstract List<Tariff> mapToEntityList(List<TariffView> dtos);
//    
//    @Mapping(target = "eidManagerId", ignore = true)
//    public  abstract  List<TariffView> mapToDtoList(List<Tariff> entities);

//  @Mapping(target = "device", ignore = true)
//  @Mapping(target = "eidAdministrator", ignore = true)
//  Tariff mapToEntity(DeviceTariffDTO dto);

	@Mapping(target = "id", ignore = true)
	public abstract Tariff mapToEntity(@MappingTarget Tariff entity, TariffDTO dto);
}
