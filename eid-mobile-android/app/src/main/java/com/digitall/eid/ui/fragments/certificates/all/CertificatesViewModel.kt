/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.certificates.all

import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.asLiveData
import androidx.lifecycle.viewModelScope
import androidx.paging.DataSource
import androidx.paging.Pager
import androidx.paging.PagingConfig
import androidx.paging.PagingData
import androidx.paging.cachedIn
import com.digitall.eid.domain.DELAY_250
import com.digitall.eid.domain.DEVICES
import com.digitall.eid.domain.extensions.getEnumValue
import com.digitall.eid.domain.models.certificates.CertificateItem
import com.digitall.eid.domain.models.common.DeviceType
import com.digitall.eid.domain.usecase.certificates.GetCertificatesUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.extensions.launchWithDispatcher
import com.digitall.eid.extensions.readOnly
import com.digitall.eid.extensions.setValueOnMainThread
import com.digitall.eid.mappers.certificates.all.CertificatesUiMapper
import com.digitall.eid.models.certificates.all.CertificateSortCriteriaEnum
import com.digitall.eid.models.certificates.all.CertificateUi
import com.digitall.eid.models.certificates.all.CertificatesElementsEnumUi
import com.digitall.eid.models.certificates.all.CertificatesSpinnerElementsEnumUi
import com.digitall.eid.models.certificates.details.CertificateDetailsType
import com.digitall.eid.models.certificates.filter.CertificatesFilterModel
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonSpinnerMenuItemUi
import com.digitall.eid.models.list.CommonSpinnerUi
import com.digitall.eid.models.main.MainTabsEnum
import com.digitall.eid.ui.BaseViewModel
import com.digitall.eid.ui.fragments.certificates.all.list.CertificatesDataSource
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
class CertificatesViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "CertificatesViewModelTag"
        const val CURSOR_SIZE = 20
    }

    override var mainTabsEnum: MainTabsEnum? = MainTabsEnum.TAB_EIM

    private val certificatesUiMapper: CertificatesUiMapper by inject()
    private val getCertificatesUseCase: GetCertificatesUseCase by inject()

    private var certificateId: String? = null

    private var filteringModel = MutableStateFlow(
        CertificatesFilterModel(
            id = null,
            status = null,
            deviceType = null,
            serialNumber = null,
            validityFrom = null,
            validityUntil = null,
            administrator = null,
            alias = null,
        )
    )

    private var currentSortingCriteria = MutableStateFlow(CertificateSortCriteriaEnum.DEFAULT)

    private var certificatesPagingDataFlow: Flow<PagingData<CertificateUi>> = combine(
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
                    CertificatesDataSource(
                        sortCriteria.type,
                        filterModel = filter,
                        certificatesUiMapper = certificatesUiMapper,
                        getCertificatesUseCase = getCertificatesUseCase,
                    )
                }
            ).flow
        }.cachedIn(viewModelScope)

    val adapterListLiveData: LiveData<PagingData<CertificateUi>> =
        certificatesPagingDataFlow.asLiveData()

    private val _sortingCriteriaSpinnerLiveData = SingleLiveEvent<CommonSpinnerUi>()
    val sortingCriteriaSpinnerLiveData = _sortingCriteriaSpinnerLiveData.readOnly()

    private val _currentSortingCriteriaLiveData =
        MutableLiveData(CertificateSortCriteriaEnum.DEFAULT)
    val currentSortingCriteriaLiveData = _currentSortingCriteriaLiveData.readOnly()

    private val _isFilterInitEvent = SingleLiveEvent<Boolean>()
    val isFilterInitEvent = _isFilterInitEvent.readOnly()

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

    fun checkFilteringModelState() = _isFilterInitEvent.setValueOnMainThread(filteringModel.value.allPropertiesAreNull.not())

    fun setupID(
        applicationId: String?,
        certificateId: String?,
    ) {
        logDebug("setupID certificateId: $certificateId applicationId: $applicationId", TAG)
        if (this.certificateId.isNullOrEmpty()) {
            this.certificateId = certificateId
            if (!certificateId.isNullOrEmpty()) {
                toDetails(
                    certificateId = certificateId,
                    applicationId = applicationId,
                )
            }
        }
    }

    fun updateFilteringModel(filterModel: CertificatesFilterModel) {
        filteringModel.value = filterModel
    }

    fun onSortingSpinnerClicked() {
        logDebug("onSortingCriteriaClicked", TAG)
        viewModelScope.launchWithDispatcher {
            val current = currentSortingCriteriaLiveData.value
            val data = CommonSpinnerUi(
                required = false,
                question = false,
                selectedValue = null,
                title = StringSource(""),
                elementEnum = CertificatesElementsEnumUi.SPINNER_SORTING_CRITERIA,
                list = CertificateSortCriteriaEnum.entries.map {
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
                CertificatesElementsEnumUi.SPINNER_SORTING_CRITERIA -> {
                    val newCriteria =
                        model.selectedValue?.elementEnum as? CertificateSortCriteriaEnum
                            ?: return@launchWithDispatcher
                    currentSortingCriteria.value = newCriteria
                    _currentSortingCriteriaLiveData.setValueOnMainThread(newCriteria)
                }

                CertificatesElementsEnumUi.SPINNER_MENU -> {
                    when (model.selectedValue?.elementEnum) {
                        CertificatesSpinnerElementsEnumUi.SPINNER_PAUSE -> {
                            (model.selectedValue.originalModel as? CertificateItem)?.let { certificate ->
                                toCertificateStop(id = certificate.id ?: return@let)
                            }
                        }

                        CertificatesSpinnerElementsEnumUi.SPINNER_RESUME -> {
                            (model.selectedValue.originalModel as? CertificateItem)?.let { certificate ->
                                toCertificateResume(id = certificate.id ?: return@let)
                            }
                        }

                        CertificatesSpinnerElementsEnumUi.SPINNER_REVOKE -> {
                            (model.selectedValue.originalModel as? CertificateItem)?.let { certificate ->
                                toCertificateRevoke(id = certificate.id ?: return@let)
                            }
                        }

                        CertificatesSpinnerElementsEnumUi.SPINNER_CHANGE_PIN -> {
                            (model.selectedValue.originalModel as? CertificateItem)?.let { certificate ->
                                DEVICES.firstOrNull { device -> device.id == certificate.deviceId }?.type?.let { type ->
                                    toCertificateChangePin(
                                        type = getEnumValue<DeviceType>(type) ?: DeviceType.OTHER
                                    )
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    fun onOpenClicked(model: CertificateUi) {
        logDebug("onOpenClicked model: $model", TAG)
        toDetails(
            certificateId = model.id,
            applicationId = null,
        )
    }

    private fun toDetails(
        applicationId: String?,
        certificateId: String,
        detailsType: CertificateDetailsType = CertificateDetailsType.DETAILS
    ) {
        logDebug("toDetails certificateId: $certificateId applicationId: $applicationId", TAG)
        navigateInFlow(
            CertificatesFragmentDirections.toCertificateDetailsFragment(
                certificateId = certificateId,
                applicationId = applicationId,
                detailsType = detailsType
            )
        )
    }

    private fun toCertificateStop(id: String) {
        logDebug("toCertificateStop", TAG)
        toDetails(
            applicationId = null,
            certificateId = id,
            detailsType = CertificateDetailsType.DETAILS_STOP
        )
    }

    private fun toCertificateResume(id: String) {
        logDebug("toCertificateResume", TAG)
        toDetails(
            applicationId = null,
            certificateId = id,
            detailsType = CertificateDetailsType.DETAIL_RESUME
        )
    }

    private fun toCertificateRevoke(id: String) {
        logDebug("toCertificateResume", TAG)
        toDetails(
            applicationId = null,
            certificateId = id,
            detailsType = CertificateDetailsType.DETAILS_REVOKE
        )
    }

    private fun toCertificateChangePin(type: DeviceType) {
        logDebug("toCertificateChangePin", TAG)
        navigateInFlow(
            CertificatesFragmentDirections.toCertificateChangePinFragment(type)
        )
    }

    fun onFilterClicked() {
        logDebug("onFilterClicked", TAG)
        navigateInFlow(
            CertificatesFragmentDirections.toCertificateDetailsFilterFragment(
                model = filteringModel.value
            )
        )
    }

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        backToTab()
    }
}