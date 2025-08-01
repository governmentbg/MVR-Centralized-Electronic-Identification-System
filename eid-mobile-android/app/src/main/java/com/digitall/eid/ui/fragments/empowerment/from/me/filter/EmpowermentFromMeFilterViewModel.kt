/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.empowerment.from.me.filter

import com.digitall.eid.R
import com.digitall.eid.domain.UiDateFormats
import com.digitall.eid.domain.extensions.fromServerDate
import com.digitall.eid.domain.extensions.getCalendar
import com.digitall.eid.domain.models.empowerment.common.filter.EmpowermentFilterModel
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.common.validator.PersonalIdentifierValidator
import com.digitall.eid.models.empowerment.common.filter.EmpowermentFilterAdapterMarker
import com.digitall.eid.models.empowerment.common.filter.EmpowermentFilterElementsEnumUi
import com.digitall.eid.models.empowerment.common.filter.EmpowermentFilterOnBehalfOfEnumUi
import com.digitall.eid.models.empowerment.common.filter.EmpowermentFilterIdTypeEnumUi
import com.digitall.eid.models.empowerment.common.filter.EmpowermentFilterStatusEnumUi
import com.digitall.eid.models.list.CommonButtonTransparentUi
import com.digitall.eid.models.list.CommonCheckBoxUi
import com.digitall.eid.models.list.CommonDatePickerUi
import com.digitall.eid.models.list.CommonEditTextUi
import com.digitall.eid.models.list.CommonEditTextUiType
import com.digitall.eid.models.list.CommonSpinnerMenuItemUi
import com.digitall.eid.models.list.CommonSpinnerUi
import com.digitall.eid.models.list.CommonTextFieldUi
import com.digitall.eid.models.list.CommonTitleSmallUi
import com.digitall.eid.ui.fragments.empowerment.base.filter.BaseEmpowermentFilterViewModel

class EmpowermentFromMeFilterViewModel : BaseEmpowermentFilterViewModel() {

    companion object {
        private const val TAG = "EmpowermentFromMeFilterViewModelTag"
    }

