package com.digitall.eid.ui.fragments.journal.base.filter

import androidx.lifecycle.viewModelScope
import com.digitall.eid.R
import com.digitall.eid.domain.ToServerDateFormats
import com.digitall.eid.domain.extensions.getCalendar
import com.digitall.eid.domain.extensions.readOnly
import com.digitall.eid.domain.extensions.toServerDate
import com.digitall.eid.domain.models.assets.localization.logs.LogLocalizationModel
import com.digitall.eid.domain.models.journal.filter.JournalFilterModel
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.launchWithDispatcher
import com.digitall.eid.extensions.readOnly
import com.digitall.eid.extensions.setValueOnMainThread
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.journal.common.filter.JournalFilterAdapterMarker
import com.digitall.eid.models.journal.common.filter.JournalFilterButtonsEnumUi
import com.digitall.eid.models.journal.common.filter.JournalFilterElementEnumUi
import com.digitall.eid.models.list.CommonDatePickerUi
import com.digitall.eid.models.list.CommonDialogWithSearchMultiselectUi
import com.digitall.eid.models.list.CommonDoubleButtonUi
import com.digitall.eid.ui.BaseViewModel
import com.digitall.eid.utils.SingleLiveEvent
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.withContext

abstract class BaseJournalFilterViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "BaseJournalFilterViewModelTag"
    }

    private val currentList = mutableListOf<JournalFilterAdapterMarker>()

    private val _adapterListLiveData =
        MutableStateFlow<List<JournalFilterAdapterMarker>>(emptyList())
    val adapterListLiveData = _adapterListLiveData.readOnly()

    private val _scrollToPositionLiveData = SingleLiveEvent<Int>()
    val scrollToPositionLiveData = _scrollToPositionLiveData.readOnly()

    private val _applyFilterDataLiveData = SingleLiveEvent<JournalFilterModel>()
    val applyFilterDataLiveData = _applyFilterDataLiveData.readOnly()

    private lateinit var filteringModel: JournalFilterModel
    private lateinit var localizations: Array<LogLocalizationModel>

    abstract fun getStartScreenElements(
        model: JournalFilterModel,
        localizations: Array<LogLocalizationModel>
    ): List<JournalFilterAdapterMarker>

    fun setViewModel(filterModel: JournalFilterModel, localizations: Array<LogLocalizationModel>) {
        filteringModel = filterModel
        this.localizations = localizations
        viewModelScope.launchWithDispatcher {
            currentList.clear()
            currentList.addAll(getStartScreenElements(filteringModel, localizations))
            _adapterListLiveData.emit(currentList.toList())
            delay(100)
            _scrollToPositionLiveData.setValueOnMainThread(0)
        }
    }

    final override fun onFirstAttach() {
        logDebug("onFirstAttach", TAG)
    }

    final override fun onDatePickerChanged(model: CommonDatePickerUi) {
        logDebug("onDatePickerChanged", TAG)
        viewModelScope.launchWithDispatcher {
            currentList.firstOrNull {
                it.elementEnum == model.elementEnum
            }?.let {
                val index = currentList.indexOf(it)
                currentList[index] = model.copy()
                when (it.elementEnum) {
                    JournalFilterElementEnumUi.DATE_PICKER_START_DATE -> {
                        (currentList[index + 1] as? CommonDatePickerUi)?.let { element ->
                            currentList[index + 1] = element.copy(
                                minDate = model.selectedValue ?: getCalendar(
                                    minusDays = 100,
                                )
                            )
                        }
                    }

                    JournalFilterElementEnumUi.DATE_PICKER_END_DATE -> {
                        (currentList[index - 1] as? CommonDatePickerUi)?.let { element ->
                            currentList[index - 1] = element.copy(
                                maxDate = model.selectedValue ?: getCalendar()
                            )
                        }
                    }
                }
                _adapterListLiveData.emit(currentList.toList())
            } ?: run {
                logError("onDatePickerChanged null", TAG)
                showErrorState(
                    title = StringSource(R.string.error_internal_error_short),
                    description = StringSource("List element not found")
                )
            }
        }
    }

    final override fun onDialogMultiselectSelected(model: CommonDialogWithSearchMultiselectUi) {
        logDebug("onDialogElementSelected", TAG)
        viewModelScope.launchWithDispatcher {
            if (model.selectedValue == null) return@launchWithDispatcher
            currentList.firstOrNull {
                it.elementEnum == model.elementEnum
            }?.let {
                currentList[currentList.indexOf(it)] = model.copy()
                _adapterListLiveData.emit(currentList.toList())
            } ?: run {
                logError("onDialogElementSelected null", TAG)
                showErrorState(
                    title = StringSource(R.string.error_internal_error_short),
                    description = StringSource("List element not found")
                )
            }
        }
    }

    fun clearFilter() {
        logDebug("clearFilter", TAG)
        filteringModel = JournalFilterModel(
            startDate = null,
            endDate = null,
            eventTypes = emptyList(),
        )
        _applyFilterDataLiveData.setValueOnMainThread(filteringModel)
        onBackPressed()
    }

    fun applyFilter() {
        viewModelScope.launchWithDispatcher {
            tryToApplyFilterData()
            _applyFilterDataLiveData.setValueOnMainThread(filteringModel)
            withContext(Dispatchers.Main) {
                onBackPressed()
            }
        }
    }

    private suspend fun tryToApplyFilterData() {
        logDebug("tryToApplyFilterData", TAG)
        _adapterListLiveData.emit(currentList.toList())
        applyFilterData()
    }

    private fun applyFilterData() {
        var startDate: String? = null
        var endDate: String? = null
        var eventTypes = emptyList<String>()
        var allEventTypesSelected = false
        currentList.forEach { element ->
            when (element) {
                is CommonDatePickerUi -> {

                    when (element.elementEnum) {
                        JournalFilterElementEnumUi.DATE_PICKER_START_DATE -> startDate =
                            element.selectedValue?.toServerDate(dateFormat = ToServerDateFormats.ONLY_DATE)

                        JournalFilterElementEnumUi.DATE_PICKER_END_DATE -> endDate =
                            element.selectedValue?.toServerDate(dateFormat = ToServerDateFormats.ONLY_DATE)
                    }
                }

                is CommonDialogWithSearchMultiselectUi -> {
                    eventTypes = element.selectedValue?.map { it.serverValue ?: "" }
                        ?.takeWhile { string -> string.isEmpty().not() } ?: emptyList()
                    allEventTypesSelected =
                        element.list.firstOrNull { it.isSelectAllOption }?.isSelected
                            ?: false
                }
            }
        }

        filteringModel = filteringModel.copy(
            startDate = startDate,
            endDate = endDate,
            eventTypes = eventTypes,
            allEventTypesSelected = allEventTypesSelected,
        )
    }
}