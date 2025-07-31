/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.models.applications.create

data class ApplicationSignWithBaseProfileRequestModel(
    val otpCode: String,
    val firebaseId: String,
    val forceUpdate: Boolean,
    val mobileApplicationInstanceId: String,
)