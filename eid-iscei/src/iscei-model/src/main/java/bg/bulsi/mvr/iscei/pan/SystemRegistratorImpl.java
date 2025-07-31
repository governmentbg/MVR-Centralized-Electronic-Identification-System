package bg.bulsi.mvr.iscei.pan;

import org.springframework.stereotype.Component;

import bg.bulsi.mvr.pan_client.SystemRegistrator;

import java.util.ArrayList;
import java.util.List;
import java.util.Locale;

@Component
public class SystemRegistratorImpl extends SystemRegistrator {

	@Override
	public String getSystemName() {
		return "ISCEI";
	}

	@Override
	public List<Translation> getTranslations() {
		List<Translation> translations = new ArrayList<>();
		
		Translation translationBg = new Translation("bg", "Център за електронна идентификация");
		translations.add(translationBg);
		
		Translation translationEn = new Translation(Locale.ENGLISH.getLanguage(), "Center for electronic identification");
		translations.add(translationEn);
		
		return translations;
	}
}