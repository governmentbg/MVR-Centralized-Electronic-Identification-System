package bg.bulsi.mvr.raeicei.backend.service;

import bg.bulsi.mvr.raeicei.contract.dto.DiscountDTO;
import bg.bulsi.mvr.raeicei.contract.dto.DiscountDateDTO;
import bg.bulsi.mvr.raeicei.contract.dto.DiscountDoubleCurrencyResponseDTO;
import bg.bulsi.mvr.raeicei.contract.dto.DiscountResponseDTO;
import bg.bulsi.mvr.raeicei.model.entity.Discount;

import java.util.List;
import java.util.UUID;

public interface DiscountService {

    Discount getDiscountByDateAndEidManagerId(DiscountDateDTO dto);

    Discount getDiscountById(UUID id);

    DiscountDoubleCurrencyResponseDTO getDoubleCurrencyDiscountById(UUID id);

    Discount createDiscount(DiscountResponseDTO dto);

    List<Discount> getAllDiscountsByEidManagerId(UUID id);

    DiscountResponseDTO updateDiscount(UUID id, DiscountDTO discountDTO, UUID eidManagerId);

    void deleteDiscount(UUID id);
}
