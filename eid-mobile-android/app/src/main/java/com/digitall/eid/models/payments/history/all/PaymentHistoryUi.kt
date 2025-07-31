package com.digitall.eid.models.payments.history.all

import android.os.Parcelable
import com.digitall.eid.extensions.equalTo
import kotlinx.parcelize.Parcelize

@Parcelize
data class PaymentHistoryUi(
    val ePaymentId: String,
    val createdOn: String,
    val paymentDeadline: String,
    val paymentDate: String,
    val status: PaymentStatusEnum,
    val reason: PaymentReasonEnum,
    val amount: Double,
    val currency: String,
    val lastSync: String,
) : PaymentsHistoryAdapterMarker, Parcelable {

    override fun isItemSame(other: Any?): Boolean {
        return equalTo(other)
    }

    override fun isContentSame(other: Any?): Boolean {
        return equalTo(
            other,
            { ePaymentId },
            { status },
            { createdOn },
            { paymentDeadline },
            { paymentDate },
            { reason },
            { amount },
            { currency },
            { lastSync }
        )
    }
}