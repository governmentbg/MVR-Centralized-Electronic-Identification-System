/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.models.signing.borica

data class SigningBoricaDownloadResponseModel(
    val code: String?,
    val message: String?,
    val content: String?,
    val fileName: String?,
    val contentType: String?,
    val responseCode: String?,
)