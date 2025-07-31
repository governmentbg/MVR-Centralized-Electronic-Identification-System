package com.digitall.eid.mappers.empowerment.create

import com.digitall.eid.R
import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.domain.NAMES_MIN_LENGTH
import com.digitall.eid.domain.UiDateFormats
import com.digitall.eid.domain.extensions.getCalendar
import com.digitall.eid.domain.extensions.getEnumValue
import com.digitall.eid.extensions.moveItemToFirstPosition
import com.digitall.eid.models.common.ButtonColorUi
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.common.validator.BulstatValidator
import com.digitall.eid.models.common.validator.ExistingValueEditTextValidator
import com.digitall.eid.models.common.validator.FirstUpperCasedValidator
import com.digitall.eid.models.common.validator.MinLengthEditTextValidator
import com.digitall.eid.models.common.validator.NonEmptyDatePickerValidator
import com.digitall.eid.models.common.validator.NonEmptyEditTextValidator
import com.digitall.eid.models.common.validator.NonEmptySpinnerValidator
import com.digitall.eid.models.common.validator.PersonalIdentifierValidator
import com.digitall.eid.models.empowerment.create.create.EmpowermentCreateAdapterMarker
import com.digitall.eid.models.empowerment.create.create.EmpowermentCreateElementsEnumUi
import com.digitall.eid.models.empowerment.create.create.EmpowermentCreateFromNameOfEnumUi
import com.digitall.eid.models.empowerment.create.create.EmpowermentCreateIdTypeEnumUi
import com.digitall.eid.models.empowerment.create.create.EmpowermentCreateUiModel
import com.digitall.eid.models.list.CommonButtonTransparentUi
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

class CreateEmpowermentFromCompanyUiMapper: BaseMapper<EmpowermentCreateUiModel, List<EmpowermentCreateAdapterMarker>>() {

