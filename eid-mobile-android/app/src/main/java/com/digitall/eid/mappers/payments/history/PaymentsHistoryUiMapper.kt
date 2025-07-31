package com.digitall.eid.mappers.payments.history

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.domain.extensions.getEnumValue
import com.digitall.eid.domain.models.payments.history.PaymentHistoryModel
import com.digitall.eid.models.payments.history.all.PaymentHistoryUi
import com.digitall.eid.models.payments.history.all.PaymentReasonEnum
import com.digitall.eid.models.payments.history.all.PaymentStatusEnum

class PaymentsHistoryUiMapper : BaseMapper<PaymentHistoryModel, PaymentHistoryUi>() {

    override fun map(from: PaymentHistoryModel): PaymentHistoryUi {
        return with(from) {
            PaymentHistoryUi(
                ePaymentId = paymentId ?: "",
                createdOn = createdOn ?: "",
                paymentDeadline = paymentDeadline ?: "",
                paymentDate = paymentDate ?: "",
                status = getEnumValue<PaymentStatusEnum>(status ?: "") ?: PaymentStatusEnum.UNKNOWN,
                reason = getEnumValue<PaymentReasonEnum>(reason ?: "") ?: PaymentReasonEnum.UNKNOWN,
                amount = amount ?: 0.0,
                currency = currency ?: "",
                lastSync = lastSync ?: ""
            )
        }
    }
}