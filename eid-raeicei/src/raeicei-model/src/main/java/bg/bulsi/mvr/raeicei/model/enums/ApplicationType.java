package bg.bulsi.mvr.raeicei.model.enums;

import lombok.Getter;

@Getter
public enum ApplicationType {
	/*
	 * 8.3.1. Процес по вписване на АЕИ и ЦЕИ в РАЕИЦЕИ 
	 * 8.3.2. Процес по спиране на регистрация на АЕИ/ЦЕИ с РАЕИЦЕИ 
	 * 8.3.3. Процес по възобновяване на регистрация на АЕИ/ЦЕИ с РАЕИЦЕИ 
	 * 8.3.4. Процес по прекратяване (заличаване) на регистрация на АЕИ/ЦЕИ с
	 */
	  REGISTER("ВП."),
	  RESUME("ВС."),
	  REVOKE("ПР."),
	  STOP("СП.");

	ApplicationType(String shortPrefix) {
		prefix= shortPrefix;
	}
	String prefix;
}
