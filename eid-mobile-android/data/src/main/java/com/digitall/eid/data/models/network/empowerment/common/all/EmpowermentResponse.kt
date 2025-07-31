/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.models.network.empowerment.common.all

import com.google.gson.annotations.SerializedName

data class EmpowermentResponse(
    @SerializedName("pageIndex") val pageIndex: Int?,
    @SerializedName("totalItems") val totalItems: Int?,
    @SerializedName("data") val data: List<EmpowermentResponseItem>?,
)

data class EmpowermentResponseItem(
    @SerializedName("id") val id: String,
    @SerializedName("uid") val uid: String?,
    @SerializedName("name") val name: String?,
    @SerializedName("number") val number: String?,
    @SerializedName("status") val status: String?,
    @SerializedName("startDate") val startDate: String?,
    @SerializedName("serviceId") val serviceId: String?,
    @SerializedName("createdOn") val createdOn: String?,
    @SerializedName("createdBy") val createdBy: String?,
    @SerializedName("expiryDate") val expiryDate: String?,
    @SerializedName("onBehalfOf") val onBehalfOf: String?,
    @SerializedName("serviceName") val serviceName: String?,
    @SerializedName("providerId") val providerId: String?,
    @SerializedName("providerName") val providerName: String?,
    @SerializedName("denialReason") val denialReason: String?,
    @SerializedName("issuerPosition") val issuerPosition: String?,
    @SerializedName("xmlRepresentation") val xmlRepresentation: String?,
    @SerializedName("calculatedStatusOn") val calculatedStatusOn: String?,
    @SerializedName("statusHistory") val statusHistory: List<EmpowermentStatusHistoryResponseItem>?,
    @SerializedName("empoweredUids") val empoweredUids: List<EmpowermentEmpowererUidResponseItem>?,
    @SerializedName("authorizerUids") val authorizerUids: List<EmpowermentAuthorizerUidResponseItem>?,
    @SerializedName("empowermentSignatures") val empowermentSignatures: List<EmpowermentSignatureResponseItem>?,
    @SerializedName("empowermentWithdrawals") val empowermentWithdrawals: List<EmpowermentWithdrawalResponseItem>?,
    @SerializedName("empowermentDisagreements") val empowermentDisagreements: List<EmpowermentDisagreementResponseItem>?,
    @SerializedName("volumeOfRepresentation") val volumeOfRepresentation: List<EmpowermentVolumeOfRepresentationResponseItem>?,
)

data class EmpowermentSignatureResponseItem(
    @SerializedName("dateTime") val dateTime: String?,
    @SerializedName("signerUid") val signerUid: String?,
)

data class EmpowermentEmpowererUidResponseItem(
    @SerializedName("uid") val uid: String?,
    @SerializedName("uidType") val uidType: String?,
    @SerializedName("name") val name: String?,
)

data class EmpowermentAuthorizerUidResponseItem(
    @SerializedName("uid") val uid: String?,
    @SerializedName("uidType") val uidType: String?,
    @SerializedName("name") val name: String?,
    @SerializedName("isIssuer") val issuer: Boolean?,
)

data class EmpowermentVolumeOfRepresentationResponseItem(
    @SerializedName("code") val code: String?,
    @SerializedName("name") val name: String?,
)

data class EmpowermentWithdrawalResponseItem(
    @SerializedName("startDateTime") val startDateTime: String?,
    @SerializedName("activeDateTime") val activeDateTime: String?,
    @SerializedName("issuerUid") val issuerUid: String?,
    @SerializedName("reason") val reason: String?,
    @SerializedName("status") val status: String?,
)

data class EmpowermentDisagreementResponseItem(
    @SerializedName("activeDateTime") val activeDateTime: String?,
    @SerializedName("issuerUid") val issuerUid: String?,
    @SerializedName("reason") val reason: String?,
)

data class EmpowermentStatusHistoryResponseItem(
    @SerializedName("id") val id: String?,
    @SerializedName("dateTime") val dateTime: String?,
    @SerializedName("status") val status: String?,
)