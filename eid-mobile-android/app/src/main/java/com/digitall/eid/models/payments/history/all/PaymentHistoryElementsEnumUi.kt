package com.digitall.eid.models.payments.history.all

import com.digitall.eid.domain.models.base.TypeEnum
import com.digitall.eid.models.list.CommonListElementIdentifier
import kotlinx.parcelize.Parcelize

@Parcelize
enum class PaymentHistoryElementsEnumUi (
    override val type: String,
) : CommonListElementIdentifier, TypeEnum {
    SPINNER_SORTING_CRITERIA("SPINNER_SORTING_CRITERIA"),
}