    override fun getStartScreenElements(model: EmpowermentFilterModel): List<EmpowermentFilterAdapterMarker> {
        logDebug("setScreenElements", TAG)
        return buildList {
            add(
                CommonTitleSmallUi(
                    title = StringSource(R.string.empowerments_entity_filter_empowerment_number_section_title),
                )
            )
            add(
                CommonEditTextUi(
                    required = false,
                    question = false,
                    title = EmpowermentFilterElementsEnumUi.EDIT_TEXT_NUMBER_EMPOWERMENT.title,
                    elementEnum = EmpowermentFilterElementsEnumUi.EDIT_TEXT_NUMBER_EMPOWERMENT,
                    selectedValue = model.empowermentNumber,
                    maxSymbols = 24,
                    type = CommonEditTextUiType.TEXT_INPUT
                ),
            )
            add(
                CommonTitleSmallUi(
                    title = StringSource(R.string.empowerments_entity_filter_status_section_title),
                )
            )
            add(
                CommonSpinnerUi(
                    required = false,
                    question = false,
                    title = EmpowermentFilterElementsEnumUi.SPINNER_STATUS.title,
                    elementEnum = EmpowermentFilterElementsEnumUi.SPINNER_STATUS,
                    list = statusList,
                    selectedValue = statusList.firstOrNull { it.serverValue == model.status }
                        ?: statusList.first(),
                ),
            )
            add(
                CommonTitleSmallUi(
                    title = StringSource(R.string.empowerments_entity_filter_authorizer_section_title)
                )
            )
            val onBehalfOfType = onBehalfOfList.firstOrNull {
                (it.elementEnum as? EmpowermentFilterOnBehalfOfEnumUi)?.type == model.onBehalfOf
            }
            add(
                CommonSpinnerUi(
                    required = false,
                    question = false,
                    title = EmpowermentFilterElementsEnumUi.SPINNER_ON_BEHALF_OF.title,
                    elementEnum = EmpowermentFilterElementsEnumUi.SPINNER_ON_BEHALF_OF,
                    selectedValue = onBehalfOfType ?: onBehalfOfList.first(),
                    list = onBehalfOfList
                ),
            )
            onBehalfOfType?.let { type ->
                when (type.elementEnum) {
                    EmpowermentFilterOnBehalfOfEnumUi.LEGAL_ENTITY -> {
                        add(
                            CommonEditTextUi(
                                required = false,
                                question = false,
                                selectedValue = model.authorizer,
                                type = CommonEditTextUiType.TEXT_INPUT,
                                title = EmpowermentFilterElementsEnumUi.EDIT_TEXT_LEGAL_ENTITY_NAME.title,
                                elementEnum = EmpowermentFilterElementsEnumUi.EDIT_TEXT_LEGAL_ENTITY_NAME,
                                isEnabled = true
                            )
                        )
                        add(
                            CommonEditTextUi(
                                required = false,
                                question = false,
                                selectedValue = model.eik,
                                type = CommonEditTextUiType.NUMBERS,
                                title = EmpowermentFilterElementsEnumUi.EDIT_TEXT_EIK_BULSTAT.title,
                                elementEnum = EmpowermentFilterElementsEnumUi.EDIT_TEXT_EIK_BULSTAT,
                            )
                        )
                    }

                    EmpowermentFilterOnBehalfOfEnumUi.INDIVIDUAL, EmpowermentFilterOnBehalfOfEnumUi.ALL ->
                        add(
                            CommonEditTextUi(
                                required = false,
                                question = false,
                                selectedValue = model.authorizer,
                                type = CommonEditTextUiType.TEXT_INPUT,
                                title = EmpowermentFilterElementsEnumUi.EDIT_TEXT_EMPOWERER.title,
                                elementEnum = EmpowermentFilterElementsEnumUi.EDIT_TEXT_EMPOWERER,
                            )
                        )
                }
            } ?: add(
                CommonEditTextUi(
                    required = false,
                    question = false,
                    selectedValue = model.authorizer,
                    type = CommonEditTextUiType.TEXT_INPUT,
                    title = EmpowermentFilterElementsEnumUi.EDIT_TEXT_EMPOWERER.title,
                    elementEnum = EmpowermentFilterElementsEnumUi.EDIT_TEXT_EMPOWERER,
                )
            )
            add(
                CommonTitleSmallUi(
                    title = StringSource(R.string.empowerments_entity_filter_service_section_title)
                )
            )
            add(
                CommonEditTextUi(
                    required = false,
                    question = false,
                    selectedValue = model.providerName,
                    title = EmpowermentFilterElementsEnumUi.EDIT_TEXT_SUPPLIER.title,
                    type = CommonEditTextUiType.TEXT_INPUT,
                    elementEnum = EmpowermentFilterElementsEnumUi.EDIT_TEXT_SUPPLIER,
                )
            )
            if (model.providerName.isNullOrEmpty()) {
                add(
                    CommonTextFieldUi(
                        required = false,
                        question = false,
                        elementEnum = EmpowermentFilterElementsEnumUi.EDIT_TEXT_SERVICE,
                        title = EmpowermentFilterElementsEnumUi.EDIT_TEXT_SERVICE.title,
                        text = StringSource(R.string.empowerments_entity_filter_empty_supplier_title),
                    )
                )
            } else {
                add(
                    CommonEditTextUi(
                        required = false,
                        question = false,
                        selectedValue = model.serviceName,
                        title = EmpowermentFilterElementsEnumUi.EDIT_TEXT_SERVICE.title,
                        type = CommonEditTextUiType.TEXT_INPUT,
                        elementEnum = EmpowermentFilterElementsEnumUi.EDIT_TEXT_SERVICE,
                    )
                )
            }
            add(
                CommonTitleSmallUi(
                    title = StringSource(R.string.empowerments_entity_filter_empowered_people_section_title),
                )
            )
            if (model.empoweredIDs.isNullOrEmpty()) {
                add(
                    CommonSpinnerUi(
                        elementId = 1,
                        required = false,
                        question = false,
                        title = EmpowermentFilterElementsEnumUi.SPINNER_ID_TYPE.title,
                        elementEnum = EmpowermentFilterElementsEnumUi.SPINNER_ID_TYPE,
                        selectedValue = CommonSpinnerMenuItemUi(
                            isSelected = true,
                            text = EmpowermentFilterIdTypeEnumUi.SPINNER_ID_TYPE_EGN.title,
                            elementEnum = EmpowermentFilterIdTypeEnumUi.SPINNER_ID_TYPE_EGN,
                        ),
                        list = listOf(
                            CommonSpinnerMenuItemUi(
                                isSelected = false,
                                text = EmpowermentFilterIdTypeEnumUi.SPINNER_ID_TYPE_EGN.title,
                                elementEnum = EmpowermentFilterIdTypeEnumUi.SPINNER_ID_TYPE_EGN,
                            ),
                            CommonSpinnerMenuItemUi(
                                isSelected = false,
                                text = EmpowermentFilterIdTypeEnumUi.SPINNER_ID_TYPE_LNCH.title,
                                elementEnum = EmpowermentFilterIdTypeEnumUi.SPINNER_ID_TYPE_LNCH,
                            ),
                        ),
                    )
                )
                add(
                    CommonEditTextUi(
                        elementId = 1,
                        required = false,
                        question = false,
                        elementEnum = EmpowermentFilterElementsEnumUi.EDIT_TEXT_ID_NUMBER,
                        title = EmpowermentFilterElementsEnumUi.EDIT_TEXT_ID_NUMBER.title,
                        selectedValue = null,
                        maxSymbols = 10,
                        type = CommonEditTextUiType.NUMBERS,
                        validators = listOf(
                            PersonalIdentifierValidator()
                        )
                    )
                )
            } else {
                model.empoweredIDs?.filter {
                    !it.uid.isNullOrEmpty() && !it.uidType.isNullOrEmpty()
                }?.forEachIndexed { index, value ->
                    add(
                        CommonSpinnerUi(
                            elementId = index,
                            required = false,
                            question = false,
                            title = EmpowermentFilterElementsEnumUi.SPINNER_ID_TYPE.title,
                            elementEnum = EmpowermentFilterElementsEnumUi.SPINNER_ID_TYPE,
                            selectedValue = when (value.uidType) {
                                EmpowermentFilterIdTypeEnumUi.SPINNER_ID_TYPE_EGN.type -> {
                                    CommonSpinnerMenuItemUi(
                                        isSelected = true,
                                        text = EmpowermentFilterIdTypeEnumUi.SPINNER_ID_TYPE_EGN.title,
                                        elementEnum = EmpowermentFilterIdTypeEnumUi.SPINNER_ID_TYPE_EGN,
                                    )
                                }

                                EmpowermentFilterIdTypeEnumUi.SPINNER_ID_TYPE_LNCH.type -> {
                                    CommonSpinnerMenuItemUi(
                                        isSelected = true,
                                        text = EmpowermentFilterIdTypeEnumUi.SPINNER_ID_TYPE_LNCH.title,
                                        elementEnum = EmpowermentFilterIdTypeEnumUi.SPINNER_ID_TYPE_LNCH,
                                    )
                                }

                                else -> null
                            },
                            list = listOf(
                                CommonSpinnerMenuItemUi(
                                    isSelected = false,
                                    text = EmpowermentFilterIdTypeEnumUi.SPINNER_ID_TYPE_EGN.title,
                                    elementEnum = EmpowermentFilterIdTypeEnumUi.SPINNER_ID_TYPE_EGN,
                                ),
                                CommonSpinnerMenuItemUi(
                                    isSelected = false,
                                    text = EmpowermentFilterIdTypeEnumUi.SPINNER_ID_TYPE_LNCH.title,
                                    elementEnum = EmpowermentFilterIdTypeEnumUi.SPINNER_ID_TYPE_LNCH,
                                ),
                            ),
                        )
                    )
                    add(
                        CommonEditTextUi(
                            elementId = index,
                            required = false,
                            question = false,
                            selectedValue = value.uid!!,
                            maxSymbols = 10,
                            type = CommonEditTextUiType.NUMBERS,
                            title = EmpowermentFilterElementsEnumUi.EDIT_TEXT_ID_NUMBER.title,
                            elementEnum = EmpowermentFilterElementsEnumUi.EDIT_TEXT_ID_NUMBER,
                            validators = listOf(
                                PersonalIdentifierValidator()
                            )
                        )
                    )
                }
            }
            add(
                CommonButtonTransparentUi(
                    title = EmpowermentFilterElementsEnumUi.BUTTON_ADD_EMPOWERED_PERSON.title,
                    elementEnum = EmpowermentFilterElementsEnumUi.BUTTON_ADD_EMPOWERED_PERSON,
                )
            )
            add(
                CommonTitleSmallUi(
                    title = StringSource(R.string.empowerments_entity_filter_period_section_title)
                )
            )
            if (model.showOnlyNoExpiryDate.not()) {
                val haveValidDate = !model.validToDate.isNullOrEmpty()
                add(
                    CommonDatePickerUi(
                        required = false,
                        question = false,
                        title = EmpowermentFilterElementsEnumUi.DATE_PICKER_DATE.title,
                        elementEnum = EmpowermentFilterElementsEnumUi.DATE_PICKER_DATE,
                        selectedValue = if (haveValidDate) model.validToDate?.fromServerDate()
                        else null,
                        minDate = getCalendar(minusYears = 100),
                        maxDate = getCalendar(plusYears = 100),
                        dateFormat = UiDateFormats.WITHOUT_TIME,
                    ),
                )
            }
            add(
                CommonCheckBoxUi(
                    title = EmpowermentFilterElementsEnumUi.CHECK_BOX_UNLIMITED_DATE.title,
                    isChecked = model.showOnlyNoExpiryDate,
                    elementEnum = EmpowermentFilterElementsEnumUi.CHECK_BOX_UNLIMITED_DATE,
                )
            )
        }
    }

