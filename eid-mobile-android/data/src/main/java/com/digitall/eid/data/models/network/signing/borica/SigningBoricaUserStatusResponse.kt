/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.models.network.signing.borica

import com.google.gson.annotations.SerializedName

data class SigningBoricaUserStatusResponse(
    @SerializedName("responseCode") val responseCode: String?,
    @SerializedName("code") val code: String?,
    @SerializedName("message") val message: String?,
    @SerializedName("data") val data: SigningBoricaUserStatusDataResponse?,
)

data class SigningBoricaUserStatusDataResponse(
    @SerializedName("certReqId") val certReqId: String?,
    @SerializedName("devices") val devices: List<String>?,
    @SerializedName("encodedCert") val encodedCert: String?,
)
