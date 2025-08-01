/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.applications.show.all

import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.asLiveData
import androidx.lifecycle.viewModelScope
import androidx.paging.Pager
import androidx.paging.PagingConfig
import androidx.paging.PagingData
import androidx.paging.cachedIn
import com.digitall.eid.domain.APPLICATION_LANGUAGE
import com.digitall.eid.domain.DELAY_250
import com.digitall.eid.domain.models.applications.all.ApplicationItem
import com.digitall.eid.domain.usecase.applications.all.GetApplicationsUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.extensions.launchWithDispatcher
import com.digitall.eid.extensions.readOnly
import com.digitall.eid.extensions.setValueOnMainThread
import com.digitall.eid.mappers.applications.show.all.ApplicationsUiMapper
import com.digitall.eid.models.applications.all.ApplicationUi
import com.digitall.eid.models.applications.all.ApplicationsElementsEnumUi
import com.digitall.eid.models.applications.all.ApplicationsSortCriteriaEnum
import com.digitall.eid.models.applications.all.ApplicationsSpinnerElementsEnumUi
import com.digitall.eid.models.applications.filter.ApplicationsFilterModel
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonSpinnerMenuItemUi
import com.digitall.eid.models.list.CommonSpinnerUi
import com.digitall.eid.models.main.MainTabsEnum
import com.digitall.eid.ui.BaseViewModel
import com.digitall.eid.ui.fragments.applications.show.all.list.ApplicationsDataSource
import com.digitall.eid.utils.SingleLiveEvent
import kotlinx.coroutines.ExperimentalCoroutinesApi
import kotlinx.coroutines.FlowPreview
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.combine
import kotlinx.coroutines.flow.debounce
import kotlinx.coroutines.flow.distinctUntilChanged
import kotlinx.coroutines.flow.flatMapLatest
import kotlinx.coroutines.launch
import org.koin.core.component.inject

@OptIn(ExperimentalCoroutinesApi::class, FlowPreview::class)
class ApplicationsViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "ApplicationsViewModelTag"
        const val CURSOR_SIZE = 20
    }

    override var mainTabsEnum: MainTabsEnum? = MainTabsEnum.TAB_EIM

    private val applicationsUiMapper: ApplicationsUiMapper by inject()
    private val getApplicationsUseCase: GetApplicationsUseCase by inject()

    private var currentSortingCriteria = MutableStateFlow(ApplicationsSortCriteriaEnum.DEFAULT)

    private var filteringModel = MutableStateFlow(
        ApplicationsFilterModel(
            id = null,
            status = null,
            createDate = null,
            deviceType = null,
            administrator = null,
            applicationType = null,
            applicationNumber = null,
        )
    )

    private var applicationsPagingDataFlow: Flow<PagingData<ApplicationUi>> = combine(
        filteringModel.debounce(DELAY_250),
        currentSortingCriteria
    ) { filter, sortCriteria ->
        Pair(filter, sortCriteria)
    }.distinctUntilChanged()
        .flatMapLatest { (filter, sortCriteria) ->
            Pager(
                config = PagingConfig(
                    pageSize = CURSOR_SIZE,
                    initialLoadSize = CURSOR_SIZE,
                    enablePlaceholders = false,
                ),
                pagingSourceFactory = {
                    ApplicationsDataSource(
                        sortCriteria.type,
                        filterModel = filter,
                        applicationsUiMapper = applicationsUiMapper,
                        getApplicationsUseCase = getApplicationsUseCase,
                        language = APPLICATION_LANGUAGE
                    )
                }
            ).flow
        }.cachedIn(viewModelScope)


    val adapterListLiveData: LiveData<PagingData<ApplicationUi>> =
        applicationsPagingDataFlow.asLiveData()


    private val _sortingCriteriaSpinnerLiveData = SingleLiveEvent<CommonSpinnerUi>()
    val sortingCriteriaSpinnerLiveData = _sortingCriteriaSpinnerLiveData.readOnly()

    private val _currentSortingCriteriaLiveData =
        MutableLiveData(ApplicationsSortCriteriaEnum.DEFAULT)
    val currentSortingCriteriaLiveData = _currentSortingCriteriaLiveData.readOnly()

    private val _isFilterInitEvent = SingleLiveEvent<Boolean>()
    val isFilterInitEvent = _isFilterInitEvent.readOnly()

    private val _openPaymentEvent = SingleLiveEvent<String?>()
    val openPaymentEvent = _openPaymentEvent.readOnly()

    private var applicationId: String? = null

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

    fun setupModel(
        applicationId: String?,
        certificateId: String?,
    ) {
        logDebug("setupModel applicationId: $applicationId", TAG)
        if (this.applicationId.isNullOrEmpty()) {
            this.applicationId = applicationId
            if (!applicationId.isNullOrEmpty()) {
                toDetails(
                    applicationId = applicationId,
                    certificateId = certificateId,
                )
            }
        }
    }

    fun updateFilteringModel(newFilterModel: ApplicationsFilterModel) {
        filteringModel.value = newFilterModel
    }

    fun checkFilteringModelState() = _isFilterInitEvent.setValueOnMainThread(filteringModel.value.allPropertiesAreNull.not())

    fun onSortingSpinnerClicked() {
        logDebug("onSortingCriteriaClicked", TAG)
        viewModelScope.launchWithDispatcher {
            val current = currentSortingCriteriaLiveData.value
            val data = CommonSpinnerUi(
                required = false,
                question = false,
                selectedValue = null,
                title = StringSource(""),
                elementEnum = ApplicationsElementsEnumUi.SPINNER_SORTING_CRITERIA,
                list = ApplicationsSortCriteriaEnum.entries.map {
                    CommonSpinnerMenuItemUi(
                        elementEnum = it,
                        text = it.title,
                        isSelected = it == current
                    )
                }
            )
            _sortingCriteriaSpinnerLiveData.setValueOnMainThread(data)
        }
    }

    override fun onSpinnerSelected(model: CommonSpinnerUi) {
        logDebug("onSpinnerSelected", TAG)
        viewModelScope.launchWithDispatcher {
            when (model.elementEnum) {
                ApplicationsElementsEnumUi.SPINNER_SORTING_CRITERIA -> {
                    val newCriteria =
                        model.selectedValue?.elementEnum as? ApplicationsSortCriteriaEnum
                            ?: return@launchWithDispatcher
                    currentSortingCriteria.value = newCriteria
                    _currentSortingCriteriaLiveData.setValueOnMainThread(newCriteria)
                }

                ApplicationsElementsEnumUi.SPINNER_MENU -> {
                    when (model.selectedValue?.elementEnum) {
                        ApplicationsSpinnerElementsEnumUi.SPINNER_PAYMENT -> _openPaymentEvent.setValueOnMainThread(
                            (model.selectedValue.originalModel as? ApplicationItem)?.paymentAccessCode
                        )
                    }
                }
            }
        }
    }

    fun onOpenClicked(model: ApplicationUi) {
        logDebug("onOpenClicked", TAG)
        toDetails(
            applicationId = model.id,
            certificateId = null,
        )
    }

    private fun toDetails(applicationId: String, certificateId: String?) {
        logDebug("toDetails applicationId: $applicationId", TAG)
        navigateInFlow(
            ApplicationsFragmentDirections.toApplicationDetailsFragment(
                applicationId = applicationId,
                certificateId = certificateId
            )
        )
    }

    fun onFilterClicked() {
        logDebug("onFilterClicked", TAG)
        navigateInFlow(
            ApplicationsFragmentDirections.toApplicationDetailsFilterFragment(
                model = filteringModel.value
            )
        )
    }

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        backToTab()
    }

}