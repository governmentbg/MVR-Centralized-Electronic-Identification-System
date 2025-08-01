package com.digitall.eid.models.empowerment.legal.search

import com.digitall.eid.R
import com.digitall.eid.domain.models.base.TypeEnum
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonListElementIdentifier
import kotlinx.parcelize.Parcelize

@Parcelize
enum class EmpowermentLegalSearchElementsEnumUi(
    override val type: String,
    val title: StringSource,
) : CommonListElementIdentifier, TypeEnum {
    EDIT_TEXT_LEGAL_ENTITY_NUMBER("EDIT_TEXT_LEGAL_ENTITY_NUMBER", StringSource(R.string.empowerments_inquiry_legal_entity_search_input_title)),
    BUTTON_CONFIRM("BUTTON_CONFIRM", StringSource(R.string.empowerments_inquiry_legal_entity_search_confirm_button_title)),
}