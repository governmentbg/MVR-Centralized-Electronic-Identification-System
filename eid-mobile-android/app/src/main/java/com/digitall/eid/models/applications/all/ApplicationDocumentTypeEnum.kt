package com.digitall.eid.models.applications.all

import com.digitall.eid.R
import com.digitall.eid.domain.models.base.TypeEnum
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonListElementIdentifier
import kotlinx.parcelize.Parcelize

@Parcelize
enum class ApplicationDocumentTypeEnum(
    override val type: String,
    val title: StringSource,
) : CommonListElementIdentifier, TypeEnum {
    IDENTITY_CARD(
        "Identity card",
        StringSource(R.string.create_application_document_type_enum_identity_card_title)
    ),
}