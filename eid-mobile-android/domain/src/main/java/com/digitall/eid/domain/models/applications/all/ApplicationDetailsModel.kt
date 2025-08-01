/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.models.applications.all

data class ApplicationDetailsModel(
    val fromJSON: ApplicationDetailsFromJSONModel,
    val fromXML: ApplicationDetailsFromXMLModel?,
)

data class ApplicationDetailsFromJSONModel(
    val id: String?,
    val xml: String?,
    val email: String?,
    val lastName: String?,
    val firstName: String?,
    val secondName: String?,
    val status: String?,
    val reasonId: String?,
    val createDate: String?,
    val deviceId: String?,
    val reasonText: String?,
    val eidentityId: String?,
    val phoneNumber: String?,
    val serialNumber: String?,
    val certificateId: String?,
    val submissionType: String?,
    val applicationType: String?,
    val eidAdministratorName: String?,
    val eidAdministratorOfficeName: String?,
    val applicationNumber: String?,
    val identityType: String?,
    val identityNumber: String?,
    val identityIssueDate: String?,
    val identityValidityToDate: String?,
    val paymentAccessCode: String?,
)

data class ApplicationDetailsFromXMLModel(
    val email: String?,
    val lastName: String?,
    val firstName: String?,
    val secondName: String?,
    val deviceId: String?,
    val citizenship: String?,
    val dateOfBirth: String?,
    val eidentityId: String?,
    val phoneNumber: String?,
    val identityType: String?,
    val applicationId: String?,
    val lastNameLatin: String?,
    val firstNameLatin: String?,
    val identityIssuer: String?,
    val identityNumber: String?,
    val secondNameLatin: String?,
    val applicationType: String?,
    val citizenProfileId: String?,
    val identityIssueDate: String?,
    val eidAdministratorId: String?,
    val citizenIdentifierType: String?,
    val identityValidityToDate: String?,
    val citizenIdentifierNumber: String?,
    val eidAdministratorOfficeId: String?,
)