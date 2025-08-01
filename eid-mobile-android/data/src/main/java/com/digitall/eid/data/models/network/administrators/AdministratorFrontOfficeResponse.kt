package com.digitall.eid.data.models.network.administrators

import com.google.gson.annotations.SerializedName

data class AdministratorFrontOfficeResponse(
    @SerializedName("id") val id: String?,
    @SerializedName("name") val name: String?,
    @SerializedName("eidManagerId") val eidManagerId: String?,
    @SerializedName("location") val location: String?,
    @SerializedName("region") val region: String?,
    @SerializedName("contact") val contact: String?,
    @SerializedName("isActive") val active: Boolean?,
)