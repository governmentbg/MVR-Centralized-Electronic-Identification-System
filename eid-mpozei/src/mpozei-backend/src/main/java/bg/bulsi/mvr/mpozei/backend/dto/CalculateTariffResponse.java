package bg.bulsi.mvr.mpozei.backend.dto;

import bg.bulsi.mvr.raeicei.contract.dto.Currency;
import lombok.Data;

@Data
public class CalculateTariffResponse {
    private Double primaryPrice;
    private Currency primaryCurrency;
    private Double secondaryPrice;
    private Currency secondaryCurrency;
    private Double devicePrimaryPrice;
    private Currency devicePrimaryCurrency;
    private Double deviceSecondaryPrice;
    private Currency deviceSecondaryCurrency;
}
