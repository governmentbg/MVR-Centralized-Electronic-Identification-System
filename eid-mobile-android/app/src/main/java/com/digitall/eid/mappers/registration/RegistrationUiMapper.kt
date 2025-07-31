package com.digitall.eid.mappers.registration

import com.digitall.eid.R
import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.domain.BG_COUNTRY_CODE
import com.digitall.eid.domain.NAMES_MAX_LENGTH
import com.digitall.eid.domain.NAMES_MIN_LENGTH
import com.digitall.eid.models.common.ButtonColorUi
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.common.validator.EmailValidator
import com.digitall.eid.models.common.validator.FieldsMatchValidator
import com.digitall.eid.models.common.validator.FirstUpperCasedValidator
import com.digitall.eid.models.common.validator.MinLengthEditTextValidator
import com.digitall.eid.models.common.validator.NonEmptyEditTextValidator
import com.digitall.eid.models.common.validator.PasswordValidator
import com.digitall.eid.models.common.validator.PhoneValidator
import com.digitall.eid.models.common.validator.SharedNameDependencyValidator
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonDisclaimerTextUi
import com.digitall.eid.models.list.CommonEditTextUi
import com.digitall.eid.models.list.CommonEditTextUiType
import com.digitall.eid.models.list.CommonPhoneTextUi
import com.digitall.eid.models.registration.RegistrationAdapterMarker
import com.digitall.eid.models.registration.RegistrationElementsEnumUi

class RegistrationUiMapper : BaseMapper<Unit, List<RegistrationAdapterMarker>>() {

