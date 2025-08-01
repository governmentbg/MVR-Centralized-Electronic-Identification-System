package com.digitall.eid.models.payments.history.all

import com.digitall.eid.R
import com.digitall.eid.domain.models.base.TypeEnum
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonListElementIdentifier
import kotlinx.parcelize.Parcelize

@Parcelize
enum class PaymentHistorySortCriteriaEnum(
    override val type: String,
    val title: StringSource,
) : TypeEnum, CommonListElementIdentifier {
    DEFAULT("", StringSource(R.string.payment_history_sorting_enum_by_default)),
    CREATED_ON_ASC("createdOn,asc", StringSource(R.string.payment_history_sorting_enum_created_on_asc)),
    CREATED_ON_DESC("createdOn,desc", StringSource(R.string.payment_history_sorting_enum_created_on_desc)),
    SUBJECT_ASC("subject,asc", StringSource(R.string.payment_history_sorting_enum_subject_asc)),
    SUBJECT_DESC("subject,desc", StringSource(R.string.payment_history_sorting_enum_subject_desc)),
    PAYMENT_DATE_ASC("paymentDate,asc", StringSource(R.string.payment_history_sorting_enum_payment_date_asc)),
    PAYMENT_DATE_DESC("paymentDate,desc", StringSource(R.string.payment_history_sorting_enum_payment_date_desc)),
    VALID_UNTIL_ASC("validUntil,asc", StringSource(R.string.payment_history_sorting_enum_valid_until_asc)),
    VALID_UNTIL_DESC("validUntil,desc", StringSource(R.string.payment_history_sorting_enum_valid_until_desc)),
    STATUS_ASC("status,asc", StringSource(R.string.payment_history_sorting_enum_status_asc)),
    STATUS_DESC("status,desc", StringSource(R.string.payment_history_sorting_enum_status_desc)),
    AMOUNT_ASC("amount,asc", StringSource(R.string.payment_history_sorting_enum_amount_asc)),
    AMOUNT_DESC("amount,desc", StringSource(R.string.payment_history_sorting_enum_amount_desc)),
    LAST_SYNC_ASC("lastSync,asc", StringSource(R.string.payment_history_sorting_enum_last_sync_asc)),
    LAST_SYNC_DESC("lastSync,desc", StringSource(R.string.payment_history_sorting_enum_last_sync_desc)),
}