/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.models.network.signing.evrotrust

import com.google.gson.annotations.SerializedName

data class SigningEvrotrustSignRequest(
    @SerializedName("dateExpire") val dateExpire: String?,
    @SerializedName("documents") val documents: List<SigningEvrotrustSignDocumentRequest>?,
    @SerializedName("uid") val uid: String?,
)

data class SigningEvrotrustSignDocumentRequest(
    @SerializedName("content") val content: String?,
    @SerializedName("fileName") val fileName: String?,
    @SerializedName("contentType") val contentType: String?,
)
