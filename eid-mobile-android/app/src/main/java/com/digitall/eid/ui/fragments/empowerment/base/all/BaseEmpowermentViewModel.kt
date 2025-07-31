/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.empowerment.base.all

import androidx.lifecycle.LiveData
import androidx.lifecycle.asLiveData
import androidx.lifecycle.viewModelScope
import androidx.paging.PagingData
import com.digitall.eid.R
import com.digitall.eid.domain.extensions.readOnly
import com.digitall.eid.domain.models.empowerment.common.all.EmpowermentSortingByEnum
import com.digitall.eid.domain.models.empowerment.common.all.EmpowermentSortingDirectionEnum
import com.digitall.eid.domain.models.empowerment.common.filter.EmpowermentFilterModel
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.launchWithDispatcher
import com.digitall.eid.extensions.readOnly
import com.digitall.eid.extensions.setValueOnMainThread
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.empowerment.common.all.EmpowermentAdapterMarker
import com.digitall.eid.models.empowerment.common.all.EmpowermentElementsEnumUi
import com.digitall.eid.models.empowerment.common.all.EmpowermentSortingModelUi
import com.digitall.eid.models.empowerment.common.all.EmpowermentSpinnerElementsEnumUi
import com.digitall.eid.models.list.CommonSpinnerUi
import com.digitall.eid.ui.BaseViewModel
import com.digitall.eid.utils.SingleLiveEvent
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.launch

