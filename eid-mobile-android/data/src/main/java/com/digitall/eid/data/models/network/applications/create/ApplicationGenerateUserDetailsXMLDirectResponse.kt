/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.models.network.applications.create

import org.simpleframework.xml.Element
import org.simpleframework.xml.Root

@Root(name = "signedDetails")
data class ApplicationGenerateUserDetailsXMLDirectResponse(
    @field:Element(name = "firstName", required = false)
    var firstName: String? = null,
    @field:Element(name = "firstNameLatin", required = false)
    var firstNameLatin: String? = null,
    @field:Element(name = "secondName", required = false)
    var secondName: String? = null,
    @field:Element(name = "secondNameLatin", required = false)
    var secondNameLatin: String? = null,
    @field:Element(name = "lastName", required = false)
    var lastName: String? = null,
    @field:Element(name = "lastNameLatin", required = false)
    var lastNameLatin: String? = null,
    @field:Element(name = "citizenIdentifierNumber", required = false)
    var citizenIdentifierNumber: String? = null,
    @field:Element(name = "citizenIdentifierType", required = false)
    var citizenIdentifierType: String? = null,
    @field:Element(name = "citizenship", required = false)
    var citizenship: String? = null,
    @field:Element(name = "identityNumber", required = false)
    var identityNumber: String? = null,
    @field:Element(name = "identityType", required = false)
    var identityType: String? = null,
    @field:Element(name = "identityIssueDate", required = false)
    var identityIssueDate: String? = null,
    @field:Element(name = "identityValidityToDate", required = false)
    var identityValidityToDate: String? = null,
    @field:Element(name = "identityIssuer", required = false)
    var identityIssuer: String? = null,
    @field:Element(name = "eidAdministratorOfficeId", required = false)
    var eidAdministratorOfficeId: String? = null,
    @field:Element(name = "eidAdministratorId", required = false)
    var eidAdministratorId: String? = null,
    @field:Element(name = "applicationId", required = false)
    var applicationId: String? = null,
    @field:Element(name = "deviceType", required = false)
    var deviceType: String? = null,
    @field:Element(name = "applicationType", required = false)
    var applicationType: String? = null,
    @field:Element(name = "dateOfBirth", required = false)
    var dateOfBirth: String? = null,
    @field:Element(name = "citizenProfileId", required = false)
    var citizenProfileId: String? = null,
)