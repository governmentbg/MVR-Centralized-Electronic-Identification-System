/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.models.applications.create

data class ApplicationConfirmWithBaseProfileRequestModel(
    val status: String,
    val otpCode: String,
)