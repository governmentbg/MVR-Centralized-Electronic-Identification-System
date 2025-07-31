package bg.bulsi.mvr.iscei.gateway.config;

import org.apache.commons.lang3.StringUtils;
import org.apache.commons.lang3.EnumUtils;
import org.springframework.core.convert.converter.Converter;
import org.springframework.stereotype.Component;

import bg.bulsi.mvr.iscei.contract.dto.SupportedGrantType;


@Component
public class StringToSupportedGrantTypeConverter implements Converter<String, SupportedGrantType> {

	@Override
	public SupportedGrantType convert(String source) {
		if (StringUtils.isBlank(source)) {
			return null;
		}
		return EnumUtils.getEnum(SupportedGrantType.class, source.toUpperCase());
	}
}
