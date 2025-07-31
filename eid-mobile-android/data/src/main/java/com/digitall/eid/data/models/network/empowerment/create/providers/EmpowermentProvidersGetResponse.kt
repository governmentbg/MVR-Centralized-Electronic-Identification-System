/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.models.network.empowerment.create.providers

import com.google.gson.annotations.SerializedName

data class EmpowermentProvidersGetResponse(
    @SerializedName("pageIndex") val pageIndex: Int?,
    @SerializedName("totalItems") val totalItems: Int?,
    @SerializedName("data") val data: List<EmpowermentProviderResponse>?,
)

data class EmpowermentProviderResponse(
    @SerializedName("id") val id: String?,
    @SerializedName("name") val name: String?,
    @SerializedName("status") val status: String?,
    @SerializedName("isExternal") val external: Boolean?,
    @SerializedName("identificationNumber") val identificationNumber: String?,
)
