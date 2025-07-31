package bg.bulsi.mvr.raeicei.model.pan;

import org.springframework.stereotype.Component;

import bg.bulsi.mvr.pan_client.SystemRegistrator;

import java.util.ArrayList;
import java.util.List;
import java.util.Locale;

@Component
public class SystemRegistratorImpl extends SystemRegistrator {

	@Override
	public String getSystemName() {
		return "RAEICEI";
	}

	@Override
	public List<Translation> getTranslations() {
		List<Translation> translations = new ArrayList<>();
		
		Translation translationBg = new Translation("bg", "Регистър на АЕИ и ЦЕИ");
		translations.add(translationBg);
		
		Translation translationEn = new Translation(Locale.ENGLISH.getLanguage(), "Register for AEI and CEI");
		translations.add(translationEn);
		
		return translations;
	}
}