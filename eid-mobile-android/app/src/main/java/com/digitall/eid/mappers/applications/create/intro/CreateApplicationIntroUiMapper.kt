package com.digitall.eid.mappers.applications.create.intro

import com.digitall.eid.R
import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.mappers.base.BaseMapperWithData
import com.digitall.eid.domain.APPLICATION_LANGUAGE
import com.digitall.eid.domain.DEBUG_APPLICATION_CREATE_BORN_DATE
import com.digitall.eid.domain.DEBUG_APPLICATION_CREATE_DOCUMENT_CREATED
import com.digitall.eid.domain.DEBUG_APPLICATION_CREATE_DOCUMENT_NUMBER
import com.digitall.eid.domain.DEBUG_APPLICATION_CREATE_DOCUMENT_VALID
import com.digitall.eid.domain.DEBUG_APPLICATION_CREATE_EGN
import com.digitall.eid.domain.DEBUG_APPLICATION_CREATE_FIRST_LATIN_NAME
import com.digitall.eid.domain.DEBUG_APPLICATION_CREATE_LAST_LATIN_NAME
import com.digitall.eid.domain.DEBUG_APPLICATION_CREATE_SECOND_LATIN_NAME
import com.digitall.eid.domain.MIN_AGE_YEARS
import com.digitall.eid.domain.NAMES_MIN_LENGTH
import com.digitall.eid.domain.UiDateFormats
import com.digitall.eid.domain.extensions.getCalendar
import com.digitall.eid.domain.extensions.getEnumValue
import com.digitall.eid.domain.models.applications.create.ApplicationCreateInitialInformationModel
import com.digitall.eid.domain.models.common.ApplicationLanguage
import com.digitall.eid.domain.models.user.UserAcrEnum
import com.digitall.eid.models.applications.all.ApplicationCitizenIdentifierTypeEnum
import com.digitall.eid.models.applications.all.ApplicationDocumentTypeEnum
import com.digitall.eid.models.applications.create.ApplicationCreateIntroAdapterMarker
import com.digitall.eid.models.applications.create.ApplicationCreateIntroElementsEnumUi
import com.digitall.eid.models.applications.create.ApplicationCreateIntroSigningMethodsEnumUi
import com.digitall.eid.models.common.ButtonColorUi
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.common.validator.FirstUpperCasedValidator
import com.digitall.eid.models.common.validator.MinLengthEditTextValidator
import com.digitall.eid.models.common.validator.NonEmptyDatePickerValidator
import com.digitall.eid.models.common.validator.NonEmptyDialogWithSearchItemValidator
import com.digitall.eid.models.common.validator.NonEmptyEditTextValidator
import com.digitall.eid.models.common.validator.PersonalIdentifierValidator
import com.digitall.eid.models.common.validator.SharedNameDependencyValidator
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonDatePickerUi
import com.digitall.eid.models.list.CommonDialogWithSearchItemUi
import com.digitall.eid.models.list.CommonDialogWithSearchUi
import com.digitall.eid.models.list.CommonEditTextUi
import com.digitall.eid.models.list.CommonEditTextUiType
import com.digitall.eid.models.list.CommonSeparatorUi
import com.digitall.eid.models.list.CommonSimpleTextUi
import com.digitall.eid.models.list.CommonSpinnerMenuItemUi
import com.digitall.eid.models.list.CommonSpinnerUi
import com.digitall.eid.models.list.CommonTextFieldUi
import com.digitall.eid.models.list.CommonTitleBigUi
import com.digitall.eid.models.list.CommonTitleSmallUi
import com.digitall.eid.models.list.CommonTitleUi

