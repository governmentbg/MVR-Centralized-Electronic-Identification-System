package bg.bulsi.mvr.raeicei.model.repository.view;

import java.util.List;

public interface ReportOfEidManagers {
    List<String> HEADERS = List.of("Име на организация", "ЕИК", "Уеб-сайт", "Адрес", "Електронна поща", "Информация за офиси", "Регион на офис", "Географски координати на офис", "Адрес/Населено място на офис", "Телефон за контакти на офис", "Работно време на офис");

    String getName();

    String getEikNumber();

    String getHomePage();

    String getAddress();

    String getEmail();

    String getFrontOfficeInfo();

    String getFrontOfficeRegion();

    String getFrontOfficeLatitude();

    String getFrontOfficeLongitude();

    String getFrontOfficeLocation();

    String getFrontOfficePhoneNumber();

    String getFrontOfficeWorkingHours();
}
