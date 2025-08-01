package com.digitall.eid.ui.fragments.empowerment.legal.search

import androidx.lifecycle.viewModelScope
import com.digitall.eid.R
import com.digitall.eid.domain.extensions.readOnly
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.extensions.launchWithDispatcher
import com.digitall.eid.models.common.ButtonColorUi
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.empowerment.legal.search.EmpowermentLegalSearchAdapterMarker
import com.digitall.eid.models.empowerment.legal.search.EmpowermentLegalSearchElementsEnumUi
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonEditTextUi
import com.digitall.eid.models.list.CommonEditTextUiType
import com.digitall.eid.models.main.MainTabsEnum
import com.digitall.eid.ui.BaseViewModel
import kotlinx.coroutines.Job
import kotlinx.coroutines.flow.MutableStateFlow

class EmpowermentLegalSearchViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "EmpowermentLegalSearchViewModelTag"
    }

    private val currentList = mutableListOf<EmpowermentLegalSearchAdapterMarker>()

    private val _adapterListLiveData =
        MutableStateFlow<List<EmpowermentLegalSearchAdapterMarker>>(emptyList())
    val adapterListLiveData = _adapterListLiveData.readOnly()

    private var startElementsJob: Job? = null
    private var editTextChangedJob: Job? = null

    override var mainTabsEnum: MainTabsEnum? = MainTabsEnum.TAB_MORE

    override fun onFirstAttach() {
        logDebug("onFirstAttach", TAG)
        viewModelScope.launchWithDispatcher {
            setStartElements()
        }
    }

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        navigateUp()
    }


    fun onEditTextFocusChanged(model: CommonEditTextUi) {
        logDebug("onEditTextFocusChanged", TAG)
        editTextChangedJob?.cancel()
        editTextChangedJob = viewModelScope.launchWithDispatcher {
            changeEditText(model)
            if (model.hasFocus.not()) {
                checkEditText(model)
                _adapterListLiveData.emit(currentList.toList())
            }
        }
    }

    fun onEditTextDone(model: CommonEditTextUi) {
        logDebug("onEditTextDone", TAG)
        editTextChangedJob?.cancel()
        editTextChangedJob = viewModelScope.launchWithDispatcher {
            changeEditText(model)
            checkEditText(model)
            _adapterListLiveData.emit(currentList.toList())
        }
    }

    fun onEditTextChanged(model: CommonEditTextUi) {
        logDebug("onEditTextChanged", TAG)
        editTextChangedJob?.cancel()
        editTextChangedJob = viewModelScope.launchWithDispatcher {
            changeEditText(model)
        }
    }

    fun onButtonClicked(model: CommonButtonUi) {
        logDebug("onButtonClicked", TAG)
        when (model.elementEnum) {
            EmpowermentLegalSearchElementsEnumUi.BUTTON_CONFIRM -> {
                val legalEntityNumberInputField =
                    currentList.filterIsInstance<CommonEditTextUi>().first()
                checkEditText(legalEntityNumberInputField)

                when {
                    currentList.filterIsInstance<CommonButtonUi>()
                        .first().isEnabled -> toEmpowermentsLegal()

                    else -> viewModelScope.launchWithDispatcher {
                        _adapterListLiveData.emit(currentList.toList())
                    }
                }
            }
        }
    }

    private fun toEmpowermentsLegal() {
        logDebug("toEmpowermentsLegal", TAG)
        val legalNumber = currentList.filterIsInstance<CommonEditTextUi>()
            .firstOrNull { it.elementEnum == EmpowermentLegalSearchElementsEnumUi.EDIT_TEXT_LEGAL_ENTITY_NUMBER }?.selectedValue
            ?: ""
        navigateInFlow(
            EmpowermentLegalSearchFragmentDirections.toEmpowermentLegalFragment(
                legalNumber = legalNumber
            )
        )
    }

    private fun changeEditText(model: CommonEditTextUi) {
        currentList.firstOrNull {
            it.elementEnum == model.elementEnum
        }?.let {
            currentList[currentList.indexOf(it)] = model.copy()
        }
    }

    private fun checkEditText(model: CommonEditTextUi) {
        currentList.firstOrNull {
            it.elementEnum == model.elementEnum
        }?.let { element ->
            val index = currentList.indexOf(element)
            currentList[index] = when {
                model.selectedValue.isNullOrEmpty() -> model.copy(validationError = StringSource(R.string.error_field_required))
                (model.selectedValue.length == 9 || model.selectedValue.length == 13).not() -> model.copy(
                    validationError = StringSource(
                        R.string.empowerments_inquiry_legal_entity_search_invalid_input,
                    )
                )

                else -> model.copy(validationError = null)
            }
        }
        validateInput()
    }

    private fun validateInput() {
        val areInputsValid = currentList.filterIsInstance<CommonEditTextUi>()
            .all { it.selectedValue.isNullOrEmpty().not() && it.validationError == null }
        val confirmButton = currentList.last() as CommonButtonUi
        currentList[currentList.lastIndex] = confirmButton.copy(isEnabled = areInputsValid)
    }

    private fun setStartElements() {
        startElementsJob?.cancel()
        startElementsJob = viewModelScope.launchWithDispatcher {
            currentList.apply {
                clear()
                add(
                    CommonEditTextUi(
                        required = true,
                        question = false,
                        minSymbols = 0,
                        selectedValue = null,
                        type = CommonEditTextUiType.NUMBERS,
                        title = EmpowermentLegalSearchElementsEnumUi.EDIT_TEXT_LEGAL_ENTITY_NUMBER.title,
                        elementEnum = EmpowermentLegalSearchElementsEnumUi.EDIT_TEXT_LEGAL_ENTITY_NUMBER,
                    )
                )
                add(
                    CommonButtonUi(
                        title = EmpowermentLegalSearchElementsEnumUi.BUTTON_CONFIRM.title,
                        elementEnum = EmpowermentLegalSearchElementsEnumUi.BUTTON_CONFIRM,
                        buttonColor = ButtonColorUi.BLUE,
                        isEnabled = false
                    )
                )
            }
            _adapterListLiveData.emit(currentList.toList())
        }
    }

}