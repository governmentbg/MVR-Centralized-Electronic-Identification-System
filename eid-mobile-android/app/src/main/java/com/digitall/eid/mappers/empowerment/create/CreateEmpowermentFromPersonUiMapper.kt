package com.digitall.eid.mappers.empowerment.create

import com.digitall.eid.R
import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.domain.NAMES_MIN_LENGTH
import com.digitall.eid.domain.UiDateFormats
import com.digitall.eid.domain.extensions.getCalendar
import com.digitall.eid.domain.extensions.getEnumValue
import com.digitall.eid.models.common.ButtonColorUi
import com.digitall.eid.models.common.StringSource
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

class CreateEmpowermentFromPersonUiMapper: BaseMapper<EmpowermentCreateUiModel, List<EmpowermentCreateAdapterMarker>>() {

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
                    text = EmpowermentCreateFromNameOfEnumUi.PERSON.title,
                    elementEnum = EmpowermentCreateFromNameOfEnumUi.PERSON,
                    serverValue = EmpowermentCreateFromNameOfEnumUi.PERSON.type,
                ),
                list = EmpowermentCreateFromNameOfEnumUi.entries.map {
                    CommonSpinnerMenuItemUi(
                        isSelected = false,
                        text = it.title,
                        elementEnum = it,
                        serverValue = it.type,
                    )
                }
            )
        )
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
                elementId = 1,
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
                elementId = 1,
                required = true,
                question = false,
                serverValue = from.user?.citizenIdentifier,
                title = EmpowermentCreateElementsEnumUi.TEXT_VIEW_LEGAL_REPRESANTATIVE_UID_NUMBER.title,
                elementEnum = EmpowermentCreateElementsEnumUi.TEXT_VIEW_LEGAL_REPRESANTATIVE_UID_NUMBER,
                text = StringSource(from.user?.citizenIdentifier),
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
                                errorMessage = StringSource(R.string.error_empowerment_self)
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