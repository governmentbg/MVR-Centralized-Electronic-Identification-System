/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.models.network.applications.create

import com.google.gson.annotations.SerializedName

data class ApplicationConfirmWithBaseProfileRequest(
    @SerializedName("status") val status: String,
    @SerializedName("otpCode") val otpCode: String,
)