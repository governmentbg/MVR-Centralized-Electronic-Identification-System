package com.digitall.eid.models.card.change.pin

import com.digitall.eid.R
import com.digitall.eid.domain.models.base.TypeEnum
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonListElementIdentifier
import kotlinx.parcelize.Parcelize

@Parcelize
enum class CertificateChangePinElementsEnumUi(override val type: String, val title: StringSource) :
    CommonListElementIdentifier,
    TypeEnum {
    DISCLAIMER_TEXT("DISCLAIMER_TEXT", StringSource(R.string.change_certificate_pin_disclaimer_text)),
    EDIT_TEXT_CURRENT_CARD_PIN("EDIT_TEXT_CURRENT_CARD_PIN", StringSource(R.string.change_certificate_pin_enter_current_pin_title)),
    EDIT_TEXT_NEW_CARD_PIN("EDIT_TEXT_NEW_CARD_PIN", StringSource(R.string.change_certificate_pin_enter_new_pin_title)),
    EDIT_TEXT_CONFIRM_CARD_PIN("EDIT_TEXT_CONFIRM_CARD_PIN", StringSource(R.string.change_certificate_pin_confirm_new_pin_title)),
    EDIT_TEXT_CURRENT_CARD_CAN("EDIT_TEXT_CURRENT_CARD_CAN", StringSource(R.string.change_certificate_pin_enter_can_title)),
    BUTTON_CHANGE_PIN("BUTTON_CHANGE_PIN", StringSource(R.string.change_certificate_pin_change_pin_button_title))
}