package com.digitall.eid.data.models.network.payments.history.response

import com.google.gson.annotations.SerializedName

data class PaymentHistoryResponse(
    @SerializedName("ePaymentId") val paymentId: String?,
    @SerializedName("citizenProfileId") val citizenProfileId: String?,
    @SerializedName("createdOn") val createdOn: String?,
    @SerializedName("paymentDeadline") val paymentDeadline: String?,
    @SerializedName("paymentDate") val paymentDate: String?,
    @SerializedName("status") val status: String?,
    @SerializedName("accessCode") val accessCode: String?,
    @SerializedName("registrationTime") val registrationTime: String?,
    @SerializedName("referenceNumber") val referenceNumber: String?,
    @SerializedName("reason") val reason: String?,
    @SerializedName("currency") val currency: String?,
    @SerializedName("amount") val amount: Double?,
    @SerializedName("lastSync") val lastSync: String?,
)
