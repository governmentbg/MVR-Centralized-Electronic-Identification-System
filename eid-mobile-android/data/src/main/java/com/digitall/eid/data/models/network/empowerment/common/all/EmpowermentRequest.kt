/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.models.network.empowerment.common.all

import com.google.gson.annotations.SerializedName

data class EmpowermentRequest(
    @SerializedName("number") val number: String?,
    @SerializedName("status") val status: String?,
    @SerializedName("sortBy") val sortBy: String?,
    @SerializedName("pageSize") val pageSize: Int,
    @SerializedName("pageIndex") val pageIndex: Int,
    @SerializedName("authorizer") val authorizer: String?,
    @SerializedName("onBehalfOf") val onBehalfOf: String?,
    @SerializedName("eik") val eik: String?,
    @SerializedName("validToDate") val validToDate: String?,
    @SerializedName("serviceName") val serviceName: String?,
    @SerializedName("providerName") val providerName: String?,
    @SerializedName("sortDirection") val sortDirection: String?,
    @SerializedName("showOnlyNoExpiryDate") val showOnlyNoExpiryDate: Boolean?,
    @SerializedName("empoweredUids") val empoweredUids: List<EmpowermentUidsRequest>?,
)

data class EmpowermentUidsRequest(
    @SerializedName("uid") val uid: String?,
    @SerializedName("uidType") val uidType: String?,
)