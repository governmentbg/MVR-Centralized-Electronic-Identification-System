/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.models.applications.create

data class ApplicationEnrollWithEIDRequestModel(
    val applicationId: String,
    val certificateAuthorityName: String,
    val certificateSigningRequest: String,
)