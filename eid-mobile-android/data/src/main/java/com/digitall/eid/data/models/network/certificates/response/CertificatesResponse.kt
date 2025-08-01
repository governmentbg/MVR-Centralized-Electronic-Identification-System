/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.models.network.certificates.response

import com.google.gson.annotations.SerializedName

data class CertificatesResponse(
    @SerializedName("size") val size: Int?,
    @SerializedName("last") val last: Boolean?,
    @SerializedName("number") val number: Int?,
    @SerializedName("first") val first: Boolean?,
    @SerializedName("totalPages") val totalPages: Int?,
    @SerializedName("totalElements") val totalElements: Int?,
    @SerializedName("numberOfElements") val numberOfElements: Int?,
    @SerializedName("content") val content: List<CertificateResponseItem>?,
)

data class CertificateResponseItem(
    @SerializedName("id") val id: String?,
    @SerializedName("status") val status: String?,
    @SerializedName("deviceId") val deviceId: String?,
    @SerializedName("eidentityId") val eidentityId: String?,
    @SerializedName("validityFrom") val validityFrom: String?,
    @SerializedName("serialNumber") val serialNumber: String?,
    @SerializedName("validityUntil") val validityUntil: String?,
    @SerializedName("isExpiring") val expiring: Boolean?,
    @SerializedName("alias") val alias: String?
)
