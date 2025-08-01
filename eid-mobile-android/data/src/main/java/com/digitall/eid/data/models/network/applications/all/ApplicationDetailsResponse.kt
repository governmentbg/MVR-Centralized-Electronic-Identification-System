/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.models.network.applications.all

import com.google.gson.annotations.SerializedName

data class ApplicationDetailsResponse(
    @SerializedName("id") val id: String?,
    @SerializedName("xml") val xml: String?,
    @SerializedName("email") val email: String?,
    @SerializedName("status") val status: String?,
    @SerializedName("reasonId") val reasonId: String?,
    @SerializedName("createDate") val createDate: String?,
    @SerializedName("lastName") val lastName: String?,
    @SerializedName("firstName") val firstName: String?,
    @SerializedName("secondName") val secondName: String?,
    @SerializedName("deviceId") val deviceId: String?,
    @SerializedName("reasonText") val reasonText: String?,
    @SerializedName("phoneNumber") val phoneNumber: String?,
    @SerializedName("eidentityId") val eidentityId: String?,
    @SerializedName("serialNumber") val serialNumber: String?,
    @SerializedName("certificateId") val certificateId: String?,
    @SerializedName("applicationNumber") val applicationNumber: String?,
    @SerializedName("submissionType") val submissionType: String,
    @SerializedName("applicationType") val applicationType: String?,
    @SerializedName("eidAdministratorName") val eidAdministratorName: String?,
    @SerializedName("eidAdministratorOfficeName") val eidAdministratorOfficeName: String?,
    @SerializedName("identityType") val identityType: String?,
    @SerializedName("identityNumber") val identityNumber: String?,
    @SerializedName("identityIssueDate") val identityIssueDate: String?,
    @SerializedName("identityValidityToDate") val identityValidityToDate: String?,
    @SerializedName("paymentAccessCode") val paymentAccessCode: String?,
)