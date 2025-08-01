/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.models.network.signing.borica

import com.google.gson.annotations.SerializedName

data class SigningBoricaSignResponse(
    @SerializedName("code") val code: String?,
    @SerializedName("message") val message: String?,
    @SerializedName("responseCode") val responseCode: String?,
    @SerializedName("data") val data: SigningBoricaSignDataResponse?,
)

data class SigningBoricaSignDataResponse(
    @SerializedName("validity") val validity: String?,
    @SerializedName("callbackId") val callbackId: String?,
)
