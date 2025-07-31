/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.models.empowerment.common.filter

import android.os.Parcelable
import androidx.annotation.ColorRes
import androidx.annotation.DrawableRes
import com.digitall.eid.R
import com.digitall.eid.domain.models.base.TypeEnum
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonListElementIdentifier
import kotlinx.parcelize.Parcelize

@Parcelize
enum class EmpowermentFilterElementsEnumUi(
    override val type: String,
    val title: StringSource,
) : CommonListElementIdentifier, TypeEnum, Parcelable {
    SPINNER_STATUS(
        "SPINNER_STATUS",
        StringSource(R.string.empowerments_entity_filter_status_title)
    ),
    SPINNER_ON_BEHALF_OF(
        "SPINNER_ON_BEHALF_OF",
        StringSource(R.string.empowerments_entity_filter_on_behalf_off_title)
    ),
    EDIT_TEXT_NUMBER_EMPOWERMENT(
        "EDIT_TEXT_NUMBER_EMPOWERMENT",
        StringSource(R.string.empowerments_entity_filter_empowerment_number_title)
    ),
    EDIT_TEXT_LEGAL_ENTITY_NAME(
        "EDIT_TEXT_LEGAL_ENTITY_NAME",
        StringSource(R.string.empowerments_entity_filter_legal_entity_name_title)
    ),
    EDIT_TEXT_EMPOWERER(
        "EDIT_TEXT_EMPOWERER",
        StringSource(R.string.empowerments_entity_filter_empowerer_title)
    ),
    EDIT_TEXT_SUPPLIER(
        "SPINNER_SUPPLIER",
        StringSource(R.string.empowerments_entity_filter_supplier_title)
    ),
    EDIT_TEXT_SERVICE(
        "EDIT_TEXT_SERVICE",
        StringSource(R.string.empowerments_entity_filter_service_title)
    ),
    EDIT_TEXT_EIK_BULSTAT(
        "EDIT_TEXT_EIK_BULSTAT",
        StringSource(R.string.empowerments_entity_filter_eik_bulstat_title)
    ),
    CHECK_BOX_UNLIMITED_DATE(
        "CHECK_BOX_UNLIMITED_DATE",
        StringSource(R.string.empowerments_entity_filter_unlimited_title)
    ),
    DATE_PICKER_DATE(
        "DATE_PICKER_DATE",
        StringSource(R.string.empowerments_entity_filter_end_date_title)
    ),
    BUTTON_ADD_EMPOWERED_PERSON(
        "BUTTON_ADD_EMPOWERED_PERSON",
        StringSource(R.string.empowerments_entity_filter_add_empowered_person_button_title)
    ),
    SPINNER_ID_TYPE(
        "SPINNER_ID_TYPE",
        StringSource(R.string.empowerments_entity_filter_identifier_title)
    ),
    EDIT_TEXT_ID_NUMBER(
        "EDIT_TEXT_ID_NUMBER",
        StringSource(R.string.empowerments_entity_filter_egn_lnch_empowered_person_title)
    ),
    BUTTON(
        "BUTTON",
        StringSource("")
    ),
}

@Parcelize
enum class EmpowermentFilterIdTypeEnumUi(
    override val type: String,
    val title: StringSource,
) : CommonListElementIdentifier, TypeEnum, Parcelable {
    SPINNER_ID_TYPE_EGN(
        "EGN",
        StringSource(R.string.egn)
    ),
    SPINNER_ID_TYPE_LNCH(
        "LNCH",
        StringSource(R.string.lnch)
    ),
}

@Parcelize
enum class EmpowermentFilterButtonsEnumUi(
    override val type: String,
    val title: StringSource,
) : CommonListElementIdentifier, TypeEnum, Parcelable {
    BUTTON_SEND(
        "BUTTON_SEND",
        StringSource(R.string.empowerments_entity_filter_apply_filters_button_title)
    ),
    BUTTON_CANCEL(
        "BUTTON_CANCEL",
        StringSource(R.string.empowerments_entity_filter_clear_button_title)
    ),
}

