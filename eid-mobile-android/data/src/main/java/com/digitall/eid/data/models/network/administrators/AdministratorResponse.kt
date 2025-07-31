/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.models.network.administrators

import com.google.gson.annotations.SerializedName

data class AdministratorResponse(
    @SerializedName("id") val id: String?,
    @SerializedName("name") val name: String?,
    @SerializedName("nameLatin") val nameLatin: String?,
    @SerializedName("eikNumber") val eikNumber: String?,
    @SerializedName("isActive") val active: Boolean?,
    @SerializedName("contact") val contact: String?,
    @SerializedName("eidManagerFrontOfficeIds") val eidManagerFrontOfficeIds: List<String>?,
    @SerializedName("deviceIds") val deviceIds: List<String>?,
)