abstract class BaseEmpowermentViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "BaseEmpowermentViewModelTag"
    }

    abstract fun updateSortingModel()

    abstract fun updateSortingModel(
        sortBy: EmpowermentSortingByEnum,
        sortDirection: EmpowermentSortingDirectionEnum,
    )

    abstract fun toCreate(model: CommonSpinnerUi? = null)

    abstract fun toCancel(model: CommonSpinnerUi)

    abstract fun toInquiry()

    abstract fun toFilter()

    abstract fun onSortingSpinnerClicked()

    protected val _sortingCriteriaSpinnerData = SingleLiveEvent<CommonSpinnerUi>()
    val sortingCriteriaSpinnerData = _sortingCriteriaSpinnerData.readOnly()

    protected val _currentSortingCriteriaData = MutableStateFlow(StringSource(""))
    val currentSortingCriteriaData = _currentSortingCriteriaData.readOnly()

    private val _expandOptionsEvent = SingleLiveEvent<Boolean>()
    val expandOptionsEvent = _expandOptionsEvent.readOnly()

    private val _isFilterInitEvent = SingleLiveEvent<Boolean>()
    val isFilterInitEvent = _isFilterInitEvent.readOnly()

    abstract val empowermentsPagingDataFlow: Flow<PagingData<EmpowermentAdapterMarker>>

    lateinit var adapterListLiveData: LiveData<PagingData<EmpowermentAdapterMarker>>

    protected var filteringModel = MutableStateFlow(
        EmpowermentFilterModel(
            empowermentNumber = null,
            status = null,
            authorizer = null,
            onBehalfOf = null,
            fromNameOf = null,
            serviceName = null,
            validToDate = null,
            providerName = null,
            authorizerList = emptyList(),
            onBehalfOfList = emptyList(),
            serviceNameList = emptyList(),
            providerNameList = emptyList(),
            showOnlyNoExpiryDate = false,
            empoweredIDs = null,
            eik = null,
        )
    )

    init {
        viewModelScope.launch {
            filteringModel.collect { filter ->
                val isFilterActive = filter.allPropertiesAreNull.not()
                if (_isFilterInitEvent.value != isFilterActive) {
                    _isFilterInitEvent.postValue(isFilterActive)
                }
            }
        }
    }

    override fun onCreated() {
        super.onCreated()
        adapterListLiveData = empowermentsPagingDataFlow.asLiveData()
    }

    fun checkFilteringModelState() = _isFilterInitEvent.setValueOnMainThread(filteringModel.value.allPropertiesAreNull.not())

    final override fun onSpinnerSelected(model: CommonSpinnerUi) {
        logDebug("onSpinnerSelected", TAG)
        viewModelScope.launchWithDispatcher {
            when (model.elementEnum) {
                EmpowermentElementsEnumUi.SPINNER_SORTING_CRITERIA -> {
                    val identifier = model.selectedValue?.elementEnum
                    if (identifier !is EmpowermentSortingModelUi) return@launchWithDispatcher
                    updateSortingModel(
                        sortBy = identifier.sortBy,
                        sortDirection = identifier.sortDirection,
                    )
                }

                EmpowermentElementsEnumUi.SPINNER_MENU -> {
                    when (model.selectedValue?.elementEnum) {
                        EmpowermentSpinnerElementsEnumUi.SPINNER_COPY -> toCreate(model)
                        else -> toCancel(model)
                    }

                }
            }
        }
    }

    protected fun getSortingTitleByModel(model: EmpowermentSortingModelUi): StringSource {
        logDebug("getSortingTitleByModel", TAG)
        return when {
            model.sortBy == EmpowermentSortingByEnum.DEFAULT -> StringSource(R.string.empowerments_entity_by_default_sorting_enum)

            model.sortBy == EmpowermentSortingByEnum.ID &&
                    model.sortDirection == EmpowermentSortingDirectionEnum.ASC ->
                StringSource(R.string.empowerments_entity_identifier_asc_sorting_enum)

            model.sortBy == EmpowermentSortingByEnum.ID &&
                    model.sortDirection == EmpowermentSortingDirectionEnum.DESC ->
                StringSource(R.string.empowerments_entity_identifier_desc_sorting_enum)

            model.sortBy == EmpowermentSortingByEnum.CREATED_ON &&
                    model.sortDirection == EmpowermentSortingDirectionEnum.ASC ->
                StringSource(R.string.empowerments_entity_created_on_asc_sorting_enum)

            model.sortBy == EmpowermentSortingByEnum.CREATED_ON &&
                    model.sortDirection == EmpowermentSortingDirectionEnum.DESC ->
                StringSource(R.string.empowerments_entity_created_on_desc_sorting_enum)

            model.sortBy == EmpowermentSortingByEnum.NAME &&
                    model.sortDirection == EmpowermentSortingDirectionEnum.ASC ->
                StringSource(R.string.empowerments_entity_on_behalf_off_asc_sorting_enum)

            model.sortBy == EmpowermentSortingByEnum.NAME &&
                    model.sortDirection == EmpowermentSortingDirectionEnum.DESC ->
                StringSource(R.string.empowerments_entity_on_behalf_off_desc_sorting_enum)

            model.sortBy == EmpowermentSortingByEnum.AUTHORIZER &&
                    model.sortDirection == EmpowermentSortingDirectionEnum.ASC ->
                StringSource(R.string.empowerments_entity_empowerer_asc_sorting_enum)

            model.sortBy == EmpowermentSortingByEnum.AUTHORIZER &&
                    model.sortDirection == EmpowermentSortingDirectionEnum.DESC ->
                StringSource(R.string.empowerments_entity_empowerer_desc_sorting_enum)

            model.sortBy == EmpowermentSortingByEnum.PROVIDER_NAME &&
                    model.sortDirection == EmpowermentSortingDirectionEnum.ASC ->
                StringSource(R.string.empowerments_entity_provider_asc_sorting_enum)

            model.sortBy == EmpowermentSortingByEnum.PROVIDER_NAME &&
                    model.sortDirection == EmpowermentSortingDirectionEnum.DESC ->
                StringSource(R.string.empowerments_entity_provider_desc_sorting_enum)

            model.sortBy == EmpowermentSortingByEnum.SERVICE_NAME &&
                    model.sortDirection == EmpowermentSortingDirectionEnum.ASC ->
                StringSource(R.string.empowerments_entity_service_asc_sorting_enum)

            model.sortBy == EmpowermentSortingByEnum.SERVICE_NAME &&
                    model.sortDirection == EmpowermentSortingDirectionEnum.DESC ->
                StringSource(R.string.empowerments_entity_service_desc_sorting_enum)

            model.sortBy == EmpowermentSortingByEnum.STATUS &&
                    model.sortDirection == EmpowermentSortingDirectionEnum.ASC ->
                StringSource(R.string.empowerments_entity_status_asc_sorting_enum)

            model.sortBy == EmpowermentSortingByEnum.STATUS &&
                    model.sortDirection == EmpowermentSortingDirectionEnum.DESC ->
                StringSource(R.string.empowerments_entity_status_desc_sorting_enum)

            else -> {
                logError("getSortingTitleByModel else", TAG)
                StringSource(R.string.unknown)
            }
        }
    }

    fun updateFilteringModel(filterModel: EmpowermentFilterModel) {
        filteringModel.value = filterModel
    }

    fun expandHideOptions(isVisible: Boolean) {
        _expandOptionsEvent.setValueOnMainThread(isVisible)
    }

}