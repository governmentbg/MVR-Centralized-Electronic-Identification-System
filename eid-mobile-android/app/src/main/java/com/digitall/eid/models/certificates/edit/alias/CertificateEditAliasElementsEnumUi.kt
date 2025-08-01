package com.digitall.eid.models.certificates.edit.alias

import com.digitall.eid.R
import com.digitall.eid.domain.models.base.TypeEnum
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonListElementIdentifier
import kotlinx.parcelize.Parcelize

@Parcelize
enum class CertificateEditAliasElementsEnumUi(override val type: String, val title: StringSource) :
    CommonListElementIdentifier, TypeEnum {
    EDIT_TEXT_NAME("EDIT_TEXT_ALIAS", StringSource(R.string.certificate_edit_alias_title)),
    BUTTON_BACK(
        type = "BUTTON_BACK",
        title = StringSource(R.string.back)
    ),
    BUTTON_CONFIRM(
        type = "BUTTON_CONFIRM",
        title = StringSource(R.string.certificate_edit_alias_confirm_button)
    )
}