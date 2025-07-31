package bg.bulsi.mvr.mpozei.model.pan;

import java.util.ArrayList;
import java.util.List;
import java.util.Locale;

import org.springframework.stereotype.Component;

import bg.bulsi.mvr.pan_client.SystemRegistrator;

@Component
public class SystemRegistratorImpl extends SystemRegistrator {

	@Override
	public String getSystemName() {
		return "MPOZEI";
	}

	@Override
	public List<Translation> getTranslations() {
		List<Translation> translations = new ArrayList<>();
		
		Translation translationBg = new Translation("bg", "Приемане и обработка на заявления за електронна идентичност");
		translations.add(translationBg);
		
		Translation translationEn = new Translation(Locale.ENGLISH.getLanguage(), "Acceptance and processing of electronic identity applications");
		translations.add(translationEn);
		
		return translations;
	}
}
