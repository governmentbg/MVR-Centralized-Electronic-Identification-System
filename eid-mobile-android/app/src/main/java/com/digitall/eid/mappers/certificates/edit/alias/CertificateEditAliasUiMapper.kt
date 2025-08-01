package com.digitall.eid.mappers.certificates.edit.alias

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.domain.CERTIFICATE_ALIAS_MAX_LENGTH
import com.digitall.eid.models.certificates.edit.alias.CertificateEditAliasAdapterMarker
import com.digitall.eid.models.certificates.edit.alias.CertificateEditAliasElementsEnumUi
import com.digitall.eid.models.common.ButtonColorUi
import com.digitall.eid.models.common.validator.NonEmptyEditTextValidator
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonEditTextUi
import com.digitall.eid.models.list.CommonEditTextUiType

class CertificateEditAliasUiMapper: BaseMapper<String?, List<CertificateEditAliasAdapterMarker>>() {

    override fun map(from: String?) = buildList {
        add(
            CommonEditTextUi(
                required = false,
                question = false,
                selectedValue = from,
                type = CommonEditTextUiType.TEXT_INPUT_CAP,
                title = CertificateEditAliasElementsEnumUi.EDIT_TEXT_NAME.title,
                elementEnum = CertificateEditAliasElementsEnumUi.EDIT_TEXT_NAME,
                maxSymbols = CERTIFICATE_ALIAS_MAX_LENGTH,
                validators = listOf(
                    NonEmptyEditTextValidator(),
                )
            )
        )
        add(
            CommonButtonUi(
                title = CertificateEditAliasElementsEnumUi.BUTTON_CONFIRM.title,
                elementEnum = CertificateEditAliasElementsEnumUi.BUTTON_CONFIRM,
                buttonColor = ButtonColorUi.BLUE,
                isEnabled = false
            )
        )
        add(
            CommonButtonUi(
                title = CertificateEditAliasElementsEnumUi.BUTTON_BACK.title,
                elementEnum = CertificateEditAliasElementsEnumUi.BUTTON_BACK,
                buttonColor = ButtonColorUi.TRANSPARENT,
            )
        )
    }
}