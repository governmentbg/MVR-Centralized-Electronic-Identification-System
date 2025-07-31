package bg.bulsi.mvr.raeicei.model.enums;

import lombok.AllArgsConstructor;
import lombok.Getter;

import java.util.Map;

@AllArgsConstructor
@Getter
public enum Region {
    BLAGOEVGRAD(Map.of(NomLanguage.BG, "Благоевград", NomLanguage.EN, "Blagoevgrad")),
    BURGAS(Map.of(NomLanguage.BG, "Бургас", NomLanguage.EN, "Burgas")),
    VARNA(Map.of(NomLanguage.BG, "Варна", NomLanguage.EN, "Varna")),
    VELIKO_TARNOVO(Map.of(NomLanguage.BG, "Велико Търново", NomLanguage.EN, "Veliko Tarnovo")),
    VIDIN(Map.of(NomLanguage.BG, "Видин", NomLanguage.EN, "Vidin")),
    VRATSA(Map.of(NomLanguage.BG, "Враца", NomLanguage.EN, "Vratsa")),
    GABROVO(Map.of(NomLanguage.BG, "Габрово", NomLanguage.EN, "Gabrovo")),
    DOBRICH(Map.of(NomLanguage.BG, "Добрич", NomLanguage.EN, "Dobrich")),
    KARDZHALI(Map.of(NomLanguage.BG, "Кърджали", NomLanguage.EN, "Kardzhali")),
    KYUSTENDIL(Map.of(NomLanguage.BG, "Кюстендил", NomLanguage.EN, "Kyustendil")),
    LOVECH(Map.of(NomLanguage.BG, "Ловеч", NomLanguage.EN, "Lovech")),
    MONTANA(Map.of(NomLanguage.BG, "Монтана", NomLanguage.EN, "Montana")),
    PAZARDZHIK(Map.of(NomLanguage.BG, "Пазарджик", NomLanguage.EN, "Pazardzhik")),
    PERNIK(Map.of(NomLanguage.BG, "Перник", NomLanguage.EN, "Pernik")),
    PLEVEN(Map.of(NomLanguage.BG, "Плевен", NomLanguage.EN, "Pleven")),
    PLOVDIV(Map.of(NomLanguage.BG, "Пловдив", NomLanguage.EN, "Plovdiv")),
    RAZGRAD(Map.of(NomLanguage.BG, "Разград", NomLanguage.EN, "Razgrad")),
    RUSE(Map.of(NomLanguage.BG, "Русе", NomLanguage.EN, "Ruse")),
    SILISTRA(Map.of(NomLanguage.BG, "Силистра", NomLanguage.EN, "Silistra")),
    SLIVEN(Map.of(NomLanguage.BG, "Сливен", NomLanguage.EN, "Sliven")),
    SMOLYAN(Map.of(NomLanguage.BG, "Смолян", NomLanguage.EN, "Smolyan")),
    SOFIA(Map.of(NomLanguage.BG, "София", NomLanguage.EN, "Sofia")),
    SOFIA_CITY(Map.of(NomLanguage.BG, "Столична", NomLanguage.EN, "Sofia City")),
    STARA_ZAGORA(Map.of(NomLanguage.BG, "Ст. Загора", NomLanguage.EN, "St. Zagora")),
    TARGOVISHTE(Map.of(NomLanguage.BG, "Търговище", NomLanguage.EN, "Targovishte")),
    HASKOVO(Map.of(NomLanguage.BG, "Хасково", NomLanguage.EN, "Haskovo")),
    SHUMEN(Map.of(NomLanguage.BG, "Шумен", NomLanguage.EN, "Shumen")),
    YAMBOL(Map.of(NomLanguage.BG, "Ямбол", NomLanguage.EN, "Yambol")),
    MVR(Map.of(NomLanguage.BG, "МВР", NomLanguage.EN, "MOI"));

    private final Map<NomLanguage, String> desciptions;
}
