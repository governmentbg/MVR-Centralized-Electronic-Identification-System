/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.models.applications.create

data class ApplicationDetailsXMLRequestModel(
    val lastName: String?,
    val firstName: String?,
    val secondName: String?,
    val deviceId: String?,
    val dateOfBirth: String?,
    val citizenship: String?,
    val lastNameLatin: String?,
    val firstNameLatin: String?,
    val secondNameLatin: String?,
    val applicationType: String?,
    val eidAdministratorId: String?,
    val citizenIdentifierType: String?,
    val citizenIdentifierNumber: String?,
    val eidAdministratorOfficeId: String?,
    var reasonText: String?,
    val reasonId: String?,
    val certificateId: String?,
    val personalIdentityDocument: ApplicationDetailsDocumentXMLRequestModel?,
)

data class ApplicationDetailsDocumentXMLRequestModel(
    val identityType: String?,
    val identityIssuer: String?,
    val identityNumber: String?,
    val identityIssueDate: String?,
    val identityValidityToDate: String?,
)