@Parcelize
enum class EmpowermentFilterStatusEnumUi(
    override val type: String,
    val title: StringSource,
    @param:ColorRes val colorRes: Int,
    @param:DrawableRes val iconRes: Int?,
) : CommonListElementIdentifier, TypeEnum, Parcelable {
    ALL(
        type = "",
        title = StringSource(R.string.empowerments_entity_statuses_all_status_title),
        colorRes = R.color.color_4C6F9E,
        iconRes = null
    ),
    ACTIVE(
        type = "Active",
        title = StringSource(R.string.empowerments_entity_statuses_active_status_title),
        colorRes = R.color.color_018930,
        iconRes = R.drawable.ic_multiple_check
    ),
    DISAGREEMENT_DECLARED(
        type = "DisagreementDeclared",
        title = StringSource(R.string.empowerments_entity_statuses_disagreement_declared_status_title),
        colorRes = R.color.color_BF1212,
        iconRes = R.drawable.ic_error
    ),
    WITHDRAWN(
        type = "Withdrawn",
        title = StringSource(R.string.empowerments_entity_statuses_withdrawn_status_title),
        colorRes = R.color.color_BF1212,
        iconRes = R.drawable.ic_cancel
    ),
    VERIFIED(
        type = "Verified",
        title = StringSource(R.string.empowerments_entity_statuses_verified_status_title),
        colorRes = R.color.color_018930,
        iconRes = null
    ),
    CREATED(
        type = "Created",
        title = StringSource(R.string.empowerments_entity_statuses_created_status_title),
        colorRes = R.color.color_018930,
        iconRes = null
    ),
    EXPIRED(
        type = "Expired",
        title = StringSource(R.string.empowerments_entity_statuses_expired_status_title),
        colorRes = R.color.color_94A3B8,
        iconRes = R.drawable.ic_retrieved
    ),
    DENIED(
        type = "Denied",
        title = StringSource(R.string.empowerments_entity_statuses_denied_status_title),
        colorRes = R.color.color_BF1212,
        iconRes = R.drawable.ic_erase
    ),
    COLLECTING_AUTHORIZER_SIGNATURES(
        type = "CollectingAuthorizerSignatures",
        title = StringSource(R.string.empowerments_entity_statuses_collecting_authorizer_signatures_status_title),
        colorRes = R.color.color_F59E0B,
        iconRes = R.drawable.ic_clock
    ),
    COLLECTING_WITHDRAWAL_SIGNATURES(
        type = "CollectingWithdrawalSignatures",
        title = StringSource(R.string.empowerments_entity_statuses_collecting_withdrawal_signatures_status_title),
        colorRes = R.color.color_F59E0B,
        iconRes = R.drawable.ic_clock
    ),
    UNCONFIRMED(
        type = "Unconfirmed",
        title = StringSource(R.string.empowerments_entity_statuses_unconfirmed_status_title),
        colorRes = R.color.color_F59E0B,
        iconRes = R.drawable.ic_warning
    ),
    UPCOMING(
        type = "UpComing",
        title = StringSource(R.string.empowerments_entity_statuses_upcoming_status_title),
        colorRes = R.color.color_0C53B2,
        iconRes = R.drawable.ic_upcoming
    ),
    AWAITING_SIGNATURE(
        type = "AwaitingSignature",
        title = StringSource(R.string.empowerments_entity_statuses_awaiting_signature_status_title),
        colorRes = R.color.color_F59E0B,
        iconRes = R.drawable.ic_clock
    ),
    UNKNOWN(
        type = "",
        title = StringSource(R.string.unknown),
        colorRes = R.color.color_BF1212,
        iconRes = null
    ),
}

@Parcelize
enum class EmpowermentFilterOnBehalfOfEnumUi(
    override val type: String,
    val title: StringSource,
) : CommonListElementIdentifier, TypeEnum, Parcelable {
    ALL(
        "",
        StringSource(R.string.all)
    ),
    INDIVIDUAL(
        "Individual",
        StringSource(R.string.empowerments_entity_on_behalf_off_individual_enum_type)
    ),
    LEGAL_ENTITY(
        "LegalEntity",
        StringSource(R.string.empowerments_entity_on_behalf_off_legal_entity_enum_type)
    ),
}

@Parcelize
enum class EmpowermentOnBehalfOf(
    override val type: String,
) : CommonListElementIdentifier, TypeEnum, Parcelable {
    INDIVIDUAL("Individual"),
    LEGAL_ENTITY("LegalEntity"),
}