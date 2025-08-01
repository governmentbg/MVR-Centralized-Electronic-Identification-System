package com.digitall.eid.models.registration

import com.digitall.eid.R
import com.digitall.eid.domain.models.base.TypeEnum
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonListElementIdentifier
import kotlinx.parcelize.Parcelize

@Parcelize
enum class RegistrationElementsEnumUi(override val type: String, val title: StringSource) :
    CommonListElementIdentifier,
    TypeEnum {
    DISCLAIMER_TEXT("DISCLAIMER", StringSource(R.string.registration_names_disclaimer)),
    EDIT_TEXT_FORNAME("EDIT_TEXT_FORNAME", StringSource(R.string.registration_name)),
    EDIT_TEXT_MIDDLENAME("EDIT_TEXT_MIDDLENAME", StringSource(R.string.registration_fathers_name)),
    EDIT_TEXT_SURNAME("EDIT_TEXT_SURNAME", StringSource(R.string.registration_surname)),
    EDIT_TEXT_FORNAME_LATIN("EDIT_TEXT_FORNAME_LATIN", StringSource(R.string.registration_name_latin)),
    EDIT_TEXT_MIDDLENAME_LATIN("EDIT_TEXT_MIDDLENAME_LATIN", StringSource(R.string.registration_fathers_name_latin)),
    EDIT_TEXT_SURNAME_LATIN("EDIT_TEXT_SURNAME_LATIN", StringSource(R.string.registration_surname_latin)),
    EDIT_TEXT_EMAIL("EDIT_TEXT_EMAIL", StringSource(R.string.registration_email)),
    EDIT_TEXT_PASSWORD("EDIT_TEXT_PASSWORD", StringSource(R.string.registration_password)),
    EDIT_TEXT_CONFIRM_PASSWORD("EDIT_TEXT_CONFIRM_PASSWORD", StringSource(R.string.registration_confirm_password)),
    EDIT_TEXT_PHONE("EDIT_TEXT_PHONE", StringSource(R.string.registration_phone_number)),
    BUTTON_REGISTER("BUTTON_REGISTER", StringSource(R.string.registration_create_profile))

}