    override fun map(from: Unit): List<RegistrationAdapterMarker> {
        return buildList {
            add(
                CommonDisclaimerTextUi(
                    elementEnum = RegistrationElementsEnumUi.DISCLAIMER_TEXT,
                    text = RegistrationElementsEnumUi.DISCLAIMER_TEXT.title
                )
            )
            val firstNameField = CommonEditTextUi(
                elementEnum = RegistrationElementsEnumUi.EDIT_TEXT_FORNAME,
                required = true,
                question = false,
                title = RegistrationElementsEnumUi.EDIT_TEXT_FORNAME.title,
                type = CommonEditTextUiType.TEXT_INPUT_CAP,
                selectedValue = null,
                minSymbols = NAMES_MIN_LENGTH,
                maxSymbols = NAMES_MAX_LENGTH,
                validators = listOf(
                    NonEmptyEditTextValidator(),
                    MinLengthEditTextValidator(minLength = NAMES_MIN_LENGTH),
                    FirstUpperCasedValidator()
                )
            )
            val middleNameField = CommonEditTextUi(
                elementEnum = RegistrationElementsEnumUi.EDIT_TEXT_MIDDLENAME,
                required = false,
                question = false,
                title = RegistrationElementsEnumUi.EDIT_TEXT_MIDDLENAME.title,
                type = CommonEditTextUiType.TEXT_INPUT_CAP,
                selectedValue = null,
                minSymbols = NAMES_MIN_LENGTH,
                maxSymbols = NAMES_MAX_LENGTH,
                validators = listOf(
                    MinLengthEditTextValidator(minLength = NAMES_MIN_LENGTH),
                    FirstUpperCasedValidator()
                )
            )

            val lastNameField = CommonEditTextUi(
                elementEnum = RegistrationElementsEnumUi.EDIT_TEXT_SURNAME,
                required = true,
                question = false,
                title = RegistrationElementsEnumUi.EDIT_TEXT_SURNAME.title,
                type = CommonEditTextUiType.TEXT_INPUT_CAP,
                selectedValue = null,
                minSymbols = NAMES_MIN_LENGTH,
                maxSymbols = NAMES_MAX_LENGTH,
            )

            add(
                firstNameField
            )
            add(
                middleNameField.copy(
                    validators = listOf(
                        MinLengthEditTextValidator(minLength = NAMES_MIN_LENGTH),
                        FirstUpperCasedValidator(),
                        SharedNameDependencyValidator(
                            primaryNameProvider = { firstNameField.selectedValue },
                            siblingNameProvider = { lastNameField.selectedValue }
                        )
                    )
                )
            )
            add(
                lastNameField.copy(
                    validators = listOf(
                        MinLengthEditTextValidator(minLength = NAMES_MIN_LENGTH),
                        FirstUpperCasedValidator(),
                        SharedNameDependencyValidator(
                            primaryNameProvider = { firstNameField.selectedValue },
                            siblingNameProvider = { middleNameField.selectedValue }
                        )
                    )
                )
            )

            val firstNameFieldLatin = CommonEditTextUi(
                elementEnum = RegistrationElementsEnumUi.EDIT_TEXT_FORNAME_LATIN,
                required = true,
                question = false,
                title = RegistrationElementsEnumUi.EDIT_TEXT_FORNAME_LATIN.title,
                type = CommonEditTextUiType.TEXT_INPUT_CAP,
                selectedValue = null,
                minSymbols = NAMES_MIN_LENGTH,
                maxSymbols = NAMES_MAX_LENGTH,
                validators = listOf(
                    NonEmptyEditTextValidator(),
                    MinLengthEditTextValidator(minLength = NAMES_MIN_LENGTH),
                    FirstUpperCasedValidator()
                )
            )
            val middleNameFieldLatin = CommonEditTextUi(
                elementEnum = RegistrationElementsEnumUi.EDIT_TEXT_MIDDLENAME_LATIN,
                required = false,
                question = false,
                title = RegistrationElementsEnumUi.EDIT_TEXT_MIDDLENAME_LATIN.title,
                type = CommonEditTextUiType.TEXT_INPUT_CAP,
                selectedValue = null,
                minSymbols = NAMES_MIN_LENGTH,
                maxSymbols = NAMES_MAX_LENGTH,
                validators = listOf(
                    MinLengthEditTextValidator(minLength = NAMES_MIN_LENGTH),
                    FirstUpperCasedValidator()
                )
            )

            val lastNameFieldLatin = CommonEditTextUi(
                elementEnum = RegistrationElementsEnumUi.EDIT_TEXT_SURNAME_LATIN,
                required = true,
                question = false,
                title = RegistrationElementsEnumUi.EDIT_TEXT_SURNAME_LATIN.title,
                type = CommonEditTextUiType.TEXT_INPUT_CAP,
                selectedValue = null,
                minSymbols = NAMES_MIN_LENGTH,
                maxSymbols = NAMES_MAX_LENGTH,
            )
            add(
                firstNameFieldLatin
            )
            add(
                middleNameFieldLatin.copy(
                    validators = listOf(
                        MinLengthEditTextValidator(minLength = NAMES_MIN_LENGTH),
                        FirstUpperCasedValidator(),
                        SharedNameDependencyValidator(
                            primaryNameProvider = { firstNameFieldLatin.selectedValue },
                            siblingNameProvider = { lastNameFieldLatin.selectedValue }
                        )
                    )
                )
            )
            add(
                lastNameFieldLatin.copy(
                    validators = listOf(
                        MinLengthEditTextValidator(minLength = NAMES_MIN_LENGTH),
                        FirstUpperCasedValidator(),
                        SharedNameDependencyValidator(
                            primaryNameProvider = { firstNameFieldLatin.selectedValue },
                            siblingNameProvider = { middleNameFieldLatin.selectedValue }
                        )
                    )
                )
            )
            add(
                CommonEditTextUi(
                    elementEnum = RegistrationElementsEnumUi.EDIT_TEXT_EMAIL,
                    required = true,
                    question = false,
                    title = RegistrationElementsEnumUi.EDIT_TEXT_EMAIL.title,
                    type = CommonEditTextUiType.EMAIL,
                    selectedValue = null,
                    validators = listOf(
                        NonEmptyEditTextValidator(),
                        EmailValidator()
                    )
                )
            )
            add(
                CommonPhoneTextUi(
                    elementEnum = RegistrationElementsEnumUi.EDIT_TEXT_PHONE,
                    required = false,
                    question = false,
                    title = RegistrationElementsEnumUi.EDIT_TEXT_PHONE.title,
                    selectedValue = null,
                    countryCode = StringSource(BG_COUNTRY_CODE),
                    countryCodeTextColor = R.color.color_0C53B2,
                    validators = listOf(
                        PhoneValidator()
                    )
                )
            )
            val passwordField = CommonEditTextUi(
                elementEnum = RegistrationElementsEnumUi.EDIT_TEXT_PASSWORD,
                required = true,
                question = false,
                title = RegistrationElementsEnumUi.EDIT_TEXT_PASSWORD.title,
                type = CommonEditTextUiType.PASSWORD,
                selectedValue = null,
                validators = listOf(
                    NonEmptyEditTextValidator(),
                    PasswordValidator(),
                )
            )
            add(passwordField)
            add(
                CommonEditTextUi(
                    elementEnum = RegistrationElementsEnumUi.EDIT_TEXT_CONFIRM_PASSWORD,
                    required = true,
                    question = false,
                    title = RegistrationElementsEnumUi.EDIT_TEXT_CONFIRM_PASSWORD.title,
                    type = CommonEditTextUiType.PASSWORD,
                    selectedValue = null,
                    validators = listOf(
                        NonEmptyEditTextValidator(),
                        PasswordValidator(),
                        FieldsMatchValidator(originalFieldTextProvider = { passwordField.selectedValue })
                    )
                )
            )
            add(
                CommonButtonUi(
                    elementEnum = RegistrationElementsEnumUi.BUTTON_REGISTER,
                    title = RegistrationElementsEnumUi.BUTTON_REGISTER.title,
                    buttonColor = ButtonColorUi.BLUE,
                )
            )
        }
    }
}