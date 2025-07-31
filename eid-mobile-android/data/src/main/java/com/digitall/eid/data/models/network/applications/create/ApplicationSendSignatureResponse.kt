/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.models.network.applications.create

import com.google.gson.annotations.SerializedName

data class ApplicationSendSignatureResponse(
    @SerializedName("id") val id: String?,
    @SerializedName("status") val status: String?,
    @SerializedName("fee") val fee: Double?,
    @SerializedName("feeCurrency") val feeCurrency: String?,
    @SerializedName("secondaryFee") val secondaryFee: Double?,
    @SerializedName("secondaryFeeCurrency") val secondaryFeeCurrency: String?,
    @SerializedName("eidAdministratorName") val eidAdministratorName: String?,
    @SerializedName("paymentAccessCode") val paymentAccessCode: String?,
)