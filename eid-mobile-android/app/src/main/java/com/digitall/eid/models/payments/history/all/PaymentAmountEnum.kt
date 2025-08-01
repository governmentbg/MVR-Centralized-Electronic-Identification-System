package com.digitall.eid.models.payments.history.all

import android.os.Parcelable
import com.digitall.eid.R
import com.digitall.eid.domain.models.base.TypeEnum
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonListElementIdentifier
import kotlinx.parcelize.Parcelize

@Parcelize
enum class PaymentAmountEnum(
    override val type: String,
    val serverValue: String?,
    val title: StringSource,
    val amount: PaymentAmountUnit,
) : TypeEnum, CommonListElementIdentifier {
    ALL(
        type = "ALL",
        serverValue = null,
        title = StringSource(R.string.all),
        amount = PaymentAmountUnit.Integer(Int.MAX_VALUE)
    ),
    BELOW(
        type = "BELOW",
        serverValue = null,
        title = StringSource(resId = R.string.payment_history_amount_enum_below, formatArgs = listOf("25")),
        amount = PaymentAmountUnit.Integer(25)
    ),
    BETWEEN(
        type = "BETWEEN",
        serverValue = null,
        title = StringSource(resId = R.string.payment_history_amount_enum_between, formatArgs = listOf("25", "50")),
        amount = PaymentAmountUnit.IntegerRange(25..50)
    ),
    OVER(
        type = "OVER",
        serverValue = null,
        title = StringSource(resId = R.string.payment_history_amount_enum_over, formatArgs = listOf("50")),
        amount = PaymentAmountUnit.Integer(50)
    )
}

@Parcelize
sealed class PaymentAmountUnit: Parcelable {
    class IntegerRange(val value: IntRange) : PaymentAmountUnit()
    class Integer(val value: Int) : PaymentAmountUnit()
}