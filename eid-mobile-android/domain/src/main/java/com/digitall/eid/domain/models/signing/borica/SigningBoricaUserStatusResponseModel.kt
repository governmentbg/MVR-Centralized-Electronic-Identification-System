/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.models.signing.borica

data class SigningBoricaUserStatusResponseModel(
    val code: String?,
    val message: String?,
    val certReqId: String?,
    val encodedCert: String?,
    val responseCode: String?,
    val devices: List<String>?,
)