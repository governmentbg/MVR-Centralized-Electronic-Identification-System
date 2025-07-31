package com.digitall.eid.models.auth.password.forgotten

import com.digitall.eid.R
import com.digitall.eid.domain.models.base.TypeEnum
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonListElementIdentifier
import kotlinx.parcelize.Parcelize

@Parcelize
enum class AuthForgottenPasswordElementsEnumUi(override val type: String, val title: StringSource) :
CommonListElementIdentifier, TypeEnum {
    DISCLAIMER_TEXT("DISCLAIMER", StringSource(R.string.forgotten_password_disclaimer)),
    EDIT_TEXT_FORNAME("EDIT_TEXT_FORNAME", StringSource(R.string.forgotten_password_input_name_title)),
    EDIT_TEXT_MIDDLENAME("EDIT_TEXT_MIDDLENAME", StringSource(R.string.forgotten_password_input_fathers_name_title)),
    EDIT_TEXT_SURNAME("EDIT_TEXT_SURNAME", StringSource(R.string.forgotten_password_input_surname_title)),
    EDIT_TEXT_EMAIL("EDIT_TEXT_EMAIL", StringSource(R.string.forgotten_password_input_email_title)),
    BUTTON_CHANGE_PASSWORD("BUTTON_CHANGE_PASSWORD", StringSource(R.string.send))
}