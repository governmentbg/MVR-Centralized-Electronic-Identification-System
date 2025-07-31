/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.models.network.signing.borica

import com.google.gson.annotations.SerializedName

data class SigningBoricaStatusResponse(
    @SerializedName("responseCode") val responseCode: String?,
    @SerializedName("code") val code: String?,
    @SerializedName("message") val message: String?,
    @SerializedName("data") val data: SigningBoricaStatusDataResponse?,
)

data class SigningBoricaStatusDataResponse(
    @SerializedName("signatures") val signatures: List<SigningBoricaStatusDataSignatureResponse>?,
    @SerializedName("cert") val cert: String?,
)

data class SigningBoricaStatusDataSignatureResponse(
    @SerializedName("signature") val signature: String?,
    @SerializedName("signatureType") val signatureType: String?,
    @SerializedName("status") val status: String?,
)
