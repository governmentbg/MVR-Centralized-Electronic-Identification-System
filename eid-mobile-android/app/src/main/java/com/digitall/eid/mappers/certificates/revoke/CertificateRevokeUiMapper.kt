package com.digitall.eid.mappers.certificates.revoke

import com.digitall.eid.R
import com.digitall.eid.data.mappers.base.BaseMapperWithData
import com.digitall.eid.domain.MIN_AGE_YEARS
import com.digitall.eid.domain.NAMES_MIN_LENGTH
import com.digitall.eid.domain.UiDateFormats
import com.digitall.eid.domain.extensions.getCalendar
import com.digitall.eid.domain.extensions.getEnumValue
import com.digitall.eid.domain.models.user.UserAcrEnum
import com.digitall.eid.models.certificates.resume.CertificateResumeSigningMethodsEnumUi
import com.digitall.eid.models.certificates.revoke.CertificateRevokeAdapterMarker
import com.digitall.eid.models.certificates.revoke.CertificateRevokeElementsEnumUi
import com.digitall.eid.models.certificates.revoke.CertificateRevokeIdentifierTypeEnum
import com.digitall.eid.models.certificates.revoke.CertificateRevokeSigningMethodsEnumUi
import com.digitall.eid.models.certificates.revoke.CertificateRevokeUiModel
import com.digitall.eid.models.certificates.stop.CertificateStopDeviceTypeEnum
import com.digitall.eid.models.certificates.stop.CertificateStopElementsEnumUi
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
import com.digitall.eid.models.list.CommonTextFieldUi
import com.digitall.eid.models.list.CommonTitleSmallUi
import com.digitall.eid.models.list.CommonTitleUi