    override val statusList = listOf(
        CommonSpinnerMenuItemUi(
            isSelected = false,
            elementEnum = EmpowermentFilterStatusEnumUi.ALL,
            text = EmpowermentFilterStatusEnumUi.ALL.title,
        ),
        CommonSpinnerMenuItemUi(
            serverValue = EmpowermentFilterStatusEnumUi.ACTIVE.type,
            isSelected = false,
            elementEnum = EmpowermentFilterStatusEnumUi.ACTIVE,
            text = EmpowermentFilterStatusEnumUi.ACTIVE.title,
        ),
        CommonSpinnerMenuItemUi(
            serverValue = EmpowermentFilterStatusEnumUi.EXPIRED.type,
            isSelected = false,
            elementEnum = EmpowermentFilterStatusEnumUi.EXPIRED,
            text = EmpowermentFilterStatusEnumUi.EXPIRED.title,
        ),
        CommonSpinnerMenuItemUi(
            serverValue = EmpowermentFilterStatusEnumUi.DISAGREEMENT_DECLARED.type,
            isSelected = false,
            elementEnum = EmpowermentFilterStatusEnumUi.DISAGREEMENT_DECLARED,
            text = EmpowermentFilterStatusEnumUi.DISAGREEMENT_DECLARED.title,
        ),
        CommonSpinnerMenuItemUi(
            serverValue = EmpowermentFilterStatusEnumUi.WITHDRAWN.type,
            isSelected = false,
            elementEnum = EmpowermentFilterStatusEnumUi.WITHDRAWN,
            text = EmpowermentFilterStatusEnumUi.WITHDRAWN.title,
        ),
        CommonSpinnerMenuItemUi(
            serverValue = EmpowermentFilterStatusEnumUi.COLLECTING_AUTHORIZER_SIGNATURES.type,
            isSelected = false,
            elementEnum = EmpowermentFilterStatusEnumUi.COLLECTING_AUTHORIZER_SIGNATURES,
            text = EmpowermentFilterStatusEnumUi.COLLECTING_AUTHORIZER_SIGNATURES.title,
        ),
        CommonSpinnerMenuItemUi(
            serverValue = EmpowermentFilterStatusEnumUi.CREATED.type,
            isSelected = false,
            elementEnum = EmpowermentFilterStatusEnumUi.CREATED,
            text = EmpowermentFilterStatusEnumUi.CREATED.title,
        ),
        CommonSpinnerMenuItemUi(
            serverValue = EmpowermentFilterStatusEnumUi.DENIED.type,
            isSelected = false,
            elementEnum = EmpowermentFilterStatusEnumUi.DENIED,
            text = EmpowermentFilterStatusEnumUi.DENIED.title,
        ),
        CommonSpinnerMenuItemUi(
            serverValue = EmpowermentFilterStatusEnumUi.UNCONFIRMED.type,
            isSelected = false,
            elementEnum = EmpowermentFilterStatusEnumUi.UNCONFIRMED,
            text = EmpowermentFilterStatusEnumUi.UNCONFIRMED.title,
        ),
    )

    private val onBehalfOfList = EmpowermentFilterOnBehalfOfEnumUi.entries.map { element ->
        CommonSpinnerMenuItemUi(
            isSelected = false,
            text = element.title,
            elementEnum = element,
        )
    }

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        popBackStackToFragment(R.id.empowermentFromMeFragment)
    }

}