package bg.bulsi.mvr.raeicei.model.pan;

import org.springframework.stereotype.Component;

import bg.bulsi.mvr.pan_client.DirectEmailRequest;

@Component
public class DirectEmailFactory {
 
	public DirectEmailRequest changeApplicationStatus(String email, Object[] bodyParams) {
		return DirectEmailRequest.builder()
			.subject("Уведомление за промяна на статус на заявление")
			.body(String.format("<pre>На дата %s е променен е статуса на Вашето заявление с Nr. %s %n%n Новия статус е %s</pre>", bodyParams))
			.emailAddress(email)
			.language("bg")
			.build();
	}
	
	public DirectEmailRequest approveEidAdministrator(String email, Object[] bodyParams) {
		return DirectEmailRequest.builder()
			.subject("Уведомление за регистрация за АЕИ")
			.body(String.format("<pre>На дата %s е променен е статуса на Вашата регистрация с код. %s %n%n Новия статус е %s</pre>", bodyParams))
			.emailAddress(email)
			.language("bg")
			.build();
	}
	
	public DirectEmailRequest approveEidCenter(String email, Object[] bodyParams) {
		return DirectEmailRequest.builder()
			.subject("Уведомление за регистрация за ЦЕИ")
			.body(String.format("<pre>На дата %s е променен е статуса на Вашата регистрация с код. %s %n%n Новия статус е %s</pre>", bodyParams))
			.emailAddress(email)
			.language("bg")
			.build();
	}
}
