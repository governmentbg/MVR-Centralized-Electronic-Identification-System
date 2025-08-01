package com.digitall.eid.data.models.network.certificates.response

import com.google.gson.annotations.SerializedName

data class CertificateStatusChangeResponse(
    @SerializedName("id") val id: String?,
    @SerializedName("status") val status: String?,
    @SerializedName("eidAdministratorName") val eidAdministratorName: String?,
    @SerializedName("fee") val fee: Double?,
    @SerializedName("feeCurrency") val feeCurrency: String?,
    @SerializedName("secondaryFee") val secondaryFee: Double?,
    @SerializedName("secondaryFeeCurrency") val secondaryFeeCurrency: String?,
    @SerializedName("paymentAccessCode") val paymentAccessCode: String?,
)
