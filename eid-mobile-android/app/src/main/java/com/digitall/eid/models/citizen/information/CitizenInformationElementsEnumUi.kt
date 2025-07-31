package com.digitall.eid.models.citizen.information

import com.digitall.eid.R
import com.digitall.eid.domain.models.base.TypeEnum
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonListElementIdentifier
import kotlinx.parcelize.Parcelize

@Parcelize
enum class CitizenInformationElementsEnumUi(override val type: String, val title: StringSource) :
    CommonListElementIdentifier,
    TypeEnum {
    PERSONAL_DATA_TEXT(
        "PERSONAL_DATA_TEXT",
        StringSource(R.string.citizen_information_personal_data_title)
    ),
    FORNAME_TEXT("FORNAME_TEXT", StringSource(R.string.citizen_information_name_title)),
    MIDDLENAME_TEXT(
        "MIDDLENAME_TEXT",
        StringSource(R.string.citizen_information_fathers_name_title)
    ),
    SURNAME_TEXT("SURNAME_TEXT", StringSource(R.string.citizen_information_surname_title)),
    FORNAME_LATIN_TEXT(
        "FORNAME_LATIN_TEXT",
        StringSource(R.string.citizen_information_name_latin_title)
    ),
    MIDDLENAME_LATIN_TEXT(
        "MIDDLENAME_LATIN_TEXT",
        StringSource(R.string.citizen_information_fathers_name_latin_title)
    ),
    SURNAME_LATIN_TEXT(
        "SURNAME_LATIN_TEXT",
        StringSource(R.string.citizen_information_surname_latin_title)
    ),
    CONTACTS_TEXT("CONTACTS_TEXT", StringSource(R.string.citizen_information_contacts_title)),
    PROFILE_SECURITY_DATA_TEXT(
        "PROFILE_SECURITY_DATA_TEXT",
        StringSource(R.string.citizen_information_profile_security_title)
    ),
    MULTI_FACTOR_AUTHENTICATION_CHECKBOX(
        "MULTI_FACTOR_AUTHENTICATION_CHECKBOX",
        StringSource(R.string.citizen_information_multi_factor_authentication_title)
    ),
    ELECTRONIC_IDENTITY_DATA_TEXT(
        "ELECTRONIC_IDENTITY_DATA_TEXT",
        StringSource(R.string.citizen_information_electronic_identity_title)
    ),
    ELECTRONIC_IDENTITY_NUMBER_TEXT(
        "ELECTRONIC_IDENTITY_NUMBER_TEXT",
        StringSource(R.string.citizen_information_electronic_identity_number_title)
    ),
    EMAIL_TEXT("EMAIL_TEXT", StringSource(R.string.citizen_information_email_title)),
    MOBILE_PHONE_TEXT(
        "MOBILE_PHONE_TEXT",
        StringSource(R.string.citizen_information_mobile_phone_title)
    ),
    BUTTON_CHANGE_PASSWORD(
        "BUTTON_CHANGE_PASSWORD",
        StringSource(R.string.citizen_information_change_password_button_title)
    ),
    BUTTON_ASSOCIATE_EID(
        "BUTTON_ASSOCIATE_EID",
        StringSource(R.string.citizen_information_change_associate_eid_button_title)
    )
}