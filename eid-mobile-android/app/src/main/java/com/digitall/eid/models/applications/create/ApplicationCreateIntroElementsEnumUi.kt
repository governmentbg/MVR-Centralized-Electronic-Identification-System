/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.models.applications.create

import android.os.Parcelable
import com.digitall.eid.R
import com.digitall.eid.domain.models.base.TypeEnum
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonListElementIdentifier
import kotlinx.parcelize.Parcelize

@Parcelize
enum class ApplicationCreateIntroElementsEnumUi(
    override val type: String,
    val title: StringSource,
) : CommonListElementIdentifier, TypeEnum, Parcelable {
    // personal data
    EDIT_TEXT_FIRST_LATIN_NAME(
        type = "EDIT_TEXT_FIRST_LATIN_NAME",
        title = StringSource(R.string.create_application_forname_latin_title),
    ),
    EDIT_TEXT_SECOND_LATIN_NAME(
        type = "EDIT_TEXT_SECOND_LATIN_NAME",
        title = StringSource(R.string.create_application_father_name_latin_title),
    ),
    EDIT_TEXT_LAST_LATIN_NAME(
        type = "EDIT_TEXT_LAST_LATIN_NAME",
        title = StringSource(R.string.create_application_surname_latin_title),
    ),
    EDIT_TEXT_CITIZENSHIP(
        type = "EDIT_TEXT_CITIZENSHIP",
        title = StringSource(R.string.create_application_citizenship_title),
    ),
    SPINNER_ID_TYPE(
        type = "SPINNER_ID_TYPE",
        title = StringSource(R.string.create_application_identifier_type_title),
    ),
    EDIT_TEXT_ID_NUMBER(
        type = "EDIT_TEXT_ID_NUMBER",
        title = StringSource(R.string.create_application_identifier_number_title),
    ),
    DATE_PICKER_BORN_DATE(
        type = "DATE_PICKER_BORN_DATE",
        title = StringSource(R.string.create_application_date_of_birth_title),
    ),
    // document
    SPINNER_DOCUMENT_TYPE(
        type = "SPINNER_DOCUMENT_TYPE",
        title = StringSource(R.string.create_application_document_type_title),
    ),
    EDIT_TEXT_DOCUMENT_NUMBER(
        type = "EDIT_TEXT_DOCUMENT_NUMBER",
        title = StringSource(R.string.create_application_document_number_title),
    ),
    DATE_PICKER_CREATED_ON(
        type = "DATE_PICKER_CREATED_ON",
        title = StringSource(R.string.create_application_date_of_issue_title),
    ),
    DATE_PICKER_VALID_TO(
        type = "DATE_PICKER_VALID_UNTIL",
        title = StringSource(R.string.create_application_valid_until_title),
    ),
    EDIT_TEXT_ISSUED_FROM(
        type = "EDIT_TEXT_ISSUED_FROM",
        title = StringSource(R.string.create_application_issued_from_title),
    ),
    // additional info
    DIALOG_ADMINISTRATOR(
        type = "DIALOG_ADMINISTRATOR",
        title = StringSource(R.string.create_application_electronic_identity_administrator_title),
    ),
    DIALOG_ADMINISTRATOR_OFFICE(
        type = "DIALOG_ADMINISTRATOR_OFFICE",
        title = StringSource(R.string.create_application_administrator_office_title),
    ),
    SPINNER_DEVICE_TYPE(
        type = "SPINNER_DEVICE_TYPE",
        title = StringSource(R.string.create_application_carrier_title),
    ),
   TEXT_VIEW_COMMENT(
        type = "TEXT_VIEW_COMMENT",
        title = StringSource(R.string.create_application_comment_title),
    ),
    EDIT_TEXT_CERTIFICATE_NAME(
        type = "EDIT_TEXT_CERTIFICATE_NAME",
        title = StringSource(R.string.create_application_certificate_alias_title)
    ),
    // Authentication type
    SPINNER_AUTH_TYPE(
        type = "SPINNER_SIGNING_TYPE",
        title = StringSource(R.string.create_application_signing_type_title),
    ),
    BUTTON_APPLY(
        type = "BUTTON_APPLY",
        title = StringSource(R.string.create_application_preview_button_title)
    ),
    BUTTON_CANCEL(
        type = "BUTTON_CANCEL",
        title = StringSource(R.string.create_application_cancel_button_title)
    ),
    BUTTON_SEND(
        type = "BUTTON_SEND",
        title = StringSource(R.string.create_application_send_button_title)
    ),
    BUTTON_SIGN(
        type = "BUTTON_SIGN",
        title = StringSource(R.string.create_application_sign_button_title)
    ),
    BUTTON_EDIT(
        type = "BUTTON_EDIT",
        title = StringSource(R.string.create_application_edit_button_title)
    ),
}

@Parcelize
enum class ApplicationCreateIntroSigningMethodsEnumUi(
    override val type: String,
    val title: StringSource,
) : CommonListElementIdentifier, TypeEnum {
    EVROTRUST("EVROTRUST", StringSource(R.string.signing_method_evrotrust_enum_type)),
    BORIKA("BORIKA", StringSource(R.string.signing_method_borica_enum_type)),
}