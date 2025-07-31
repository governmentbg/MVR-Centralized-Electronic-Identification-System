/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.models.applications.create

data class ApplicationUpdateProfileRequestModel(
    val firebaseId: String,
    val forceUpdate: Boolean,
    val mobileApplicationInstanceId: String,
)