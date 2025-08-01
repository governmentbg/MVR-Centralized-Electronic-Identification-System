/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.models.network.signing.borica

import com.google.gson.annotations.SerializedName

data class SigningBoricaDownloadResponse(
    @SerializedName("code") val code: String?,
    @SerializedName("message") val message: String?,
    @SerializedName("content") val content: String?,
    @SerializedName("fileName") val fileName: String?,
    @SerializedName("contentType") val contentType: String?,
    @SerializedName("responseCode") val responseCode: String?,
)
