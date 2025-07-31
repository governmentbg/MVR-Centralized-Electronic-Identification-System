package bg.bulsi.mvr.raeicei.contract.dto;

import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.ToString;


@AllArgsConstructor
@Getter
@ToString
public class CalculateTariff extends CalculateTariffDTO {

	private static final long serialVersionUID = 2197854515215445703L;
	private final Boolean isOnlineService;
	
	public CalculateTariff(CalculateTariffDTO dto, Boolean isOnlineService) {
		super(dto.getEidManagerId(), dto.getDeviceId(), dto.getCurrentDate(), dto.getAge(), dto.getDisability(), dto.getProvidedServiceId());
		this.isOnlineService = isOnlineService;
	}
}
		