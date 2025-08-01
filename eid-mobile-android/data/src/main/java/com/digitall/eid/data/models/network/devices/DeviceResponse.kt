package com.digitall.eid.data.models.network.devices

import com.google.gson.annotations.SerializedName

data class DeviceResponse(
    @SerializedName("id") val id: String?,
    @SerializedName("name") val name: String?,
    @SerializedName("type") val type: String?,
    @SerializedName("description") val description: String?
)