class CreateApplicationIntroUiMapper :
    BaseMapperWithData<ApplicationCreateInitialInformationModel, UserAcrEnum, List<ApplicationCreateIntroAdapterMarker>>() {

    override fun map(
        from: ApplicationCreateInitialInformationModel,
        data: UserAcrEnum?
    ): List<ApplicationCreateIntroAdapterMarker> {
        return buildList {
            add(
                CommonTitleUi(
                    title = StringSource(R.string.create_application_title),
                )
            )
            add(
                CommonSeparatorUi()
            )
            add(
                CommonTitleBigUi(
                    title = StringSource("${from.userModel?.firstName} ${from.userModel?.secondName} ${from.userModel?.lastName}"),
                    description = StringSource("${from.userModel?.email}"),
                )
            )
            add(CommonSeparatorUi())
            // personal data
            add(
                CommonTitleSmallUi(
                    title = StringSource(R.string.create_application_personal_data_title),
                )
            )
            val firstNameLatinField = CommonEditTextUi(
                required = true,
                question = false,
                minSymbols = 3,
                selectedValue = from.userModel?.firstNameLatin
                    ?: DEBUG_APPLICATION_CREATE_FIRST_LATIN_NAME,
                type = CommonEditTextUiType.TEXT_INPUT_CAP,
                title = ApplicationCreateIntroElementsEnumUi.EDIT_TEXT_FIRST_LATIN_NAME.title,
                elementEnum = ApplicationCreateIntroElementsEnumUi.EDIT_TEXT_FIRST_LATIN_NAME,
                validators = listOf(
                    NonEmptyEditTextValidator(),
                    MinLengthEditTextValidator(minLength = NAMES_MIN_LENGTH),
                    FirstUpperCasedValidator()
                )
            )

            val middleNameLatinField = CommonEditTextUi(
                required = false,
                question = false,
                minSymbols = 3,
                selectedValue = from.userModel?.secondNameLatin
                    ?: DEBUG_APPLICATION_CREATE_SECOND_LATIN_NAME,
                type = CommonEditTextUiType.TEXT_INPUT_CAP,
                title = ApplicationCreateIntroElementsEnumUi.EDIT_TEXT_SECOND_LATIN_NAME.title,
                elementEnum = ApplicationCreateIntroElementsEnumUi.EDIT_TEXT_SECOND_LATIN_NAME,
                validators = listOf(
                    MinLengthEditTextValidator(minLength = NAMES_MIN_LENGTH),
                    FirstUpperCasedValidator()
                )
            )

            val lastNameLatinField = CommonEditTextUi(
                required = true,
                question = false,
                minSymbols = 3,
                selectedValue = from.userModel?.lastNameLatin
                    ?: DEBUG_APPLICATION_CREATE_LAST_LATIN_NAME,
                type = CommonEditTextUiType.TEXT_INPUT_CAP,
                title = ApplicationCreateIntroElementsEnumUi.EDIT_TEXT_LAST_LATIN_NAME.title,
                elementEnum = ApplicationCreateIntroElementsEnumUi.EDIT_TEXT_LAST_LATIN_NAME
            )

            add(
                firstNameLatinField
            )
            add(
                middleNameLatinField.copy(
                    validators = listOf(
                        MinLengthEditTextValidator(minLength = NAMES_MIN_LENGTH),
                        FirstUpperCasedValidator(),
                        SharedNameDependencyValidator(
                            primaryNameProvider = { firstNameLatinField.selectedValue },
                            siblingNameProvider = { lastNameLatinField.selectedValue }
                        )
                    )
                )
            )
            add(
                lastNameLatinField.copy(
                    validators = listOf(
                        MinLengthEditTextValidator(minLength = NAMES_MIN_LENGTH),
                        FirstUpperCasedValidator(),
                        SharedNameDependencyValidator(
                            primaryNameProvider = { firstNameLatinField.selectedValue },
                            siblingNameProvider = { middleNameLatinField.selectedValue }
                        )
                    )
                )
            )
            add(
                CommonEditTextUi(
                    required = true,
                    question = false,
                    title = ApplicationCreateIntroElementsEnumUi.EDIT_TEXT_CITIZENSHIP.title,
                    elementEnum = ApplicationCreateIntroElementsEnumUi.EDIT_TEXT_CITIZENSHIP,
                    selectedValue = null,
                    type = CommonEditTextUiType.TEXT_INPUT_CAP,
                    validators = listOf(
                        NonEmptyEditTextValidator(),
                    )
                )
            )
            val selectedIdType = getEnumValue<ApplicationCitizenIdentifierTypeEnum>(
                from.userModel?.citizenIdentifierType ?: ""
            ) ?: ApplicationCitizenIdentifierTypeEnum.EGN
            add(
                CommonSpinnerUi(
                    required = true,
                    question = false,
                    title = ApplicationCreateIntroElementsEnumUi.SPINNER_ID_TYPE.title,
                    elementEnum = ApplicationCreateIntroElementsEnumUi.SPINNER_ID_TYPE,
                    selectedValue = CommonSpinnerMenuItemUi(
                        isSelected = false,
                        text = selectedIdType.title,
                        serverValue = selectedIdType.type,
                        elementEnum = selectedIdType,
                    ),
                    list = ApplicationCitizenIdentifierTypeEnum.entries.map {
                        CommonSpinnerMenuItemUi(
                            text = it.title,
                            elementEnum = it,
                            isSelected = false,
                            serverValue = it.type,
                        )
                    }
                )
            )
            add(
                CommonEditTextUi(
                    required = true,
                    question = false,
                    minSymbols = 8,
                    maxSymbols = 12,
                    selectedValue = from.userModel?.citizenIdentifierNumber
                        ?: DEBUG_APPLICATION_CREATE_EGN,
                    type = CommonEditTextUiType.NUMBERS,
                    title = ApplicationCreateIntroElementsEnumUi.EDIT_TEXT_ID_NUMBER.title,
                    elementEnum = ApplicationCreateIntroElementsEnumUi.EDIT_TEXT_ID_NUMBER,
                    validators = listOf(
                        NonEmptyEditTextValidator(),
                        MinLengthEditTextValidator(minLength = 8),
                        PersonalIdentifierValidator(),
                    )
                )
            )
            add(
                CommonDatePickerUi(
                    required = true,
                    question = false,
                    selectedValue = DEBUG_APPLICATION_CREATE_BORN_DATE?.let {
                        getCalendar(timeInMillis = it)
                    },
                    dateFormat = UiDateFormats.WITHOUT_TIME,
                    maxDate = getCalendar(minusYears = MIN_AGE_YEARS),
                    minDate = getCalendar(minusYears = 100),
                    title = ApplicationCreateIntroElementsEnumUi.DATE_PICKER_BORN_DATE.title,
                    elementEnum = ApplicationCreateIntroElementsEnumUi.DATE_PICKER_BORN_DATE,
                    validators = listOf(
                        NonEmptyDatePickerValidator()
                    )
                )
            )
            // document
            add(
                CommonSpinnerUi(
                    required = true,
                    question = false,
                    title = ApplicationCreateIntroElementsEnumUi.SPINNER_DOCUMENT_TYPE.title,
                    elementEnum = ApplicationCreateIntroElementsEnumUi.SPINNER_DOCUMENT_TYPE,
                    selectedValue = CommonSpinnerMenuItemUi(
                        isSelected = false,
                        text = ApplicationDocumentTypeEnum.IDENTITY_CARD.title,
                        serverValue = ApplicationDocumentTypeEnum.IDENTITY_CARD.type,
                        elementEnum = ApplicationDocumentTypeEnum.IDENTITY_CARD,
                    ),
                    list = ApplicationDocumentTypeEnum.entries.map {
                        CommonSpinnerMenuItemUi(
                            text = it.title,
                            elementEnum = it,
                            isSelected = false,
                            serverValue = it.type,
                        )
                    }
                )
            )
            add(
                CommonEditTextUi(
                    required = true,
                    question = false,
                    maxSymbols = 9,
                    selectedValue = DEBUG_APPLICATION_CREATE_DOCUMENT_NUMBER,
                    type = CommonEditTextUiType.TEXT_INPUT_CAP_ALL,
                    title = ApplicationCreateIntroElementsEnumUi.EDIT_TEXT_DOCUMENT_NUMBER.title,
                    elementEnum = ApplicationCreateIntroElementsEnumUi.EDIT_TEXT_DOCUMENT_NUMBER,
                    validators = listOf(
                        NonEmptyEditTextValidator(),
                    )
                )
            )
            add(
                CommonDatePickerUi(
                    required = true,
                    question = false,
                    selectedValue = DEBUG_APPLICATION_CREATE_DOCUMENT_CREATED?.let {
                        getCalendar(timeInMillis = it)
                    },
                    dateFormat = UiDateFormats.WITHOUT_TIME,
                    maxDate = getCalendar(),
                    minDate = getCalendar(minusYears = 100),
                    title = ApplicationCreateIntroElementsEnumUi.DATE_PICKER_CREATED_ON.title,
                    elementEnum = ApplicationCreateIntroElementsEnumUi.DATE_PICKER_CREATED_ON,
                    validators = listOf(
                        NonEmptyDatePickerValidator()
                    )
                )
            )
            add(
                CommonDatePickerUi(
                    required = true,
                    question = false,
                    selectedValue = DEBUG_APPLICATION_CREATE_DOCUMENT_VALID?.let {
                        getCalendar(timeInMillis = it)
                    },
                    dateFormat = UiDateFormats.WITHOUT_TIME,
                    maxDate = getCalendar(plusYears = 100),
                    minDate = getCalendar(),
                    title = ApplicationCreateIntroElementsEnumUi.DATE_PICKER_VALID_TO.title,
                    elementEnum = ApplicationCreateIntroElementsEnumUi.DATE_PICKER_VALID_TO,
                    validators = listOf(
                        NonEmptyDatePickerValidator()
                    )
                )
            )
            // additional information
            add(
                CommonTitleSmallUi(
                    title = StringSource(R.string.create_application_additional_information_title),
                )
            )
            val language = APPLICATION_LANGUAGE
            add(
                CommonDialogWithSearchUi(
                    required = true,
                    question = false,
                    selectedValue = null,
                    title = ApplicationCreateIntroElementsEnumUi.DIALOG_ADMINISTRATOR.title,
                    elementEnum = ApplicationCreateIntroElementsEnumUi.DIALOG_ADMINISTRATOR,
                    list = from.administrators?.map { data ->
                        CommonDialogWithSearchItemUi(
                            serverValue = data.id,
                            originalModel = data,
                            text = when (language) {
                                ApplicationLanguage.BG -> StringSource(data.name ?: "")
                                ApplicationLanguage.EN -> StringSource(data.nameLatin ?: "")
                            },
                            elementEnum = ApplicationCreateIntroElementsEnumUi.DIALOG_ADMINISTRATOR,
                        )

                    }?.takeIf { list -> list.isNotEmpty() } ?: listOf(
                        CommonDialogWithSearchItemUi(
                            text = StringSource(R.string.no_search_results),
                            selectable = false
                        )
                    ),
                    validators = listOf(
                        NonEmptyDialogWithSearchItemValidator()
                    )
                )
            )
            add(
                CommonTextFieldUi(
                    required = true,
                    question = false,
                    title = ApplicationCreateIntroElementsEnumUi.SPINNER_DEVICE_TYPE.title,
                    elementEnum = ApplicationCreateIntroElementsEnumUi.SPINNER_DEVICE_TYPE,
                    text = StringSource(R.string.create_application_choose_administrator_warning),
                )
            )
            add(
                CommonTextFieldUi(
                    required = true,
                    question = false,
                    title = ApplicationCreateIntroElementsEnumUi.DIALOG_ADMINISTRATOR_OFFICE.title,
                    elementEnum = ApplicationCreateIntroElementsEnumUi.DIALOG_ADMINISTRATOR_OFFICE,
                    text = StringSource(R.string.create_application_choose_carrier_warning),
                )
            )
            add(
                CommonSimpleTextUi(
                    title = ApplicationCreateIntroElementsEnumUi.TEXT_VIEW_COMMENT.title,
                    elementEnum = ApplicationCreateIntroElementsEnumUi.TEXT_VIEW_COMMENT,
                    text = StringSource(R.string.create_application_comment),
                    maxLines = 11,
                )
            )
            if (data == UserAcrEnum.LOW) {
                add(
                    CommonSpinnerUi(
                        required = true,
                        question = false,
                        title = ApplicationCreateIntroElementsEnumUi.SPINNER_AUTH_TYPE.title,
                        elementEnum = ApplicationCreateIntroElementsEnumUi.SPINNER_AUTH_TYPE,
                        selectedValue = CommonSpinnerMenuItemUi(
                            isSelected = false,
                            text = ApplicationCreateIntroSigningMethodsEnumUi.EVROTRUST.title,
                            serverValue = ApplicationCreateIntroSigningMethodsEnumUi.EVROTRUST.type,
                            elementEnum = ApplicationCreateIntroSigningMethodsEnumUi.EVROTRUST,
                        ),
                        list = ApplicationCreateIntroSigningMethodsEnumUi.entries.map {
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
            // buttons
            add(
                CommonButtonUi(
                    title = ApplicationCreateIntroElementsEnumUi.BUTTON_APPLY.title,
                    elementEnum = ApplicationCreateIntroElementsEnumUi.BUTTON_APPLY,
                    buttonColor = ButtonColorUi.TRANSPARENT,
                )
            )
            add(
                CommonButtonUi(
                    title = ApplicationCreateIntroElementsEnumUi.BUTTON_CANCEL.title,
                    elementEnum = ApplicationCreateIntroElementsEnumUi.BUTTON_CANCEL,
                    buttonColor = ButtonColorUi.RED,
                )
            )
        }
    }
}