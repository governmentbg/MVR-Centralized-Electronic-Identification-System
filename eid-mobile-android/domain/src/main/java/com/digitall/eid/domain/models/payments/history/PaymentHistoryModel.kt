package com.digitall.eid.domain.models.payments.history

import com.digitall.eid.domain.models.common.OriginalModel
import kotlinx.parcelize.Parcelize

@Parcelize
data class PaymentHistoryModel(
    val paymentId: String?,
    val citizenProfileId: String?,
    val createdOn: String?,
    val paymentDeadline: String?,
    val paymentDate: String?,
    val status: String?,
    val accessCode: String?,
    val registrationTime: String?,
    val referenceNumber: String?,
    val reason: String?,
    val currency: String?,
    val amount: Double?,
    val lastSync: String?,
): OriginalModel
