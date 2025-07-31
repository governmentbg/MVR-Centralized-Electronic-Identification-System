package com.digitall.eid.mappers.certificates.resume

import com.digitall.eid.R
import com.digitall.eid.data.mappers.base.BaseMapperWithData
import com.digitall.eid.domain.MIN_AGE_YEARS
import com.digitall.eid.domain.NAMES_MIN_LENGTH
import com.digitall.eid.domain.UiDateFormats
import com.digitall.eid.domain.extensions.getCalendar
import com.digitall.eid.domain.extensions.getEnumValue
import com.digitall.eid.domain.models.applications.create.ApplicationUserDetailsModel
import com.digitall.eid.domain.models.user.UserAcrEnum
import com.digitall.eid.models.certificates.resume.CertificateResumeAdapterMarker
import com.digitall.eid.models.certificates.resume.CertificateResumeDeviceTypeEnum
import com.digitall.eid.models.certificates.resume.CertificateResumeElementsEnumUi
import com.digitall.eid.models.certificates.resume.CertificateResumeIdentifierTypeEnum
import com.digitall.eid.models.certificates.resume.CertificateResumeSigningMethodsEnumUi
import com.digitall.eid.models.common.ButtonColorUi
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.common.validator.BulgarianCardDocumentWithChipValidator
import com.digitall.eid.models.common.validator.FirstUpperCasedValidator
import com.digitall.eid.models.common.validator.MinLengthEditTextValidator
import com.digitall.eid.models.common.validator.NonEmptyDatePickerValidator
import com.digitall.eid.models.common.validator.NonEmptyEditTextValidator
import com.digitall.eid.models.common.validator.NonEmptySpinnerValidator
import com.digitall.eid.models.common.validator.PersonalIdentifierValidator
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonDatePickerUi
import com.digitall.eid.models.list.CommonEditTextUi
import com.digitall.eid.models.list.CommonEditTextUiType
import com.digitall.eid.models.list.CommonSeparatorUi
import com.digitall.eid.models.list.CommonSpinnerMenuItemUi
import com.digitall.eid.models.list.CommonSpinnerUi
import com.digitall.eid.models.list.CommonTitleSmallUi
import com.digitall.eid.models.list.CommonTitleUi

