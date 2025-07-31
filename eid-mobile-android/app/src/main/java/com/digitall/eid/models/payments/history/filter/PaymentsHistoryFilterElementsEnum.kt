package com.digitall.eid.models.payments.history.filter

import android.os.Parcelable
import com.digitall.eid.R
import com.digitall.eid.domain.models.base.TypeEnum
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonListElementIdentifier
import kotlinx.parcelize.Parcelize

@Parcelize
enum class PaymentsHistoryFilterElementsEnum (
    override val type: String,
    val title: StringSource,
) : CommonListElementIdentifier, TypeEnum, Parcelable {
    EDIT_TEXT_PAYMENT_NUMBER("EDIT_TEXT_PAYMENT_NUMBER", StringSource(R.string.payments_history_filter_payment_number_title)),
    DATE_PICKER_CREATED_ON("DATE_PICKER_CREATED_ON", StringSource(R.string.payments_history_filter_created_on_title)),
    DATE_PICKER_VALID_UNTIL("DATE_PICKER_VALID_UNTIL", StringSource(R.string.payments_history_filter_valid_until_title)),
    SPINNER_STATUS("SPINNER_STATUS", StringSource(R.string.payments_history_filter_status_title)),
    DATE_PICKER_PAYMENT_DATE("DATE_PICKER_PAYMENT_DATE", StringSource(R.string.payments_history_filter_payment_date_title)),
    SPINNER_SUBJECT("SPINNER_SUBJECT", StringSource(R.string.payments_history_filter_subject_title)),
    SPINNER_AMOUNT("SPINNER_AMOUNT", StringSource(R.string.payments_history_filter_amount_title)),
    BUTTONS("BUTTONS", StringSource("")),
}

@Parcelize
enum class PaymentsHistoryFilterButtonsEnumUi(
    override val type: String,
    val title: StringSource,
) : CommonListElementIdentifier, TypeEnum, Parcelable {
    BUTTON_APPLY("BUTTON_APPLY", StringSource(R.string.payments_history_filter_apply_filter_button_title)),
    BUTTON_CANCEL("BUTTON_CANCEL", StringSource(R.string.payments_history_filter_clear_button_title)),
}