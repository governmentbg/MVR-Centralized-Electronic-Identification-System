/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.models.network.empowerment.create.create

import com.google.gson.annotations.SerializedName

data class EmpowermentCreateRequest(
    @SerializedName("uid") val uid: String?,
    @SerializedName("name") val name: String?,
    @SerializedName("uidType") val uidType: String?,
    @SerializedName("serviceId") val serviceId: Int?,
    @SerializedName("startDate") val startDate: String?,
    @SerializedName("expiryDate") val expiryDate: String?,
    @SerializedName("onBehalfOf") val onBehalfOf: String?,
    @SerializedName("providerId") val providerId: String?,
    @SerializedName("serviceName") val serviceName: String?,
    @SerializedName("providerName") val providerName: String?,
    @SerializedName("issuerPosition") val issuerPosition: String?,
    @SerializedName("typeOfEmpowerment") val typeOfEmpowerment: String?,
    @SerializedName("empoweredUids") val empoweredUids: List<EmpowermentCreateEmpoweredUIDRequest>?,
    @SerializedName("authorizerUids") val authorizerUids: List<EmpowermentCreateAuthorizerUIDRequest>?,
    @SerializedName("volumeOfRepresentation") val volumeOfRepresentation: List<EmpowermentCreateVolumeOfRepresentationRequest>?,
)

data class EmpowermentCreateEmpoweredUIDRequest(
    @SerializedName("uid") val uid: String?,
    @SerializedName("uidType") val uidType: String?,
    @SerializedName("name") val name: String?
)

data class EmpowermentCreateAuthorizerUIDRequest(
    @SerializedName("uid") val uid: String?,
    @SerializedName("uidType") val uidType: String?,
    @SerializedName("name") val name: String?,
    @SerializedName("isIssuer") val issuer: Boolean?
)

data class EmpowermentCreateVolumeOfRepresentationRequest(
    @SerializedName("code") val code: String?,
    @SerializedName("name") val name: String?,
)