package com.digitall.eid.mappers.citizen.change.information

import com.digitall.eid.R
import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.domain.BG_COUNTRY_CODE
import com.digitall.eid.domain.NAMES_MAX_LENGTH
import com.digitall.eid.domain.NAMES_MIN_LENGTH
import com.digitall.eid.domain.models.applications.create.ApplicationUserDetailsModel
import com.digitall.eid.models.citizen.change.information.ChangeCitizenInformationAdapterMarker
import com.digitall.eid.models.citizen.change.information.ChangeCitizenInformationElementsEnumUi
import com.digitall.eid.models.common.ButtonColorUi
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.common.validator.FirstUpperCasedValidator
import com.digitall.eid.models.common.validator.MinLengthEditTextValidator
import com.digitall.eid.models.common.validator.NonEmptyEditTextValidator
import com.digitall.eid.models.common.validator.PhoneValidator
import com.digitall.eid.models.common.validator.SharedNameDependencyValidator
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonEditTextUi
import com.digitall.eid.models.list.CommonEditTextUiType
import com.digitall.eid.models.list.CommonPhoneTextUi

class ChangeCitizenInformationUiMapper: BaseMapper<ApplicationUserDetailsModel, List<ChangeCitizenInformationAdapterMarker>>() {

    override fun map(from: ApplicationUserDetailsModel): List<ChangeCitizenInformationAdapterMarker> {
        return buildList {
            val firstNameField = CommonEditTextUi(
                elementEnum = ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_FORNAME,
                required = true,
                question = false,
                title = ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_FORNAME.title,
                type = CommonEditTextUiType.TEXT_INPUT_CAP,
                selectedValue = from.firstName,
                minSymbols = NAMES_MIN_LENGTH,
                maxSymbols = NAMES_MAX_LENGTH,
                validators = listOf(
                    NonEmptyEditTextValidator(),
                    MinLengthEditTextValidator(minLength = NAMES_MIN_LENGTH),
                    FirstUpperCasedValidator()
                )
            )
            val middleNameField = CommonEditTextUi(
                elementEnum = ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_MIDDLENAME,
                required = false,
                question = false,
                title = ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_MIDDLENAME.title,
                type = CommonEditTextUiType.TEXT_INPUT_CAP,
                selectedValue = from.secondName,
                minSymbols = NAMES_MIN_LENGTH,
                maxSymbols = NAMES_MAX_LENGTH,
                validators = listOf(
                    MinLengthEditTextValidator(minLength = NAMES_MIN_LENGTH),
                    FirstUpperCasedValidator()
                )
            )

            val lastNameField = CommonEditTextUi(
                elementEnum = ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_SURNAME,
                required = true,
                question = false,
                title = ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_SURNAME.title,
                type = CommonEditTextUiType.TEXT_INPUT_CAP,
                selectedValue = from.lastName,
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
                elementEnum = ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_FORNAME_LATIN,
                required = true,
                question = false,
                title = ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_FORNAME_LATIN.title,
                type = CommonEditTextUiType.TEXT_INPUT_CAP,
                selectedValue = from.firstNameLatin,
                minSymbols = NAMES_MIN_LENGTH,
                maxSymbols = NAMES_MAX_LENGTH,
                validators = listOf(
                    NonEmptyEditTextValidator(),
                    MinLengthEditTextValidator(minLength = NAMES_MIN_LENGTH),
                    FirstUpperCasedValidator()
                )
            )
            val middleNameFieldLatin = CommonEditTextUi(
                elementEnum = ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_MIDDLENAME_LATIN,
                required = false,
                question = false,
                title = ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_MIDDLENAME_LATIN.title,
                type = CommonEditTextUiType.TEXT_INPUT_CAP,
                selectedValue = from.secondNameLatin,
                minSymbols = NAMES_MIN_LENGTH,
                maxSymbols = NAMES_MAX_LENGTH,
                validators = listOf(
                    MinLengthEditTextValidator(minLength = NAMES_MIN_LENGTH),
                    FirstUpperCasedValidator()
                )
            )

            val lastNameFieldLatin = CommonEditTextUi(
                elementEnum = ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_SURNAME_LATIN,
                required = true,
                question = false,
                title = ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_SURNAME_LATIN.title,
                type = CommonEditTextUiType.TEXT_INPUT_CAP,
                selectedValue = from.lastNameLatin,
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
                CommonPhoneTextUi(
                    elementEnum = ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_PHONE_NUMBER,
                    required = false,
                    question = false,
                    title = ChangeCitizenInformationElementsEnumUi.EDIT_TEXT_PHONE_NUMBER.title,
                    selectedValue = from.phoneNumber?.removePrefix(BG_COUNTRY_CODE)?.removePrefix("0"),
                    countryCode = StringSource(BG_COUNTRY_CODE),
                    countryCodeTextColor = R.color.color_0C53B2,
                    validators = listOf(
                        PhoneValidator()
                    )
                )
            )
            add(
                CommonButtonUi(
                    elementEnum = ChangeCitizenInformationElementsEnumUi.BUTTON_CONFIRM,
                    title = ChangeCitizenInformationElementsEnumUi.BUTTON_CONFIRM.title,
                    buttonColor = ButtonColorUi.BLUE,
                    isEnabled = false
                )
            )
        }
    }
}