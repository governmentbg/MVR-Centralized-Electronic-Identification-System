/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.models.network.applications.create

import com.google.gson.annotations.SerializedName

data class ApplicationEnrollWithBaseProfileResponse(
    @SerializedName("id") val id: String?,
    @SerializedName("certificate") val certificate: String?,
    @SerializedName("serialNumber") val serialNumber: String?,
    @SerializedName("certificateChain") val certificateChain: List<String>?,
    @SerializedName("replacedExistingCertificate") val replacedExistingCertificate: Boolean?,
)
