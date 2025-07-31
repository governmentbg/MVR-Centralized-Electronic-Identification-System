package bg.bulsi.mvr.raeicei.model.entity.tariif;

import lombok.experimental.FieldNameConstants;

@FieldNameConstants(onlyExplicitlyIncluded = true)
public enum TariffType {
	@FieldNameConstants.Include  DEVICE_TARIFF,
	@FieldNameConstants.Include  SERVICE_TARIFF
}
