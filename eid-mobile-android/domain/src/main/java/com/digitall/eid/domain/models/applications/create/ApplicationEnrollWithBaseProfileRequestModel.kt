/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.models.applications.create

data class ApplicationEnrollWithBaseProfileRequestModel(
    val otpCode: String,
    val certificateSigningRequest: String,
)