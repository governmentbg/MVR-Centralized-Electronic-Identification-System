package bg.bulsi.mvr.mpozei.model.repository.view;

public interface ApplicationReportByOperators {
    String[] HEADERS = {"Оператор", "Подадено", "Подписано", "Платено", "Одобрено", "Генериран сертификат", "Приключено", "Отказано"};

    String getOperatorUsername();

    int getSubmitted();

    int getSigned();

    int getPaid();

    int getApproved();

    int getGeneratedCertificate();

    int getCompleted();

    int getDenied();
}