class CertificateRevokeUiMapper :
    BaseMapperWithData<CertificateRevokeUiModel, UserAcrEnum, List<CertificateRevokeAdapterMarker>>() {

    override fun map(
        from: CertificateRevokeUiModel,
        data: UserAcrEnum?
    ) = buildList {
        add(
            CommonTitleUi(
                title = StringSource(R.string.certificate_revoke_title),
            )
        )
        add(
            CommonSeparatorUi()
        )
        val reason = from.revokeReasons?.first()
        add(
            CommonTextFieldUi(
                required = false,
                question = false,
                elementEnum = CertificateStopElementsEnumUi.TEXT_VIEW_REASON,
                text = reason?.description?.let { StringSource(it) }
                    ?: run { StringSource(R.string.unknown) },
                title = CertificateStopElementsEnumUi.TEXT_VIEW_REASON.title,
                originalModel = reason
            )
        )
        add(
            CommonTitleSmallUi(
                title = StringSource(R.string.certificate_stop_personal_data_title),
            )
        )
        add(
            CommonEditTextUi(
                required = true,
                question = false,
                minSymbols = 3,
                selectedValue = from.userModel?.firstName,
                type = CommonEditTextUiType.TEXT_INPUT_CAP,
                title = CertificateRevokeElementsEnumUi.EDIT_TEXT_FORNAME.title,
                elementEnum = CertificateRevokeElementsEnumUi.EDIT_TEXT_FORNAME,
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
                selectedValue = from.userModel?.secondName,
                type = CommonEditTextUiType.TEXT_INPUT_CAP,
                title = CertificateRevokeElementsEnumUi.EDIT_TEXT_MIDDLENAME.title,
                elementEnum = CertificateRevokeElementsEnumUi.EDIT_TEXT_MIDDLENAME,
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
                selectedValue = from.userModel?.lastName,
                type = CommonEditTextUiType.TEXT_INPUT_CAP,
                title = CertificateRevokeElementsEnumUi.EDIT_TEXT_SURNAME.title,
                elementEnum = CertificateRevokeElementsEnumUi.EDIT_TEXT_SURNAME,
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
                selectedValue = from.userModel?.firstNameLatin,
                type = CommonEditTextUiType.TEXT_INPUT_CAP,
                title = CertificateRevokeElementsEnumUi.EDIT_TEXT_FORNAME_LATIN.title,
                elementEnum = CertificateRevokeElementsEnumUi.EDIT_TEXT_FORNAME_LATIN,
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
                selectedValue = from.userModel?.secondNameLatin,
                type = CommonEditTextUiType.TEXT_INPUT_CAP,
                title = CertificateRevokeElementsEnumUi.EDIT_TEXT_MIDDLENAME_LATIN.title,
                elementEnum = CertificateRevokeElementsEnumUi.EDIT_TEXT_MIDDLENAME_LATIN,
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
                selectedValue = from.userModel?.lastNameLatin,
                type = CommonEditTextUiType.TEXT_INPUT_CAP,
                title = CertificateRevokeElementsEnumUi.EDIT_TEXT_SURNAME_LATIN.title,
                elementEnum = CertificateRevokeElementsEnumUi.EDIT_TEXT_SURNAME_LATIN,
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
                title = CertificateRevokeElementsEnumUi.DATE_PICKER_DATE_OF_BIRTH.title,
                elementEnum = CertificateRevokeElementsEnumUi.DATE_PICKER_DATE_OF_BIRTH,
                validators = listOf(
                    NonEmptyDatePickerValidator()
                )
            )
        )
        add(
            CommonTitleSmallUi(
                title = StringSource(R.string.certificate_stop_personal_document_title),
            )
        )
        add(
            CommonSpinnerUi(
                required = true,
                question = false,
                title = CertificateRevokeElementsEnumUi.SPINNER_IDENTIFIER_TYPE.title,
                elementEnum = CertificateRevokeElementsEnumUi.SPINNER_IDENTIFIER_TYPE,
                selectedValue = from.userModel?.citizenIdentifierType?.let { value ->
                    val identifierType =
                        getEnumValue<CertificateRevokeIdentifierTypeEnum>(value)
                            ?: CertificateRevokeIdentifierTypeEnum.EGN
                    CommonSpinnerMenuItemUi(
                        isSelected = false,
                        text = identifierType.title,
                        serverValue = identifierType.type,
                        elementEnum = identifierType,
                    )
                },
                list = CertificateRevokeIdentifierTypeEnum.entries.map {
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
                selectedValue = from.userModel?.citizenIdentifierNumber,
                type = CommonEditTextUiType.NUMBERS,
                title = CertificateRevokeElementsEnumUi.EDIT_TEXT_IDENTIFIER.title,
                elementEnum = CertificateRevokeElementsEnumUi.EDIT_TEXT_IDENTIFIER,
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
                title = CertificateRevokeElementsEnumUi.EDIT_TEXT_CITIZENSHIP.title,
                elementEnum = CertificateRevokeElementsEnumUi.EDIT_TEXT_CITIZENSHIP,
                type = CommonEditTextUiType.TEXT_INPUT_CAP,
                validators = listOf(
                    NonEmptyEditTextValidator(),
                )
            )
        )
        add(
            CommonEditTextUi(
                required = true,
                question = false,
                maxSymbols = 9,
                selectedValue = null,
                type = CommonEditTextUiType.TEXT_INPUT_CAP_ALL,
                title = CertificateRevokeElementsEnumUi.EDIT_TEXT_DOCUMENT_NUMBER.title,
                elementEnum = CertificateRevokeElementsEnumUi.EDIT_TEXT_DOCUMENT_NUMBER,
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
                title = CertificateRevokeElementsEnumUi.SPINNER_DOCUMENT_TYPE.title,
                elementEnum = CertificateRevokeElementsEnumUi.SPINNER_DOCUMENT_TYPE,
                selectedValue = null,
                list = CertificateStopDeviceTypeEnum.entries.map {
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
                title = CertificateRevokeElementsEnumUi.DATE_PICKER_ISSUED_ON.title,
                elementEnum = CertificateRevokeElementsEnumUi.DATE_PICKER_ISSUED_ON,
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
                title = CertificateRevokeElementsEnumUi.DATE_PICKER_VALID_UNTIL.title,
                elementEnum = CertificateRevokeElementsEnumUi.DATE_PICKER_VALID_UNTIL,
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
                    title = CertificateRevokeElementsEnumUi.SPINNER_SIGNING_TYPE.title,
                    elementEnum = CertificateRevokeElementsEnumUi.SPINNER_SIGNING_TYPE,
                    selectedValue = CommonSpinnerMenuItemUi(
                        isSelected = false,
                        text = CertificateResumeSigningMethodsEnumUi.EVROTRUST.title,
                        serverValue = CertificateResumeSigningMethodsEnumUi.EVROTRUST.type,
                        elementEnum = CertificateResumeSigningMethodsEnumUi.EVROTRUST,
                    ),
                    list = CertificateRevokeSigningMethodsEnumUi.entries.map {
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
                title = CertificateRevokeElementsEnumUi.BUTTON_BACK.title,
                elementEnum = CertificateRevokeElementsEnumUi.BUTTON_BACK,
                buttonColor = ButtonColorUi.TRANSPARENT,
            )
        )
        add(
            CommonButtonUi(
                title = CertificateRevokeElementsEnumUi.BUTTON_CONFIRM.title,
                elementEnum = CertificateRevokeElementsEnumUi.BUTTON_CONFIRM,
                buttonColor = ButtonColorUi.BLUE,
            )
        )
    }
}