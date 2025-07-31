package com.digitall.eid.models.payments.history.all

import android.os.Parcelable
import androidx.annotation.ColorRes
import androidx.annotation.DrawableRes
import com.digitall.eid.R
import com.digitall.eid.domain.models.base.TypeEnum
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonListElementIdentifier
import kotlinx.parcelize.Parcelize

@Parcelize
enum class PaymentStatusEnum(
    override val type: String,
    val serverValue: String?,
    val title: StringSource,
    @param:ColorRes val colorRes: Int,
    @param:DrawableRes val iconRes: Int?,
): TypeEnum, CommonListElementIdentifier, Parcelable {
    UNKNOWN(
        type = "",
        serverValue = null,
        title = StringSource(R.string.unknown),
        colorRes = R.color.color_BF1212,
        iconRes = null
    ),
    ALL(
        type = "ALL",
        serverValue = null,
        title = StringSource(R.string.all),
        colorRes = R.color.color_018930,
        iconRes = null
    ),
    PENDING(
        type = "Pending",
        serverValue = "Pending",
        title = StringSource(R.string.payment_history_status_enum_pending),
        colorRes = R.color.color_F59E0B,
        iconRes = R.drawable.ic_clock,
    ),
    AUTHORIZED(
        type = "Authorized",
        serverValue = "Authorized",
        title = StringSource(R.string.payment_history_status_enum_authorized),
        colorRes = R.color.color_018930,
        iconRes = R.drawable.ic_multiple_check,
    ),
    ORDERED(
        type = "Ordered",
        serverValue = "Ordered",
        title = StringSource(R.string.payment_history_status_enum_ordered),
        colorRes = R.color.color_F59E0B,
        iconRes = R.drawable.ic_clock,
    ),
    PAID(
        type = "Paid",
        serverValue = "Paid",
        title = StringSource(R.string.payment_history_status_enum_paid),
        colorRes = R.color.color_018930,
        iconRes = R.drawable.ic_multiple_check,
    ),
    EXPIRED(
        type = "Expired",
        serverValue = "Expired",
        title = StringSource(R.string.payment_history_status_enum_expired),
        colorRes = R.color.color_BF1212,
        iconRes = R.drawable.ic_erase,
    ),
    CANCELED(
        type = "Canceled",
        serverValue = "Canceled",
        title = StringSource(R.string.payment_history_status_enum_canceled),
        colorRes = R.color.color_BF1212,
        iconRes = R.drawable.ic_erase,
    ),
    SUSPENDED(
        type = "Suspended",
        serverValue = "Suspended",
        title = StringSource(R.string.payment_history_status_enum_suspended),
        colorRes = R.color.color_BF1212,
        iconRes = R.drawable.ic_erase,
    ),
    IN_PROCESS(
        type = "In process",
        serverValue = "In process",
        title = StringSource(R.string.payment_history_status_enum_inprocess),
        colorRes = R.color.color_018930,
        iconRes = R.drawable.ic_multiple_check,
    ),
    TIMED_OUT(
        type = "TimedOut",
        serverValue = "Timed out",
        title = StringSource(R.string.payment_history_status_enum_timedout),
        colorRes = R.color.color_BF1212,
        iconRes = R.drawable.ic_erase,
    ),
}