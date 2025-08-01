/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.empowerment.base.filter

import androidx.lifecycle.MutableLiveData
import com.digitall.eid.R
import com.digitall.eid.domain.ToServerDateFormats
import com.digitall.eid.domain.extensions.toServerDate
import com.digitall.eid.domain.models.base.TypeEnum
import com.digitall.eid.domain.models.empowerment.common.EmpowermentUidModel
import com.digitall.eid.domain.models.empowerment.common.filter.EmpowermentFilterModel
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.extensions.readOnly
import com.digitall.eid.extensions.setValueOnMainThread
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.common.validator.PersonalIdentifierValidator
import com.digitall.eid.models.empowerment.common.filter.EmpowermentFilterAdapterMarker
import com.digitall.eid.models.empowerment.common.filter.EmpowermentFilterElementsEnumUi
import com.digitall.eid.models.empowerment.common.filter.EmpowermentFilterOnBehalfOfEnumUi
import com.digitall.eid.models.empowerment.common.filter.EmpowermentFilterIdTypeEnumUi
import com.digitall.eid.models.list.CommonButtonTransparentUi
import com.digitall.eid.models.list.CommonCheckBoxUi
import com.digitall.eid.models.list.CommonDatePickerUi
import com.digitall.eid.models.list.CommonDialogWithSearchUi
import com.digitall.eid.models.list.CommonEditTextUi
import com.digitall.eid.models.list.CommonEditTextUiType
import com.digitall.eid.models.list.CommonSpinnerMenuItemUi
import com.digitall.eid.models.list.CommonSpinnerUi
import com.digitall.eid.models.list.CommonTextFieldUi
import com.digitall.eid.models.list.CommonValidationFieldUi
import com.digitall.eid.ui.BaseViewModel
import com.digitall.eid.utils.SingleLiveEvent

abstract class BaseEmpowermentFilterViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "BaseEmpowermentFilterViewModelTag"
    }

    abstract val statusList: List<CommonSpinnerMenuItemUi>

    private val _adapterListLiveData =
        MutableLiveData<List<EmpowermentFilterAdapterMarker>>(emptyList())
    val adapterListLiveData = _adapterListLiveData.readOnly()

    private val _applyFilterDataLiveData = SingleLiveEvent<EmpowermentFilterModel>()
    val applyFilterDataLiveData = _applyFilterDataLiveData.readOnly()

    private val _scrollToErrorPositionLiveData = SingleLiveEvent<Int>()
    val scrollToErrorPositionLiveData = _scrollToErrorPositionLiveData.readOnly()

    private var errorPosition: Int? = null
        set(value) {
            field = value
            value?.let { position ->
                _scrollToErrorPositionLiveData.setValueOnMainThread(position)
            }
        }

    private lateinit var filteringModel: EmpowermentFilterModel

    abstract fun getStartScreenElements(model: EmpowermentFilterModel): List<EmpowermentFilterAdapterMarker>

    fun setFilteringModel(filterModel: EmpowermentFilterModel) {
        filteringModel = filterModel
        _adapterListLiveData.value = getStartScreenElements(filteringModel)
    }

    final override fun onFirstAttach() {
        logDebug("onFirstAttach", TAG)
    }

    fun onEraseClicked(model: EmpowermentFilterAdapterMarker) {
        logDebug("onEraseClicked", TAG)
        val currentItems = _adapterListLiveData.value?.toMutableList() ?: mutableListOf()

        currentItems.removeAll { element ->
            element.elementId == model.elementId && (
                    element.elementEnum == EmpowermentFilterElementsEnumUi.SPINNER_ID_TYPE ||
                            element.elementEnum == EmpowermentFilterElementsEnumUi.EDIT_TEXT_ID_NUMBER)
        }

        _adapterListLiveData.value = currentItems
    }

    final override fun onSpinnerSelected(model: CommonSpinnerUi) {
        logDebug("onSpinnerSelected", TAG)
        changeValidationField(model)

        val currentItems = _adapterListLiveData.value?.toMutableList() ?: mutableListOf()
        currentItems.firstOrNull { element -> element.elementEnum == model.elementEnum && element.elementId == model.elementId }
            ?.let { foundElement ->
                val index = currentItems.indexOf(foundElement)
                when (model.elementEnum) {
                    EmpowermentFilterElementsEnumUi.SPINNER_ON_BEHALF_OF -> {
                        logDebug("onSpinnerSelected SPINNER_ON_BEHALF_OF", TAG)
                        currentItems.removeAll { element ->
                            element.elementEnum == EmpowermentFilterElementsEnumUi.EDIT_TEXT_LEGAL_ENTITY_NAME
                                    || element.elementEnum == EmpowermentFilterElementsEnumUi.EDIT_TEXT_EMPOWERER
                                    || element.elementEnum == EmpowermentFilterElementsEnumUi.EDIT_TEXT_EIK_BULSTAT
                        }
                        when (model.selectedValue?.elementEnum) {
                            EmpowermentFilterOnBehalfOfEnumUi.LEGAL_ENTITY -> {
                                logDebug("onFromNameOfChanged SPINNER_FROM_COMPANY", TAG)
                                val legalEntityNameField = CommonEditTextUi(
                                    required = false,
                                    question = false,
                                    selectedValue = null,
                                    type = CommonEditTextUiType.TEXT_INPUT,
                                    title = EmpowermentFilterElementsEnumUi.EDIT_TEXT_LEGAL_ENTITY_NAME.title,
                                    elementEnum = EmpowermentFilterElementsEnumUi.EDIT_TEXT_LEGAL_ENTITY_NAME,
                                )
                                val eikField = CommonEditTextUi(
                                    required = false,
                                    question = false,
                                    selectedValue = null,
                                    type = CommonEditTextUiType.NUMBERS,
                                    title = EmpowermentFilterElementsEnumUi.EDIT_TEXT_EIK_BULSTAT.title,
                                    elementEnum = EmpowermentFilterElementsEnumUi.EDIT_TEXT_EIK_BULSTAT,
                                )

                                currentItems.addAll(
                                    index + 1,
                                    listOf(legalEntityNameField, eikField)
                                )
                            }

                            EmpowermentFilterOnBehalfOfEnumUi.INDIVIDUAL, EmpowermentFilterOnBehalfOfEnumUi.ALL -> {
                                logDebug("onFromNameOfChanged SPINNER_FROM_PERSON", TAG)
                                val individualNameField = CommonEditTextUi(
                                    required = false,
                                    question = false,
                                    selectedValue = null,
                                    type = CommonEditTextUiType.TEXT_INPUT,
                                    title = EmpowermentFilterElementsEnumUi.EDIT_TEXT_EMPOWERER.title,
                                    elementEnum = EmpowermentFilterElementsEnumUi.EDIT_TEXT_EMPOWERER,
                                    isEnabled = true
                                )
                                currentItems.add(
                                    index + 1,
                                    individualNameField
                                )
                            }
                        }
                    }

                    EmpowermentFilterElementsEnumUi.SPINNER_ID_TYPE -> {
                        logDebug("onSpinnerSelected SPINNER_ID_TYPE", TAG)
                        currentItems[index + 1] = CommonEditTextUi(
                            elementId = model.elementId,
                            maxSymbols = 10,
                            required = false,
                            question = false,
                            selectedValue = null,
                            type = CommonEditTextUiType.NUMBERS,
                            title = EmpowermentFilterElementsEnumUi.EDIT_TEXT_ID_NUMBER.title,
                            elementEnum = EmpowermentFilterElementsEnumUi.EDIT_TEXT_ID_NUMBER,
                            validators = listOf(
                                PersonalIdentifierValidator()
                            )
                        )
                    }
                }
                _adapterListLiveData.value = currentItems
            }
    }

    final override fun onDatePickerChanged(model: CommonDatePickerUi) {
        logDebug("onDatePickerChanged", TAG)
        changeValidationField(model)

        when (model.elementEnum) {
            EmpowermentFilterElementsEnumUi.DATE_PICKER_DATE -> {
                val currentItems = _adapterListLiveData.value?.toMutableList() ?: mutableListOf()
                currentItems.findLast { element -> element.elementEnum == EmpowermentFilterElementsEnumUi.CHECK_BOX_UNLIMITED_DATE }
                    ?.let { foundElement ->
                        when (foundElement) {
                            is CommonCheckBoxUi -> {
                                val index = currentItems.indexOf(foundElement)
                                currentItems[index] = foundElement.copy(isChecked = false)

                                _adapterListLiveData.value = currentItems
                            }

                            else -> {}
                        }
                    }
            }

            else -> {}
        }
    }

    fun onCheckBoxChangeState(model: CommonCheckBoxUi) {
        logDebug("onCheckBoxChangeState", TAG)
        changeCheckbox(model)

        if (model.isChecked) {
            when (model.elementEnum) {
                EmpowermentFilterElementsEnumUi.CHECK_BOX_UNLIMITED_DATE -> {
                    val currentItems =
                        _adapterListLiveData.value?.toMutableList() ?: mutableListOf()
                    currentItems.findLast { element -> element.elementEnum == EmpowermentFilterElementsEnumUi.DATE_PICKER_DATE }
                        ?.let { foundElement ->
                            when (foundElement) {
                                is CommonDatePickerUi -> currentItems[currentItems.indexOf(
                                    foundElement
                                )] =
                                    foundElement.copy(selectedValue = null)
                            }
                            _adapterListLiveData.value = currentItems
                        }
                }
            }
        }
    }

    final override fun onDialogElementSelected(model: CommonDialogWithSearchUi) {
        logDebug("onDialogElementSelected", TAG)
        changeValidationField(model)
    }

    fun tryApplyFilterData() {
        tryToApplyFilterData()
        _applyFilterDataLiveData.setValueOnMainThread(filteringModel)
        onBackPressed()
    }

    fun clearFilterData() {
        clearFilter()
        _applyFilterDataLiveData.setValueOnMainThread(filteringModel)
        onBackPressed()
    }

    fun onButtonTransparentClicked(model: CommonButtonTransparentUi) {
        logDebug("onCenterButtonClicked", TAG)
        when (model.elementEnum) {
            EmpowermentFilterElementsEnumUi.BUTTON_ADD_EMPOWERED_PERSON -> addEmpoweredPerson()
        }
    }

    private fun clearFilter() {
        logDebug("clearFilter", TAG)
        filteringModel = filteringModel.copy(
            status = null,
            authorizer = null,
            onBehalfOf = null,
            fromNameOf = null,
            eik = null,
            serviceName = null,
            validToDate = null,
            providerName = null,
            empoweredIDs = null,
            showOnlyNoExpiryDate = false,
            authorizerList = emptyList(),
            onBehalfOfList = emptyList(),
            serviceNameList = emptyList(),
            providerNameList = emptyList(),
            empowermentNumber = null,
        )
    }


    private fun addEmpoweredPerson() {
        logDebug("addEmpoweredPerson", TAG)
        val currentItems = _adapterListLiveData.value?.toMutableList() ?: mutableListOf()
        _adapterListLiveData.value?.findLast {
            it.elementEnum == EmpowermentFilterElementsEnumUi.SPINNER_ID_TYPE
        }?.let { foundElement ->
            val index = currentItems.indexOf(foundElement)
            val nextId = (foundElement.elementId?.plus(1))
            currentItems.add(
                index + 2,
                CommonSpinnerUi(
                    elementId = nextId,
                    required = false,
                    question = false,
                    title = EmpowermentFilterElementsEnumUi.SPINNER_ID_TYPE.title,
                    elementEnum = EmpowermentFilterElementsEnumUi.SPINNER_ID_TYPE,
                    selectedValue = null,
                    hasEraseButton = true,
                    list = listOf(
                        CommonSpinnerMenuItemUi(
                            isSelected = false,
                            elementEnum = EmpowermentFilterIdTypeEnumUi.SPINNER_ID_TYPE_EGN,
                            text = EmpowermentFilterIdTypeEnumUi.SPINNER_ID_TYPE_EGN.title,
                        ),
                        CommonSpinnerMenuItemUi(
                            isSelected = false,
                            elementEnum = EmpowermentFilterIdTypeEnumUi.SPINNER_ID_TYPE_LNCH,
                            text = EmpowermentFilterIdTypeEnumUi.SPINNER_ID_TYPE_LNCH.title,
                        ),
                    )
                ),
            )
            currentItems.add(
                index + 3,
                CommonTextFieldUi(
                    elementId = nextId,
                    required = false,
                    question = false,
                    elementEnum = EmpowermentFilterElementsEnumUi.EDIT_TEXT_ID_NUMBER,
                    title = EmpowermentFilterElementsEnumUi.EDIT_TEXT_ID_NUMBER.title,
                    text = StringSource(R.string.empowerment_create_choose_identifier_type_hint),
                )
            )
            _adapterListLiveData.value = currentItems
        }
    }

    private fun tryToApplyFilterData() {
        logDebug("tryToApplyFilterData", TAG)
        if (canSubmitForm()) {
            applyFilterData()
        }
    }

    private fun applyFilterData() {
        var status: String? = null
        var authorizer: String? = null
        var onBehalfOf: String? = null
        var serviceName: String? = null
        var validToDate: String? = null
        var providerName: String? = null
        var eik: String? = null
        var empowermentNumber: String? = null
        var showOnlyNoExpiryDate = false
        val empoweredID = mutableMapOf<Int, EmpowermentUidModel>()
        _adapterListLiveData.value?.forEach { element ->
            when (element) {
                is CommonSpinnerUi -> {
                    when (element.elementEnum) {
                        EmpowermentFilterElementsEnumUi.SPINNER_STATUS ->
                            status = element.selectedValue?.serverValue

                        EmpowermentFilterElementsEnumUi.SPINNER_ON_BEHALF_OF ->
                            onBehalfOf = (element.selectedValue?.elementEnum as? TypeEnum)?.type

                        EmpowermentFilterElementsEnumUi.SPINNER_ID_TYPE -> {
                            element.elementId?.let { elementId ->
                                if (!empoweredID.containsKey(elementId)) {
                                    empoweredID[elementId] = EmpowermentUidModel(
                                        uid = null,
                                        uidType = (element.selectedValue?.elementEnum as? EmpowermentFilterIdTypeEnumUi)?.type,
                                        name = null
                                    )
                                } else {
                                    empoweredID[elementId]?.copy(
                                        uidType = (element.selectedValue?.elementEnum as? EmpowermentFilterIdTypeEnumUi)?.type
                                    )?.let { model ->
                                        empoweredID[elementId] = model
                                    }
                                }
                            }
                        }
                    }
                }

                is CommonEditTextUi -> {
                    when (element.elementEnum) {
                        EmpowermentFilterElementsEnumUi.EDIT_TEXT_EMPOWERER ->
                            authorizer = element.selectedValue

                        EmpowermentFilterElementsEnumUi.EDIT_TEXT_NUMBER_EMPOWERMENT ->
                            empowermentNumber = element.selectedValue

                        EmpowermentFilterElementsEnumUi.EDIT_TEXT_SUPPLIER ->
                            providerName = element.selectedValue

                        EmpowermentFilterElementsEnumUi.EDIT_TEXT_SERVICE ->
                            serviceName = element.selectedValue

                        EmpowermentFilterElementsEnumUi.EDIT_TEXT_LEGAL_ENTITY_NAME ->
                            authorizer = element.selectedValue

                        EmpowermentFilterElementsEnumUi.EDIT_TEXT_EIK_BULSTAT ->
                            eik = element.selectedValue

                        EmpowermentFilterElementsEnumUi.EDIT_TEXT_ID_NUMBER -> {
                            element.elementId?.let { elementId ->
                                if (!empoweredID.containsKey(elementId)) {
                                    empoweredID[elementId] = EmpowermentUidModel(
                                        uidType = null,
                                        uid = element.selectedValue,
                                        name = null,
                                    )
                                } else {
                                    empoweredID[elementId]?.copy(
                                        uid = element.selectedValue
                                    )?.let { model ->
                                        empoweredID[elementId] = model
                                    }
                                }
                            }
                        }
                    }
                }

                is CommonCheckBoxUi -> {
                    when (element.elementEnum) {
                        EmpowermentFilterElementsEnumUi.CHECK_BOX_UNLIMITED_DATE -> showOnlyNoExpiryDate =
                            element.isChecked
                    }
                }

                is CommonDatePickerUi -> {
                    when (element.elementEnum) {
                        EmpowermentFilterElementsEnumUi.DATE_PICKER_DATE ->
                            validToDate = element.selectedValue?.toServerDate(
                                dateFormat = ToServerDateFormats.WITH_MILLIS,
                            )

                    }
                }
            }
        }

        val filteredEmpoweredID = empoweredID.values.filter {
            !it.uid.isNullOrEmpty() && !it.uidType.isNullOrEmpty()
        }.toList().takeIf { it.isNotEmpty() }

        filteringModel = filteringModel.copy(
            status = status,
            authorizer = authorizer,
            onBehalfOf = if (onBehalfOf.isNullOrEmpty()) null else onBehalfOf,
            serviceName = serviceName,
            validToDate = validToDate,
            providerName = providerName,
            empoweredIDs = filteredEmpoweredID,
            eik = eik,
            showOnlyNoExpiryDate = showOnlyNoExpiryDate,
            empowermentNumber = empowermentNumber
        )
    }

    fun onEditTextChanged(model: CommonEditTextUi) {
        logDebug("onEditTextChanged text: ${model.selectedValue}", TAG)
        changeValidationField(model)
    }

    fun onEnterTextDone(model: CommonEditTextUi) {
        logDebug("onEnterTextDone text: ${model.selectedValue}", TAG)
        changeValidationField(model)
    }

    fun onFocusChanged(model: CommonEditTextUi) {
        logDebug("onFocusChanged hasFocus: ${model.hasFocus}", TAG)
        if (model.hasFocus.not()) {
            changeValidationField(model)
            val currentItems = _adapterListLiveData.value?.toMutableList() ?: mutableListOf()
            when (model.elementEnum) {
                EmpowermentFilterElementsEnumUi.EDIT_TEXT_SUPPLIER ->
                    currentItems.findLast { element -> element.elementEnum == EmpowermentFilterElementsEnumUi.EDIT_TEXT_SERVICE }
                        ?.let { foundElement ->
                            val index = currentItems.indexOf(foundElement)
                            currentItems[index] = if (model.selectedValue.isNullOrEmpty()) {
                                CommonTextFieldUi(
                                    required = false,
                                    question = false,
                                    elementEnum = EmpowermentFilterElementsEnumUi.EDIT_TEXT_SERVICE,
                                    title = EmpowermentFilterElementsEnumUi.EDIT_TEXT_SERVICE.title,
                                    text = StringSource(R.string.empowerments_entity_filter_empty_supplier_title),
                                )
                            } else {
                                CommonEditTextUi(
                                    required = false,
                                    question = false,
                                    selectedValue = null,
                                    title = EmpowermentFilterElementsEnumUi.EDIT_TEXT_SERVICE.title,
                                    type = CommonEditTextUiType.TEXT_INPUT,
                                    elementEnum = EmpowermentFilterElementsEnumUi.EDIT_TEXT_SERVICE,
                                )
                            }
                            _adapterListLiveData.value = currentItems
                        }
            }
        }
    }

    private fun changeValidationField(model: CommonValidationFieldUi<*>) {
        _adapterListLiveData.value = _adapterListLiveData.value?.map { item ->
            if (item.elementEnum == model.elementEnum && item.elementId == model.elementId) {
                when (model) {
                    is CommonEditTextUi -> model.copy(validationError = null).apply {
                        triggerValidation()
                    }

                    is CommonDatePickerUi -> model.copy(validationError = null).apply {
                        triggerValidation()
                    }

                    is CommonDialogWithSearchUi -> model.copy(validationError = null).apply {
                        triggerValidation()
                    }

                    is CommonSpinnerUi -> model.copy(validationError = null).apply {
                        triggerValidation()
                    }

                    else -> item
                }
            } else {
                item
            }
        }
    }

    private fun changeCheckbox(model: CommonCheckBoxUi) {
        _adapterListLiveData.value = _adapterListLiveData.value?.map { item ->
            if (item.elementEnum == model.elementEnum && item.elementId == model.elementId) {
                model
            } else {
                item
            }
        }
    }

    private fun canSubmitForm(): Boolean {
        var allFieldsValid = true
        errorPosition = null
        _adapterListLiveData.value = _adapterListLiveData.value?.mapIndexed { index, item ->
            when (item) {
                is CommonEditTextUi -> {
                    item.copy(validationError = null).apply {
                        val itemIsValid = triggerValidation()
                        if (itemIsValid.not()) {
                            allFieldsValid = false
                            if (errorPosition == null) {
                                errorPosition = index
                            }
                        }
                    }
                }

                else -> item
            }
        }

        return allFieldsValid
    }
}