class CertificateResumeUiMapper :
    BaseMapperWithData<ApplicationUserDetailsModel?, UserAcrEnum, List<CertificateResumeAdapterMarker>>() {

    override fun map(
        from: ApplicationUserDetailsModel?,
        data: UserAcrEnum?
    ) = buildList {
        add(
            CommonTitleUi(
                title = StringSource(R.string.certificate_resume_title),
            )
        )
        add(
            CommonSeparatorUi()
        )
        add(
            CommonTitleSmallUi(
                title = StringSource(R.string.certificate_resume_personal_data_title),
            )
        )
        add(
            CommonEditTextUi(
                required = true,
                question = false,
                minSymbols = 3,
                selectedValue = from?.firstName,
                type = CommonEditTextUiType.TEXT_INPUT_CAP,
                title = CertificateResumeElementsEnumUi.EDIT_TEXT_FORNAME.title,
                elementEnum = CertificateResumeElementsEnumUi.EDIT_TEXT_FORNAME,
                validators = listOf(
                    NonEmptyEditTextValidator(),
                    MinLengthEditTextValidator(minLength = NAMES_MIN_LENGTH),
                    FirstUpperCasedValidator()
                )
            )
        )
        add(
            CommonEditTextUi(
                required = false,
                question = false,
                minSymbols = 3,
                selectedValue = from?.secondName,
                type = CommonEditTextUiType.TEXT_INPUT_CAP,
                title = CertificateResumeElementsEnumUi.EDIT_TEXT_MIDDLENAME.title,
                elementEnum = CertificateResumeElementsEnumUi.EDIT_TEXT_MIDDLENAME,
                validators = listOf(
                    NonEmptyEditTextValidator(),
                    MinLengthEditTextValidator(minLength = NAMES_MIN_LENGTH),
                    FirstUpperCasedValidator()
                )
            )
        )
        add(
            CommonEditTextUi(
                required = true,
                question = false,
                minSymbols = 3,
                selectedValue = from?.lastName,
                type = CommonEditTextUiType.TEXT_INPUT_CAP,
                title = CertificateResumeElementsEnumUi.EDIT_TEXT_SURNAME.title,
                elementEnum = CertificateResumeElementsEnumUi.EDIT_TEXT_SURNAME,
                validators = listOf(
                    NonEmptyEditTextValidator(),
                    MinLengthEditTextValidator(minLength = NAMES_MIN_LENGTH),
                    FirstUpperCasedValidator()
                )
            )
        )
        add(
            CommonEditTextUi(
                required = true,
                question = false,
                minSymbols = 3,
                selectedValue = from?.firstNameLatin,
                type = CommonEditTextUiType.TEXT_INPUT_CAP,
                title = CertificateResumeElementsEnumUi.EDIT_TEXT_FORNAME_LATIN.title,
                elementEnum = CertificateResumeElementsEnumUi.EDIT_TEXT_FORNAME_LATIN,
                validators = listOf(
                    NonEmptyEditTextValidator(),
                    MinLengthEditTextValidator(minLength = NAMES_MIN_LENGTH),
                    FirstUpperCasedValidator()
                )
            )
        )
        add(
            CommonEditTextUi(
                required = false,
                question = false,
                minSymbols = 3,
                selectedValue = from?.secondNameLatin,
                type = CommonEditTextUiType.TEXT_INPUT_CAP,
                title = CertificateResumeElementsEnumUi.EDIT_TEXT_MIDDLENAME_LATIN.title,
                elementEnum = CertificateResumeElementsEnumUi.EDIT_TEXT_MIDDLENAME_LATIN,
                validators = listOf(
                    NonEmptyEditTextValidator(),
                    MinLengthEditTextValidator(minLength = NAMES_MIN_LENGTH),
                    FirstUpperCasedValidator()
                )
            )
        )
        add(
            CommonEditTextUi(
                required = true,
                question = false,
                minSymbols = 3,
                selectedValue = from?.lastNameLatin,
                type = CommonEditTextUiType.TEXT_INPUT_CAP,
                title = CertificateResumeElementsEnumUi.EDIT_TEXT_SURNAME_LATIN.title,
                elementEnum = CertificateResumeElementsEnumUi.EDIT_TEXT_SURNAME_LATIN,
                validators = listOf(
                    NonEmptyEditTextValidator(),
                    MinLengthEditTextValidator(minLength = NAMES_MIN_LENGTH),
                    FirstUpperCasedValidator()
                )
            )
        )
        add(
            CommonDatePickerUi(
                required = true,
                question = false,
                selectedValue = null,
                dateFormat = UiDateFormats.WITHOUT_TIME,
                maxDate = getCalendar(minusYears = MIN_AGE_YEARS),
                minDate = getCalendar(minusYears = 100),
                title = CertificateResumeElementsEnumUi.DATE_PICKER_DATE_OF_BIRTH.title,
                elementEnum = CertificateResumeElementsEnumUi.DATE_PICKER_DATE_OF_BIRTH,
                validators = listOf(
                    NonEmptyDatePickerValidator()
                )
            )
        )
        add(
            CommonSpinnerUi(
                required = true,
                question = false,
                title = CertificateResumeElementsEnumUi.SPINNER_IDENTIFIER_TYPE.title,
                elementEnum = CertificateResumeElementsEnumUi.SPINNER_IDENTIFIER_TYPE,
                selectedValue = from?.citizenIdentifierType?.let { value ->
                    val identifierType =
                        getEnumValue<CertificateResumeIdentifierTypeEnum>(value)
                            ?: CertificateResumeIdentifierTypeEnum.EGN
                    CommonSpinnerMenuItemUi(
                        isSelected = false,
                        text = identifierType.title,
                        serverValue = identifierType.type,
                        elementEnum = identifierType,
                    )
                },
                list = CertificateResumeIdentifierTypeEnum.entries.map {
                    CommonSpinnerMenuItemUi(
                        isSelected = false,
                        text = it.title,
                        serverValue = it.type,
                        elementEnum = it,
                    )
                },
                validators = listOf(
                    NonEmptySpinnerValidator()
                )
            )
        )
        add(
            CommonEditTextUi(
                required = true,
                question = false,
                minSymbols = 3,
                selectedValue = from?.citizenIdentifierNumber,
                type = CommonEditTextUiType.NUMBERS,
                title = CertificateResumeElementsEnumUi.EDIT_TEXT_IDENTIFIER.title,
                elementEnum = CertificateResumeElementsEnumUi.EDIT_TEXT_IDENTIFIER,
                validators = listOf(
                    NonEmptyEditTextValidator(),
                    MinLengthEditTextValidator(minLength = 3),
                    PersonalIdentifierValidator(),
                )
            )
        )
        add(
            CommonEditTextUi(
                required = true,
                question = false,
                selectedValue = null,
                title = CertificateResumeElementsEnumUi.EDIT_TEXT_CITIZENSHIP.title,
                elementEnum = CertificateResumeElementsEnumUi.EDIT_TEXT_CITIZENSHIP,
                type = CommonEditTextUiType.TEXT_INPUT_CAP,
                validators = listOf(
                    NonEmptyEditTextValidator()
                )
            )
        )
        add(
            CommonTitleSmallUi(
                title = StringSource(R.string.certificate_resume_personal_document_title),
            )
        )
        add(
            CommonEditTextUi(
                required = true,
                question = false,
                maxSymbols = 9,
                selectedValue = null,
                type = CommonEditTextUiType.TEXT_INPUT_CAP_ALL,
                title = CertificateResumeElementsEnumUi.EDIT_TEXT_DOCUMENT_NUMBER.title,
                elementEnum = CertificateResumeElementsEnumUi.EDIT_TEXT_DOCUMENT_NUMBER,
                validators = listOf(
                    NonEmptyEditTextValidator(),
                    BulgarianCardDocumentWithChipValidator()
                )
            )
        )
        add(
            CommonSpinnerUi(
                required = true,
                question = false,
                title = CertificateResumeElementsEnumUi.SPINNER_DOCUMENT_TYPE.title,
                elementEnum = CertificateResumeElementsEnumUi.SPINNER_DOCUMENT_TYPE,
                selectedValue = null,
                list = CertificateResumeDeviceTypeEnum.entries.map {
                    CommonSpinnerMenuItemUi(
                        isSelected = false,
                        text = it.title,
                        serverValue = it.type,
                        elementEnum = it,
                    )
                },
                validators = listOf(
                    NonEmptySpinnerValidator()
                )
            )
        )
        add(
            CommonDatePickerUi(
                required = true,
                question = false,
                selectedValue = null,
                dateFormat = UiDateFormats.WITHOUT_TIME,
                maxDate = getCalendar(),
                minDate = getCalendar(minusYears = 100),
                title = CertificateResumeElementsEnumUi.DATE_PICKER_ISSUED_ON.title,
                elementEnum = CertificateResumeElementsEnumUi.DATE_PICKER_ISSUED_ON,
                validators = listOf(
                    NonEmptyDatePickerValidator()
                )
            )
        )
        add(
            CommonDatePickerUi(
                required = true,
                question = false,
                selectedValue = null,
                dateFormat = UiDateFormats.WITHOUT_TIME,
                maxDate = getCalendar(plusYears = 100),
                minDate = getCalendar(),
                title = CertificateResumeElementsEnumUi.DATE_PICKER_VALID_UNTIL.title,
                elementEnum = CertificateResumeElementsEnumUi.DATE_PICKER_VALID_UNTIL,
                validators = listOf(
                    NonEmptyDatePickerValidator()
                )
            )
        )
        if (data == UserAcrEnum.LOW) {
            add(
                CommonSpinnerUi(
                    required = true,
                    question = false,
                    title = CertificateResumeElementsEnumUi.SPINNER_SIGNING_TYPE.title,
                    elementEnum = CertificateResumeElementsEnumUi.SPINNER_SIGNING_TYPE,
                    selectedValue = CommonSpinnerMenuItemUi(
                        isSelected = false,
                        text = CertificateResumeSigningMethodsEnumUi.EVROTRUST.title,
                        serverValue = CertificateResumeSigningMethodsEnumUi.EVROTRUST.type,
                        elementEnum = CertificateResumeSigningMethodsEnumUi.EVROTRUST,
                    ),
                    list = CertificateResumeSigningMethodsEnumUi.entries.map {
                        CommonSpinnerMenuItemUi(
                            text = it.title,
                            elementEnum = it,
                            isSelected = false,
                            serverValue = it.type,
                        )
                    }
                )
            )
        }
        add(
            CommonButtonUi(
                title = CertificateResumeElementsEnumUi.BUTTON_BACK.title,
                elementEnum = CertificateResumeElementsEnumUi.BUTTON_BACK,
                buttonColor = ButtonColorUi.TRANSPARENT,
            )
        )
        add(
            CommonButtonUi(
                title = CertificateResumeElementsEnumUi.BUTTON_CONFIRM.title,
                elementEnum = CertificateResumeElementsEnumUi.BUTTON_CONFIRM,
                buttonColor = ButtonColorUi.BLUE,
            )
        )
    }
}