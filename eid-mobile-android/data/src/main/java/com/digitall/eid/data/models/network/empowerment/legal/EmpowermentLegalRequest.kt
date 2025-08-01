package com.digitall.eid.data.models.network.empowerment.legal

import com.google.gson.annotations.SerializedName

data class EmpowermentLegalRequest(
    @SerializedName("eik") val legalNumber: String?,
    @SerializedName("status") val status: String?,
    @SerializedName("sortBy") val sortBy: String?,
    @SerializedName("pageSize") val pageSize: Int,
    @SerializedName("pageIndex") val pageIndex: Int,
    @SerializedName("validToDate") val validToDate: String?,
    @SerializedName("serviceName") val serviceName: String?,
    @SerializedName("providerName") val providerName: String?,
    @SerializedName("sortDirection") val sortDirection: String?,
    @SerializedName("showOnlyNoExpiryDate") val showOnlyNoExpiryDate: Boolean?,
    @SerializedName("authorizerUids") val authorizerUids: List<EmpowermentLegalAuthorizerUidsRequest>?,
)

data class EmpowermentLegalAuthorizerUidsRequest(
    @SerializedName("uid") val uid: String?,
    @SerializedName("uidType") val uidType: String?,
)