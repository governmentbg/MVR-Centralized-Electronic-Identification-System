package com.digitall.eid.data.models.network.certificates.request

import com.google.gson.annotations.SerializedName

data class CertificateStatusChangeRequest(
    @SerializedName("firstName") val forname: String?,
    @SerializedName("secondName") val middlename: String?,
    @SerializedName("lastName") val surname: String?,
    @SerializedName("firstNameLatin") val fornameLatin: String?,
    @SerializedName("secondNameLatin") val middlenameLatin: String?,
    @SerializedName("lastNameLatin") val surnameLatin: String?,
    @SerializedName("dateOfBirth") val dateOfBirth: String?,
    @SerializedName("eidAdministratorId") val eidAdministratorId: String?,
    @SerializedName("eidAdministratorOfficeId") val eidAdministratorOfficeId: String?,
    @SerializedName("applicationType") val applicationType: String?,
    @SerializedName("citizenship") val citizenship: String?,
    @SerializedName("citizenIdentifierNumber") val citizenIdentifierNumber: String?,
    @SerializedName("citizenIdentifierType") val citizenIdentifierType: String?,
    @SerializedName("deviceId") val deviceId: String?,
    @SerializedName("personalIdentityDocument") val identityDocument: PersonalIdentityDocumentRequest?,
    @SerializedName("reasonId") val reasonId: String?,
    @SerializedName("reasonText") val reasonText: String?,
    @SerializedName("certificateId") val certificateId: String?,
)

data class PersonalIdentityDocumentRequest(
    @SerializedName("identityNumber") val number: String?,
    @SerializedName("identityType") val type: String?,
    @SerializedName("identityIssueDate") val issueDate: String?,
    @SerializedName("identityValidityToDate") val validUntilDate: String?,
    @SerializedName("identityIssuer") val issuer: String?,
)