    override fun map(from: EmpowermentCreateUiModel) = buildList {
        add(
            CommonTitleUi(
                title = StringSource(R.string.empowerment_create_submit_application_title),
            )
        )
        add(
            CommonSeparatorUi()
        )
        add(
            CommonTitleSmallUi(
                title = StringSource(R.string.empowerment_create_applicant_title)
            )
        )
        add(
            CommonSpinnerUi(
                required = true,
                question = false,
                title = EmpowermentCreateElementsEnumUi.SPINNER_ON_BEHALF_OF.title,
                elementEnum = EmpowermentCreateElementsEnumUi.SPINNER_ON_BEHALF_OF,
                selectedValue = CommonSpinnerMenuItemUi(
                    isSelected = false,
                    text = EmpowermentCreateFromNameOfEnumUi.COMPANY.title,
                    serverValue = EmpowermentCreateFromNameOfEnumUi.COMPANY.type,
                    elementEnum = EmpowermentCreateFromNameOfEnumUi.COMPANY,
                ),
                list = EmpowermentCreateFromNameOfEnumUi.entries.map {
                    CommonSpinnerMenuItemUi(
                        isSelected = false,
                        text = it.title,
                        serverValue = it.type,
                        elementEnum = it,
                    )
                },
            )
        )
        add(
            CommonEditTextUi(
                maxSymbols = 13,
                required = true,
                question = false,
                selectedValue = from.empowermentItem?.uid,
                type = CommonEditTextUiType.NUMBERS,
                title = EmpowermentCreateElementsEnumUi.EDIT_TEXT_COMPANY_NUMBER.title,
                elementEnum = EmpowermentCreateElementsEnumUi.EDIT_TEXT_COMPANY_NUMBER,
                validators = listOf(
                    NonEmptyEditTextValidator(),
                    BulstatValidator(),
                )
            )
        )
        add(
            CommonEditTextUi(
                required = true,
                question = false,
                selectedValue = from.empowermentItem?.name,
                type = CommonEditTextUiType.TEXT_INPUT,
                title = EmpowermentCreateElementsEnumUi.EDIT_TEXT_COMPANY_NAME.title,
                elementEnum = EmpowermentCreateElementsEnumUi.EDIT_TEXT_COMPANY_NAME,
                validators = listOf(
                    NonEmptyEditTextValidator(),
                )
            )
        )
        add(
            CommonEditTextUi(
                required = true,
                question = false,
                selectedValue = from.empowermentItem?.issuerPosition,
                type = CommonEditTextUiType.TEXT_INPUT,
                title = EmpowermentCreateElementsEnumUi.EDIT_TEXT_ISSUER_POSITION.title,
                elementEnum = EmpowermentCreateElementsEnumUi.EDIT_TEXT_ISSUER_POSITION,
                validators = listOf(
                    NonEmptyEditTextValidator(),
                )
            )
        )
        add(
            CommonTitleSmallUi(
                title = StringSource(R.string.empowerment_create_legal_representatives_title)
            )
        )
        val allAuthorizedPeopleUids = from.empowermentItem?.authorizerUids?.map { element -> element.uid }
        from.empowermentItem?.authorizerUids?.moveItemToFirstPosition { it.uid == from.user?.citizenIdentifier }
            ?.forEachIndexed { index, authorizedUiModel ->
                val uidTypeModel =
                    when (getEnumValue<EmpowermentCreateIdTypeEnumUi>(
                        authorizedUiModel.uidType ?: ""
                    )) {
                        EmpowermentCreateIdTypeEnumUi.EGN ->
                            CommonSpinnerMenuItemUi(
                                isSelected = false,
                                text = EmpowermentCreateIdTypeEnumUi.EGN.title,
                                elementEnum = EmpowermentCreateIdTypeEnumUi.EGN,
                                serverValue = EmpowermentCreateIdTypeEnumUi.EGN.type,
                            )

                        EmpowermentCreateIdTypeEnumUi.LNCH ->
                            CommonSpinnerMenuItemUi(
                                isSelected = false,
                                text = EmpowermentCreateIdTypeEnumUi.LNCH.title,
                                elementEnum = EmpowermentCreateIdTypeEnumUi.LNCH,
                                serverValue = EmpowermentCreateIdTypeEnumUi.LNCH.type,
                            )

                        null -> null
                    }
                when (index) {
                    0 -> {
                        add(
                            CommonTextFieldUi(
                                required = true,
                                question = false,
                                serverValue = authorizedUiModel.uidType,
                                title = EmpowermentCreateElementsEnumUi.TEXT_VIEW_UID_TYPE.title,
                                elementEnum = EmpowermentCreateElementsEnumUi.TEXT_VIEW_UID_TYPE,
                                text = uidTypeModel?.text
                                    ?: StringSource(authorizedUiModel.uidType),
                            )
                        )
                        add(
                            CommonTextFieldUi(
                                required = true,
                                question = false,
                                serverValue = authorizedUiModel.uid,
                                title = EmpowermentCreateElementsEnumUi.TEXT_VIEW_LEGAL_REPRESANTATIVE_UID_NUMBER.title,
                                elementEnum = EmpowermentCreateElementsEnumUi.TEXT_VIEW_LEGAL_REPRESANTATIVE_UID_NUMBER,
                                text = StringSource(authorizedUiModel.uid),
                            )
                        )
                        add(
                            CommonTextFieldUi(
                                required = true,
                                question = false,
                                serverValue = authorizedUiModel.name,
                                title = EmpowermentCreateElementsEnumUi.TEXT_VIEW_LEGAL_REPRESENTATIVE_NAMES.title,
                                elementEnum = EmpowermentCreateElementsEnumUi.TEXT_VIEW_LEGAL_REPRESENTATIVE_NAMES,
                                text = if (authorizedUiModel.name.isNullOrEmpty()) StringSource(
                                    from.user?.nameCyrillic
                                ) else
                                    StringSource(authorizedUiModel.name),
                            )
                        )
                    }

                    else -> {
                        add(
                            CommonSpinnerUi(
                                elementId = index,
                                required = true,
                                question = false,
                                title = EmpowermentCreateElementsEnumUi.SPINNER_LEGAL_REPRESENTATIVE_UID_TYPE.title,
                                elementEnum = EmpowermentCreateElementsEnumUi.SPINNER_LEGAL_REPRESENTATIVE_UID_TYPE,
                                selectedValue = uidTypeModel,
                                hasEraseButton = true,
                                list = EmpowermentCreateIdTypeEnumUi.entries.map {
                                    CommonSpinnerMenuItemUi(
                                        isSelected = false,
                                        text = it.title,
                                        elementEnum = it,
                                        serverValue = it.type,
                                    )
                                },
                                validators = listOf(
                                    NonEmptySpinnerValidator()
                                )
                            )
                        )
                        add(
                            CommonEditTextUi(
                                elementId = index,
                                required = true,
                                question = false,
                                selectedValue = authorizedUiModel.uid,
                                type = CommonEditTextUiType.NUMBERS,
                                title = EmpowermentCreateElementsEnumUi.EDIT_TEXT_LEGAL_REPRESENTATIVE_UID_NUMBER.title,
                                elementEnum = EmpowermentCreateElementsEnumUi.EDIT_TEXT_LEGAL_REPRESENTATIVE_UID_NUMBER,
                                validators = listOf(
                                    NonEmptyEditTextValidator(),
                                    PersonalIdentifierValidator(),
                                    ExistingValueEditTextValidator(
                                        existingValuesProvider = { allAuthorizedPeopleUids ?: emptyList() },
                                        errorMessage = StringSource(R.string.error_personal_identifier_already_used)
                                    )
                                )
                            )
                        )
                        add(
                            CommonEditTextUi(
                                elementId = index,
                                required = true,
                                question = false,
                                selectedValue = authorizedUiModel.name,
                                type = CommonEditTextUiType.TEXT_INPUT_CAP,
                                minSymbols = 3,
                                maxSymbols = 200,
                                title = EmpowermentCreateElementsEnumUi.EDIT_TEXT_LEGAL_REPRESENTATIVE_NAMES.title,
                                elementEnum = EmpowermentCreateElementsEnumUi.EDIT_TEXT_LEGAL_REPRESENTATIVE_NAMES,
                                validators = listOf(
                                    NonEmptyEditTextValidator(),
                                    MinLengthEditTextValidator(minLength = NAMES_MIN_LENGTH),
                                    FirstUpperCasedValidator(),
                                )
                            )
                        )
                    }
                }

            } ?: run {
            val uidTypeTitle =
                when (getEnumValue<EmpowermentCreateIdTypeEnumUi>(
                    from.user?.citizenIdentifierType ?: ""
                )) {
                    EmpowermentCreateIdTypeEnumUi.EGN -> EmpowermentCreateIdTypeEnumUi.EGN.title
                    EmpowermentCreateIdTypeEnumUi.LNCH -> EmpowermentCreateIdTypeEnumUi.LNCH.title
                    null -> StringSource(R.string.unknown)
                }

            add(
                CommonTextFieldUi(
                    required = true,
                    question = false,
                    serverValue = from.user?.citizenIdentifierType,
                    title = EmpowermentCreateElementsEnumUi.TEXT_VIEW_UID_TYPE.title,
                    elementEnum = EmpowermentCreateElementsEnumUi.TEXT_VIEW_UID_TYPE,
                    text = uidTypeTitle,
                )
            )
            add(
                CommonTextFieldUi(
                    required = true,
                    question = false,
                    serverValue = from.user?.citizenIdentifier,
                    title = EmpowermentCreateElementsEnumUi.TEXT_VIEW_LEGAL_REPRESANTATIVE_UID_NUMBER.title,
                    elementEnum = EmpowermentCreateElementsEnumUi.TEXT_VIEW_LEGAL_REPRESANTATIVE_UID_NUMBER,
                    text = StringSource(from.user?.citizenIdentifier),
                )
            )
            add(
                CommonTextFieldUi(
                    required = true,
                    question = false,
                    serverValue = from.user?.nameCyrillic,
                    title = EmpowermentCreateElementsEnumUi.TEXT_VIEW_LEGAL_REPRESENTATIVE_NAMES.title,
                    elementEnum = EmpowermentCreateElementsEnumUi.TEXT_VIEW_LEGAL_REPRESENTATIVE_NAMES,
                    text = StringSource(from.user?.nameCyrillic),
                )
            )
        }
        add(
            CommonButtonUi(
                title = EmpowermentCreateElementsEnumUi.BUTTON_ADD_LEGAL_REPRESENTATIVE.title,
                elementEnum = EmpowermentCreateElementsEnumUi.BUTTON_ADD_LEGAL_REPRESENTATIVE,
                buttonColor = ButtonColorUi.GREEN,
            )
        )
        add(
            CommonTitleSmallUi(
                title = StringSource(R.string.empowerment_create_empowered_persons_title)
            )
        )
        if (from.empowermentItem?.empoweredUids.isNullOrEmpty()) {
            add(
                CommonSpinnerUi(
                    elementId = 1,
                    required = true,
                    question = false,
                    title = EmpowermentCreateElementsEnumUi.SPINNER_EMPOWERED_UID_TYPE.title,
                    elementEnum = EmpowermentCreateElementsEnumUi.SPINNER_EMPOWERED_UID_TYPE,
                    selectedValue = null,
                    list = EmpowermentCreateIdTypeEnumUi.entries.map {
                        CommonSpinnerMenuItemUi(
                            isSelected = false,
                            text = it.title,
                            elementEnum = it,
                            serverValue = it.type,
                        )
                    },
                    validators = listOf(
                        NonEmptySpinnerValidator()
                    )
                )
            )
            add(
                CommonTextFieldUi(
                    elementId = 1,
                    required = true,
                    question = false,
                    title = EmpowermentCreateElementsEnumUi.EDIT_TEXT_EMPOWERED_UID_NUMBER.title,
                    elementEnum = EmpowermentCreateElementsEnumUi.EDIT_TEXT_EMPOWERED_UID_NUMBER,
                    text = StringSource(R.string.empowerment_create_choose_identifier_type_hint),
                )
            )
            add(
                CommonTextFieldUi(
                    elementId = 1,
                    required = true,
                    question = false,
                    title = EmpowermentCreateElementsEnumUi.EDIT_TEXT_EMPOWERED_NAMES.title,
                    elementEnum = EmpowermentCreateElementsEnumUi.EDIT_TEXT_EMPOWERED_NAMES,
                    text = StringSource(R.string.empowerment_create_choose_identifier_type_hint),
                )
            )
        } else {
            val allEmpoweredPeopleUids = from.empowermentItem?.empoweredUids?.map { element -> element.uid }
            from.empowermentItem?.empoweredUids?.forEachIndexed { index, value ->
                val uidTypeModel =
                    when (getEnumValue<EmpowermentCreateIdTypeEnumUi>(value.uidType ?: "")) {
                        EmpowermentCreateIdTypeEnumUi.EGN ->
                            CommonSpinnerMenuItemUi(
                                isSelected = false,
                                text = EmpowermentCreateIdTypeEnumUi.EGN.title,
                                elementEnum = EmpowermentCreateIdTypeEnumUi.EGN,
                                serverValue = EmpowermentCreateIdTypeEnumUi.EGN.type,
                            )

                        EmpowermentCreateIdTypeEnumUi.LNCH ->
                            CommonSpinnerMenuItemUi(
                                isSelected = false,
                                text = EmpowermentCreateIdTypeEnumUi.LNCH.title,
                                elementEnum = EmpowermentCreateIdTypeEnumUi.LNCH,
                                serverValue = EmpowermentCreateIdTypeEnumUi.LNCH.type,
                            )

                        null -> null
                    }
                add(
                    CommonSpinnerUi(
                        elementId = index + 1,
                        required = true,
                        question = false,
                        title = EmpowermentCreateElementsEnumUi.SPINNER_EMPOWERED_UID_TYPE.title,
                        elementEnum = EmpowermentCreateElementsEnumUi.SPINNER_EMPOWERED_UID_TYPE,
                        selectedValue = uidTypeModel,
                        list = listOf(
                            CommonSpinnerMenuItemUi(
                                isSelected = false,
                                text = EmpowermentCreateIdTypeEnumUi.EGN.title,
                                elementEnum = EmpowermentCreateIdTypeEnumUi.EGN,
                                serverValue = EmpowermentCreateIdTypeEnumUi.EGN.type,
                            ),
                            CommonSpinnerMenuItemUi(
                                isSelected = false,
                                text = EmpowermentCreateIdTypeEnumUi.LNCH.title,
                                elementEnum = EmpowermentCreateIdTypeEnumUi.LNCH,
                                serverValue = EmpowermentCreateIdTypeEnumUi.LNCH.type,
                            ),
                        ),
                        validators = listOf(
                            NonEmptySpinnerValidator()
                        )
                    )
                )
                add(
                    CommonEditTextUi(
                        elementId = index + 1,
                        required = true,
                        question = false,
                        selectedValue = value.uid,
                        maxSymbols = 10,
                        type = CommonEditTextUiType.NUMBERS,
                        title = EmpowermentCreateElementsEnumUi.EDIT_TEXT_EMPOWERED_UID_NUMBER.title,
                        elementEnum = EmpowermentCreateElementsEnumUi.EDIT_TEXT_EMPOWERED_UID_NUMBER,
                        validators = listOf(
                            NonEmptyEditTextValidator(),
                            PersonalIdentifierValidator(),
                            ExistingValueEditTextValidator(
                                existingValuesProvider = { listOf(from.user?.citizenIdentifier) },
                                errorMessage = StringSource(R.string.error_empowerment_self_as_representative)
                            ),
                            ExistingValueEditTextValidator(
                                existingValuesProvider = { allEmpoweredPeopleUids ?: emptyList() },
                                errorMessage = StringSource(R.string.error_personal_identifier_already_used)
                            )
                        )
                    )
                )
                add(
                    CommonEditTextUi(
                        elementId = index + 1,
                        required = true,
                        question = false,
                        selectedValue = value.name,
                        title = EmpowermentCreateElementsEnumUi.EDIT_TEXT_EMPOWERED_NAMES.title,
                        elementEnum = EmpowermentCreateElementsEnumUi.EDIT_TEXT_EMPOWERED_NAMES,
                        minSymbols = 3,
                        maxSymbols = 200,
                        type = CommonEditTextUiType.TEXT_INPUT_CAP,
                        validators = listOf(
                            NonEmptyEditTextValidator(),
                            MinLengthEditTextValidator(minLength = NAMES_MIN_LENGTH),
                            FirstUpperCasedValidator(),
                        )
                    )
                )
            }
        }
        add(
            CommonButtonTransparentUi(
                title = EmpowermentCreateElementsEnumUi.BUTTON_ADD_EMPOWERED.title,
                elementEnum = EmpowermentCreateElementsEnumUi.BUTTON_ADD_EMPOWERED,
            )
        )
        add(
            CommonTitleSmallUi(
                title = StringSource(R.string.empowerment_create_empowerment_title)
            )
        )
        add(
            CommonTextFieldUi(
                required = true,
                question = false,
                title = EmpowermentCreateElementsEnumUi.DIALOG_SUPPLIER_NAME.title,
                elementEnum = EmpowermentCreateElementsEnumUi.DIALOG_SUPPLIER_NAME,
                text = StringSource(R.string.please_select),
            )
        )
        add(
            CommonTextFieldUi(
                required = true,
                question = false,
                title = EmpowermentCreateElementsEnumUi.DIALOG_SERVICE_NAME.title,
                elementEnum = EmpowermentCreateElementsEnumUi.DIALOG_SERVICE_NAME,
                text = StringSource(R.string.please_select),
            )
        )
        add(
            CommonTextFieldUi(
                required = true,
                question = false,
                title = EmpowermentCreateElementsEnumUi.DIALOG_VOLUME_OF_REPRESENTATION.title,
                elementEnum = EmpowermentCreateElementsEnumUi.DIALOG_VOLUME_OF_REPRESENTATION,
                text = StringSource(R.string.please_select),
            )
        )
        add(
            CommonDatePickerUi(
                question = false,
                required = true,
                elementEnum = EmpowermentCreateElementsEnumUi.DATE_PICKER_START_DATE,
                selectedValue = getCalendar(),
                title = EmpowermentCreateElementsEnumUi.DATE_PICKER_START_DATE.title,
                minDate = getCalendar(),
                maxDate = getCalendar(plusYears = 100),
                dateFormat = UiDateFormats.WITHOUT_TIME,
                validators = listOf(
                    NonEmptyDatePickerValidator()
                )
            )
        )
        add(
            CommonDatePickerUi(
                question = false,
                required = false,
                selectedValue = null,
                title = EmpowermentCreateElementsEnumUi.DATE_PICKER_END_DATE.title,
                elementEnum = EmpowermentCreateElementsEnumUi.DATE_PICKER_END_DATE,
                minDate = getCalendar(),
                maxDate = getCalendar(plusYears = 100),
                dateFormat = UiDateFormats.WITHOUT_TIME,
            )
        )
        add(
            CommonButtonUi(
                title = EmpowermentCreateElementsEnumUi.BUTTON_PREVIEW.title,
                elementEnum = EmpowermentCreateElementsEnumUi.BUTTON_PREVIEW,
                buttonColor = ButtonColorUi.BLUE,
            )
        )
        add(
            CommonButtonUi(
                title = EmpowermentCreateElementsEnumUi.BUTTON_CANCEL.title,
                elementEnum = EmpowermentCreateElementsEnumUi.BUTTON_CANCEL,
                buttonColor = ButtonColorUi.RED,
            )
        )
    }
}