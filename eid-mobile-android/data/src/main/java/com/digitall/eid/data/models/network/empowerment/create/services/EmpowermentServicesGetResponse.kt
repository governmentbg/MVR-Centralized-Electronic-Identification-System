/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.models.network.empowerment.create.services

import com.google.gson.annotations.SerializedName

data class EmpowermentServicesGetResponse(
    @SerializedName("pageIndex") val pageIndex: Int?,
    @SerializedName("totalItems") val totalItems: Int?,
    @SerializedName("data") val data: List<EmpowermentServiceResponse>?,
)

data class EmpowermentServiceResponse(
    @SerializedName("id") val id: String,
    @SerializedName("serviceNumber") val serviceNumber: String?,
    @SerializedName("name") val name: String?,
    @SerializedName("description") val description: String?,
    @SerializedName("paymentInfoNormalCost") val paymentInfoNormalCost: Double?,
    @SerializedName("isEmpowerment") val empowerment: Boolean?,
    @SerializedName("isExternal") val external: Boolean?,
    @SerializedName("isDeleted") val deleted: Boolean?,
    @SerializedName("providerDetailsId") val providerDetailsId: String?,
    @SerializedName("providerSectionId") val providerSectionId: String?,
)
