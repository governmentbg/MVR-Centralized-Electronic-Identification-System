/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.models.network.applications.create

import com.google.gson.annotations.SerializedName

data class ApplicationDetailsXMLRequest(
    @SerializedName("lastName") val lastName: String?,
    @SerializedName("firstName") val firstName: String?,
    @SerializedName("secondName") val secondName: String?,
    @SerializedName("deviceId") val deviceId: String?,
    @SerializedName("dateOfBirth") val dateOfBirth: String?,
    @SerializedName("citizenship") val citizenship: String?,
    @SerializedName("lastNameLatin") val lastNameLatin: String?,
    @SerializedName("firstNameLatin") val firstNameLatin: String?,
    @SerializedName("secondNameLatin") val secondNameLatin: String?,
    @SerializedName("applicationType") val applicationType: String?,
    @SerializedName("eidAdministratorId") val eidAdministratorId: String?,
    @SerializedName("citizenIdentifierType") val citizenIdentifierType: String?,
    @SerializedName("citizenIdentifierNumber") val citizenIdentifierNumber: String?,
    @SerializedName("eidAdministratorOfficeId") val eidAdministratorOfficeId: String?,
    @SerializedName("certificateId") val certificateId: String?,
    @SerializedName("personalIdentityDocument") val personalIdentityDocument: ApplicationDetailsDocumentXMLRequest?,
    @SerializedName("reasonId") val reasonId: String?,
    @SerializedName("reasonText") val reasonText: String?,
)

data class ApplicationDetailsDocumentXMLRequest(
    @SerializedName("identityType") val identityType: String?,
    @SerializedName("identityIssuer") val identityIssuer: String?,
    @SerializedName("identityNumber") val identityNumber: String?,
    @SerializedName("identityIssueDate") val identityIssueDate: String?,
    @SerializedName("identityValidityToDate") val identityValidityToDate: String?,
)