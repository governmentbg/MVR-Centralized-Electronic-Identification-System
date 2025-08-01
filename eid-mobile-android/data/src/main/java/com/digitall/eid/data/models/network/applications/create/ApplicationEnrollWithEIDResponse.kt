/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.models.network.applications.create

import com.google.gson.annotations.SerializedName

data class ApplicationEnrollWithEIDResponse(
    @SerializedName("id") val id: String?,
    @SerializedName("issuerDN") val issuerDN: String?,
    @SerializedName("certificate") val certificate: String?,
    @SerializedName("serialNumber") val serialNumber: String?,
    @SerializedName("endEntityProfile") val endEntityProfile: String?,
    @SerializedName("certificateProfile") val certificateProfile: String?,
    @SerializedName("certificateChain") val certificateChain: List<String>?,
)