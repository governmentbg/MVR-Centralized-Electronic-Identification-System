package com.digitall.eid.models.certificates.revoke

import com.digitall.eid.R
import com.digitall.eid.domain.models.base.TypeEnum
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonListElementIdentifier
import kotlinx.parcelize.Parcelize

@Parcelize
enum class CertificateRevokeElementsEnumUi(
    override val type: String,
    val title: StringSource,
) : CommonListElementIdentifier, TypeEnum {
    TEXT_VIEW_REASON(
        type = "TEXT_VIEW_REASON",
        title = StringSource(R.string.certificate_revoke_reason_title),
    ),
    EDIT_TEXT_REASON(
        type = "EDIT_TEXT_REASON",
        title = StringSource(R.string.certificate_revoke_input_reason_title),
    ),
    EDIT_TEXT_FORNAME(
        type = "EDIT_TEXT_FORNAME",
        title = StringSource(R.string.certificate_revoke_name_title),
    ),
    EDIT_TEXT_MIDDLENAME(
        type = "EDIT_TEXT_MIDDLENAME",
        title = StringSource(R.string.certificate_revoke_fathers_name_title),
    ),
    EDIT_TEXT_SURNAME(
        type = "EDIT_TEXT_SURNAME",
        title = StringSource(R.string.certificate_revoke_surname_title),
    ),
    EDIT_TEXT_FORNAME_LATIN(
        type = "EDIT_TEXT_FORNAME_LATIN",
        title = StringSource(R.string.certificate_revoke_name_latin_title),
    ),
    EDIT_TEXT_MIDDLENAME_LATIN(
        type = "EDIT_TEXT_MIDDLENAME_LATIN",
        title = StringSource(R.string.certificate_revoke_fathers_name_latin_title),
    ),
    EDIT_TEXT_SURNAME_LATIN(
        type = "EDIT_TEXT_SURNAME_LATIN",
        title = StringSource(R.string.certificate_revoke_surname_latin_title),
    ),
    DATE_PICKER_DATE_OF_BIRTH(
        type = "DATE_PICKER_DATE_OF_BIRTH",
        title = StringSource(R.string.certificate_revoke_date_of_birth_title),
    ),
    EDIT_TEXT_DOCUMENT_NUMBER(
        type = "EDIT_TEXT_DOCUMENT_NUMBER",
        title = StringSource(R.string.certificate_revoke_document_number_title),
    ),
    SPINNER_DOCUMENT_TYPE(
        type = "SPINNER_DOCUMENT_TYPE",
        title = StringSource(R.string.certificate_revoke_document_type_title),
    ),
    EDIT_TEXT_CITIZENSHIP(
        type = "EDIT_TEXT_CITIZENSHIP",
        title = StringSource(R.string.certificate_revoke_citizenship_title),
    ),
    EDIT_TEXT_IDENTIFIER(
        type = "EDIT_TEXT_IDENTIFIER",
        title = StringSource(R.string.certificate_revoke_identifier_title),
    ),
    SPINNER_IDENTIFIER_TYPE(
        type = "SPINNER_IDENTIFIER_TYPE",
        title = StringSource(R.string.certificate_revoke_identifier_type_title),
    ),
    DATE_PICKER_ISSUED_ON(
        type = "DATE_PICKER_ISSUED_ON",
        title = StringSource(R.string.certificate_revoke_issued_on_title)
    ),
    EDIT_TEXT_ISSUED_FROM(
        type = "EDIT_TEXT_ISSUED_FROM",
        title = StringSource(R.string.certificate_revoke_issued_from_title),
    ),
    DATE_PICKER_VALID_UNTIL(
        type = "DATE_PICKER_VALID_UNTIL",
        title = StringSource(R.string.certificate_revoke_valid_until_title)
    ),
    SPINNER_SIGNING_TYPE(
        type = "SPINNER_SIGNING_TYPE",
        title = StringSource(R.string.certificate_revoke_signing_type_title),
    ),
    BUTTON_BACK(
        type = "BUTTON_BACK",
        title = StringSource(R.string.back)
    ),
    BUTTON_CONFIRM(
        type = "BUTTON_CONFIRM",
        title = StringSource(R.string.certificate_revoke_confirm_termination_title)
    )
}

@Parcelize
enum class CertificateRevokeDeviceTypeEnum(
    override val type: String,
    val title: StringSource,
) : CommonListElementIdentifier, TypeEnum {
    //    MOBILE("MOBILE", StringSource(R.string.mobile_applicatipon)),
    IDENTITY_CARD("IDENTITY_CARD", StringSource(R.string.identity_card)),
}

@Parcelize
enum class CertificateRevokeIdentifierTypeEnum(
    override val type: String,
    val title: StringSource,
) : CommonListElementIdentifier, TypeEnum {
    EGN("EGN", StringSource(R.string.egn)),
    LNCH("LNCh", StringSource(R.string.lnch)),
}

@Parcelize
data class CertificateRevokeReasonListElement(override val type: String, val id: String) :
    CommonListElementIdentifier, TypeEnum

@Parcelize
enum class CertificateRevokeSigningMethodsEnumUi(
    override val type: String,
    val title: StringSource,
) : CommonListElementIdentifier, TypeEnum {
    EVROTRUST("EVROTRUST", StringSource(R.string.signing_method_evrotrust_enum_type)),
    BORIKA("BORIKA", StringSource(R.string.signing_method_borica_enum_type)),
}