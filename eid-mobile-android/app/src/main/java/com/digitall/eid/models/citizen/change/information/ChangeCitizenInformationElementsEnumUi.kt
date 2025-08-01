package com.digitall.eid.models.citizen.change.information

import com.digitall.eid.R
import com.digitall.eid.domain.models.base.TypeEnum
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonListElementIdentifier
import kotlinx.parcelize.Parcelize


@Parcelize
enum class ChangeCitizenInformationElementsEnumUi(
    override val type: String,
    val title: StringSource
) :
    CommonListElementIdentifier, TypeEnum {
    EDIT_TEXT_FORNAME(
        "EDIT_TEXT_FORNAME",
        StringSource(R.string.change_citizen_information_name_title)
    ),
    EDIT_TEXT_MIDDLENAME(
        "EDIT_TEXT_MIDDLENAME",
        StringSource(R.string.change_citizen_information_fathers_name_title)
    ),
    EDIT_TEXT_SURNAME(
        "EDIT_TEXT_SURNAME",
        StringSource(R.string.change_citizen_information_surname_title)
    ),
    EDIT_TEXT_FORNAME_LATIN(
        "EDIT_TEXT_FORNAME_LATIN",
        StringSource(R.string.change_citizen_information_name_latin_title)
    ),
    EDIT_TEXT_MIDDLENAME_LATIN(
        "EDIT_TEXT_MIDDLENAME_LATIN",
        StringSource(R.string.change_citizen_information_fathers_name_latin_title)
    ),
    EDIT_TEXT_SURNAME_LATIN(
        "EDIT_TEXT_SURNAME_LATIN",
        StringSource(R.string.change_citizen_information_surname_latin_title)
    ),
    EDIT_TEXT_PHONE_NUMBER(
        "EDIT_TEXT_PHONE_NUMBER",
        StringSource(R.string.change_citizen_information_phone_title)
    ),
    BUTTON_CONFIRM(
        "BUTTON_CONFIRM",
        StringSource(R.string.change_user_password_confirm_button_title)
    ),
}