package com.digitall.eid.mappers.certificates.change.pin

import com.digitall.eid.R
import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.domain.PIN_MAX_LENGTH
import com.digitall.eid.models.card.change.pin.CertificateChangePinElementsEnumUi
import com.digitall.eid.models.card.change.pin.CertificateChangePinAdapterMarker
import com.digitall.eid.models.common.ButtonColorUi
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.common.validator.FieldsMatchValidator
import com.digitall.eid.models.common.validator.MinLengthEditTextValidator
import com.digitall.eid.models.common.validator.NonEmptyEditTextValidator
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonDisclaimerTextUi
import com.digitall.eid.models.list.CommonEditTextUi
import com.digitall.eid.models.list.CommonEditTextUiType
import com.digitall.eid.models.registration.RegistrationElementsEnumUi

class CertificateChangePinUiMapper : BaseMapper<Unit, List<CertificateChangePinAdapterMarker>>() {

    override fun map(from: Unit): List<CertificateChangePinAdapterMarker> {
        return buildList {
            val newPinCodeField = CommonEditTextUi(
                elementEnum = CertificateChangePinElementsEnumUi.EDIT_TEXT_NEW_CARD_PIN,
                required = true,
                question = false,
                title = CertificateChangePinElementsEnumUi.EDIT_TEXT_NEW_CARD_PIN.title,
                type = CommonEditTextUiType.PASSWORD_NUMBERS,
                selectedValue = null,
                maxSymbols = PIN_MAX_LENGTH,
                validators = listOf(
                    NonEmptyEditTextValidator(),
                    MinLengthEditTextValidator(
                        minLength = PIN_MAX_LENGTH, errorMessage = StringSource(
                            R.string.error_pin_code_length,
                            formatArgs = listOf(PIN_MAX_LENGTH.toString())
                        )
                    )
                )
            )
            add(
                CommonDisclaimerTextUi(
                    elementEnum = CertificateChangePinElementsEnumUi.DISCLAIMER_TEXT,
                    text = CertificateChangePinElementsEnumUi.DISCLAIMER_TEXT.title
                )
            )
            add(
                CommonEditTextUi(
                    elementEnum = CertificateChangePinElementsEnumUi.EDIT_TEXT_CURRENT_CARD_PIN,
                    required = true,
                    question = false,
                    title = CertificateChangePinElementsEnumUi.EDIT_TEXT_CURRENT_CARD_PIN.title,
                    type = CommonEditTextUiType.PASSWORD_NUMBERS,
                    selectedValue = null,
                    maxSymbols = PIN_MAX_LENGTH,
                    validators = listOf(
                        NonEmptyEditTextValidator(),
                        MinLengthEditTextValidator(
                            minLength = PIN_MAX_LENGTH, errorMessage = StringSource(
                                R.string.error_pin_code_length,
                                formatArgs = listOf(PIN_MAX_LENGTH.toString())
                            )
                        ),
                    )
                )
            )
            add(
                newPinCodeField
            )
            add(
                CommonEditTextUi(
                    elementEnum = CertificateChangePinElementsEnumUi.EDIT_TEXT_CONFIRM_CARD_PIN,
                    required = true,
                    question = false,
                    title = CertificateChangePinElementsEnumUi.EDIT_TEXT_CONFIRM_CARD_PIN.title,
                    type = CommonEditTextUiType.PASSWORD_NUMBERS,
                    selectedValue = null,
                    maxSymbols = PIN_MAX_LENGTH,
                    validators = listOf(
                        NonEmptyEditTextValidator(),
                        MinLengthEditTextValidator(
                            minLength = PIN_MAX_LENGTH, errorMessage = StringSource(
                                R.string.error_pin_code_length,
                                formatArgs = listOf(PIN_MAX_LENGTH.toString())
                            )
                        ),
                        FieldsMatchValidator(
                            originalFieldTextProvider = { newPinCodeField.selectedValue },
                            errorMessage = StringSource(R.string.error_pin_code_mismatch)
                        )
                    )
                )
            )
            add(
                CommonButtonUi(
                    elementEnum = CertificateChangePinElementsEnumUi.BUTTON_CHANGE_PIN,
                    title = CertificateChangePinElementsEnumUi.BUTTON_CHANGE_PIN.title,
                    buttonColor = ButtonColorUi.BLUE,
                )
            )
        }
    }
}