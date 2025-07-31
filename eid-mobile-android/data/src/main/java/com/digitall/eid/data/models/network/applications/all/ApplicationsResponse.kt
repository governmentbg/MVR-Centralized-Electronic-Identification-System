/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.models.network.applications.all

import com.google.gson.annotations.SerializedName

data class ApplicationsResponse(
    @SerializedName("size") val size: Int?,
    @SerializedName("last") val last: Boolean?,
    @SerializedName("number") val number: Int?,
    @SerializedName("first") val first: Boolean?,
    @SerializedName("empty") val empty: Boolean?,
    @SerializedName("totalPages") val totalPages: Int?,
    @SerializedName("totalElements") val totalElements: Int?,
    @SerializedName("numberOfElements") val numberOfElements: Int?,
    @SerializedName("content") val content: List<ApplicationResponseItem>?,
)

data class ApplicationResponseItem(
    @SerializedName("id") val id: String?,
    @SerializedName("status") val status: String?,
    @SerializedName("applicationNumber") val applicationNumber: String?,
    @SerializedName("createDate") val createDate: String?,
    @SerializedName("deviceId") val deviceId: String?,
    @SerializedName("eidentityId") val eidentityId: String,
    @SerializedName("applicationType") val applicationType: String?,
    @SerializedName("eidAdministratorName") val eidAdministratorName: String?,
    @SerializedName("paymentAccessCode") val paymentAccessCode: String?,
)