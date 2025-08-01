package com.digitall.eid.domain.models.certificates

data class CertificateStatusChangeRequestModel(
    val forname: String?,
    val middlename: String?,
    val surname: String?,
    val fornameLatin: String?,
    val middlenameLatin: String?,
    val surnameLatin: String?,
    val dateOfBirth: String?,
    val eidAdministratorId: String?,
    val eidAdministratorOfficeId: String?,
    val applicationType: String?,
    val citizenship: String?,
    val citizenIdentifierNumber: String?,
    val citizenIdentifierType: String?,
    var deviceId: String? = "MOBILE",
    val identityDocument: PersonalIdentityDocumentRequestModel?,
    val reasonId: String?,
    val reasonText: String?,
    val certificateId: String?,
)

data class PersonalIdentityDocumentRequestModel(
    val number: String?,
    val type: String?,
    val issueDate: String?,
    val validUntilDate: String?,
    val issuer: String?,
)