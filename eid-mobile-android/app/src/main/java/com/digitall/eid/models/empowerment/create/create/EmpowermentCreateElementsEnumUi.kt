/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.models.empowerment.create.create

import android.os.Parcelable
import com.digitall.eid.R
import com.digitall.eid.domain.models.base.TypeEnum
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonListElementIdentifier
import kotlinx.parcelize.Parcelize

@Parcelize
enum class EmpowermentCreateElementsEnumUi(
    override val type: String,
    val title: StringSource,
) : CommonListElementIdentifier, TypeEnum, Parcelable {
    TEXT_VIEW_UID_TYPE(
        "TEXT_VIEW_UID_TYPE",
        StringSource(R.string.empowerment_create_identifier_title)
    ),
    TEXT_VIEW_LEGAL_REPRESANTATIVE_UID_NUMBER(
        "TEXT_VIEW_LEGAL_REPRESANTATIVE_UID_NUMBER",
        StringSource(R.string.empowerment_create_identifier_number_title)
    ),
    TEXT_VIEW_LEGAL_REPRESENTATIVE_NAMES(
        "TEXT_VIEW_LEGAL_REPRESENTATIVE_NAMES",
        StringSource(R.string.empowerment_create_legal_representative_names_title)
    ),
    SPINNER_LEGAL_REPRESENTATIVE_UID_TYPE(
        "SPINNER_LEGAL_REPRESENTATIVE_UID_TYPE",
        StringSource(R.string.empowerment_create_identifier_title)
    ),
    EDIT_TEXT_LEGAL_REPRESENTATIVE_UID_NUMBER(
        "EDIT_TEXT_EMPOWERED_UID_NUMBER",
        StringSource(R.string.empowerment_create_identifier_number_title)
    ),
    EDIT_TEXT_LEGAL_REPRESENTATIVE_NAMES(
        "EDIT_TEXT_LEGAL_REPRESENTATIVE_NAMES",
        StringSource(R.string.empowerment_create_legal_representative_names_title)
    ),
    SPINNER_ON_BEHALF_OF(
        "SPINNER_ON_BEHALF_OF",
        StringSource(R.string.empowerment_create_on_behalf_of_title)
    ),
    SPINNER_EMPOWERED_UID_TYPE(
        "SPINNER_EMPOWERED_UID_TYPE",
        StringSource(R.string.empowerment_create_identifier_title)
    ),
    EDIT_TEXT_EMPOWERED_UID_NUMBER(
        "EDIT_TEXT_EMPOWERED_UID_NUMBER",
        StringSource(R.string.empowerment_create_identifier_number_title)
    ),
    EDIT_TEXT_EMPOWERED_NAMES(
        "EDIT_TEXT_EMPOWERED_NAMES",
        StringSource(R.string.empowerment_create_names_title)
    ),
    BUTTON_ADD_EMPOWERED(
        "BUTTON_ADD_EMPOWERED",
        StringSource(R.string.empowerment_create_add_empowered_person_button_title)
    ),
    BUTTON_ADD_LEGAL_REPRESENTATIVE(
        "BUTTON_ADD_LEGAL_REPRESENTATIVE",
        StringSource(R.string.empowerment_create_add_legal_representative_button_title)
    ),
    SPINNER_TYPE_OF_EMPOWERMENT(
        "SPINNER_TYPE_OF_EMPOWERMENT",
        StringSource(R.string.empowerment_create_type_of_empowerment_title)
    ),
    DIALOG_SUPPLIER_NAME(
        "DIALOG_SUPPLIER_NAME",
        StringSource(R.string.empowerment_create_supplier_name_title)
    ),
    DIALOG_SERVICE_NAME(
        "DIALOG_SERVICE_NAME",
        StringSource(R.string.empowerment_create_service_title)
    ),
    DIALOG_VOLUME_OF_REPRESENTATION(
        "DIALOG_VOLUME_OF_REPRESENTATION",
        StringSource(R.string.empowerment_create_volume_of_representation_title)
    ),
    DATE_PICKER_START_DATE(
        "DATE_PICKER_START_DATE",
        StringSource(R.string.empowerment_create_start_date_title)
    ),
    DATE_PICKER_END_DATE(
        "DATE_PICKER_END_DATE",
        StringSource(R.string.empowerment_create_end_date_title)
    ),
    EDIT_TEXT_ISSUER_POSITION(
        "EDIT_TEXT_ISSUER_POSITION",
        StringSource(R.string.empowerment_create_issuer_position_title)
    ),
    EDIT_TEXT_COMPANY_NUMBER(
        "EDIT_TEXT_COMPANY_NUMBER",
        StringSource(R.string.empowerment_create_company_number_title)
    ),
    EDIT_TEXT_COMPANY_NAME(
        "EDIT_TEXT_LEGAL_ENTITY_NAME",
        StringSource(R.string.empowerment_create_legal_entity_name)
    ),
    BUTTON_PREVIEW(
        "BUTTON_PREVIEW",
        StringSource(R.string.empowerment_create_preview_button_title)
    ),
    BUTTON_SUBMISSION(
        "BUTTON_SUBMISSION",
        StringSource(R.string.empowerment_create_submission_button_title)
    ),
    BUTTON_CANCEL(
        "BUTTON_CANCEL",
        StringSource(R.string.cancel)
    ),
    BUTTON_SEND(
        "BUTTON_SEND",
        StringSource(R.string.send)
    ),
    BUTTON_EDIT(
        "BUTTON_EDIT",
        StringSource(R.string.edit)
    ),
}

@Parcelize
enum class EmpowermentCreateFromNameOfEnumUi(
    override val type: String,
    val title: StringSource,
) : CommonListElementIdentifier, TypeEnum, Parcelable {
    PERSON(
        "Individual",
        StringSource(R.string.empowerments_entity_on_behalf_off_individual_enum_type)
    ),
    COMPANY(
        "LegalEntity",
        StringSource(R.string.empowerments_entity_on_behalf_off_legal_entity_enum_type)
    ),
}

@Parcelize
enum class EmpowermentCreateIdTypeEnumUi(
    override val type: String,
    val title: StringSource,
) : CommonListElementIdentifier, TypeEnum, Parcelable {
    EGN(
        "EGN",
        StringSource(R.string.egn)
    ),
    LNCH(
        "LNCH",
        StringSource(R.string.lnch)
    ),
}
