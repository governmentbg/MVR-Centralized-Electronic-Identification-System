/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.models.signing.borica

data class SigningBoricaSignRequestModel(
    val contents: List<SigningBoricaSignContentRequestModel>?,
    val uid: String?,
)

data class SigningBoricaSignContentRequestModel(
    val data: String?,
    val fileName: String?,
    val mediaType: String?,
    val confirmText: String?,
    val contentFormat: String?,
    val pagesVisualSignature: Boolean?,
    val signaturePosition: SigningBoricaSignSignaturePositionRequestModel?,
)

data class SigningBoricaSignSignaturePositionRequestModel(
    val imageWidth: Int?,
    val imageXAxis: Int?,
    val imageYAxis: Int?,
    val pageNumber: Int?,
    val imageHeight: Int?,
)
