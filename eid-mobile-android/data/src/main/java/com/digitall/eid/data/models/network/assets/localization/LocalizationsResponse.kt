package com.digitall.eid.data.models.network.assets.localization

import com.google.gson.annotations.SerializedName

data class LocalizationsResponse(
    @SerializedName("logs") val logs: Map<String, String>,
    @SerializedName("approvalRequestTypes") val approvalRequestTypes: Map<String, String>,
    @SerializedName("errors") val errors: Map<String, String>
)
