/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.models.signing.evrotrust

data class SigningEvrotrustDownloadResponseModel(
    val content: String?,
    val fileName: String?,
    val contentType: String?,
)