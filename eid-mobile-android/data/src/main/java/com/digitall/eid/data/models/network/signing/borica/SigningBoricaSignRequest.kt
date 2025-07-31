/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.models.network.signing.borica

import com.google.gson.annotations.SerializedName

data class SigningBoricaSignRequest(
    @SerializedName("contents") val contents: List<SigningBoricaSignContentRequest>?,
    @SerializedName("uid") val uid: String?,
)

data class SigningBoricaSignContentRequest(
    @SerializedName("data") val data: String?,
    @SerializedName("fileName") val fileName: String?,
    @SerializedName("mediaType") val mediaType: String?,
    @SerializedName("confirmText") val confirmText: String?,
    @SerializedName("contentFormat") val contentFormat: String?,
    @SerializedName("padesVisualSignature") val pagesVisualSignature: Boolean?,
    @SerializedName("signaturePosition") val signaturePosition: SigningBoricaSignSignaturePositionRequest?,
)

data class SigningBoricaSignSignaturePositionRequest(
    @SerializedName("imageWidth") val imageWidth: Int?,
    @SerializedName("imageXAxis") val imageXAxis: Int?,
    @SerializedName("imageYAxis") val imageYAxis: Int?,
    @SerializedName("pageNumber") val pageNumber: Int?,
    @SerializedName("imageHeight") val imageHeight: Int?,
)