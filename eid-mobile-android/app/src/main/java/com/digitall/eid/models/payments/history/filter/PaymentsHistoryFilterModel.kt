package com.digitall.eid.models.payments.history.filter

import android.os.Parcelable
import com.digitall.eid.models.payments.history.all.PaymentAmountEnum
import com.digitall.eid.models.payments.history.all.PaymentReasonEnum
import com.digitall.eid.models.payments.history.all.PaymentStatusEnum
import kotlinx.parcelize.IgnoredOnParcel
import kotlinx.parcelize.Parcelize

@Parcelize
data class PaymentsHistoryFilterModel(
    val paymentId: String?,
    val status: PaymentStatusEnum?,
    val createdOn: Long?,
    val subject: PaymentReasonEnum?,
    val amount: PaymentAmountEnum?,
    val paymentDate: Long?,
    val validUntil: Long?,
) : Parcelable {

    @IgnoredOnParcel
    val allPropertiesAreNull: Boolean
        get() {
            val primitiveMemberProps = listOf(
                paymentId,
                status,
                createdOn,
                subject,
                amount,
                paymentDate,
                validUntil,
            )
            val arePrimitivesNotInit = primitiveMemberProps.all { member -> member == null }
            return arePrimitivesNotInit
        }
}
