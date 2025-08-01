/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.models.network.applications.create

import com.google.gson.annotations.SerializedName

data class ApplicationSignWithBaseProfileRequest(
    @SerializedName("otpCode") val otpCode: String,
    @SerializedName("firebaseId") val firebaseId: String,
    @SerializedName("forceUpdate") val forceUpdate: Boolean,
    @SerializedName("mobileApplicationInstanceId") val mobileApplicationInstanceId: String,
)