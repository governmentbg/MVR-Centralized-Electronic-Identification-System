/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.empowerment.from.me.all

import androidx.lifecycle.viewModelScope
import androidx.paging.Pager
import androidx.paging.PagingConfig
import androidx.paging.PagingData
import androidx.paging.cachedIn
import com.digitall.eid.R
import com.digitall.eid.domain.DELAY_250
import com.digitall.eid.domain.models.empowerment.common.all.EmpowermentItem
import com.digitall.eid.domain.models.empowerment.common.all.EmpowermentSortingByEnum
import com.digitall.eid.domain.models.empowerment.common.all.EmpowermentSortingDirectionEnum
import com.digitall.eid.domain.models.empowerment.common.all.EmpowermentSortingModel
import com.digitall.eid.domain.usecase.empowerment.from.me.GetEmpowermentFromMeUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.extensions.launchWithDispatcher
import com.digitall.eid.extensions.setValueOnMainThread
import com.digitall.eid.mappers.empowerment.common.all.EmpowermentSortingModelUiMapper
import com.digitall.eid.mappers.empowerment.from.me.all.EmpowermentFromMeUiMapper
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.empowerment.common.all.EmpowermentAdapterMarker
import com.digitall.eid.models.empowerment.common.all.EmpowermentElementsEnumUi
import com.digitall.eid.models.empowerment.common.all.EmpowermentSortingModelUi
import com.digitall.eid.models.empowerment.from.me.all.EmpowermentFromMeUi
import com.digitall.eid.models.list.CommonSpinnerMenuItemUi
import com.digitall.eid.models.list.CommonSpinnerUi
import com.digitall.eid.models.main.MainTabsEnum
import com.digitall.eid.ui.fragments.empowerment.base.all.BaseEmpowermentViewModel
import com.digitall.eid.ui.fragments.empowerment.from.me.all.list.EmpowermentFromMeDataSource
import com.digitall.eid.ui.fragments.main.tabs.more.MainTabMoreFragmentDirections
import kotlinx.coroutines.ExperimentalCoroutinesApi
import kotlinx.coroutines.FlowPreview
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.combine
import kotlinx.coroutines.flow.debounce
import kotlinx.coroutines.flow.distinctUntilChanged
import kotlinx.coroutines.flow.flatMapLatest
import org.koin.core.component.inject

@OptIn(ExperimentalCoroutinesApi::class, FlowPreview::class)
class EmpowermentFromMeViewModel : BaseEmpowermentViewModel() {

    companion object {
        private const val TAG = "EmpowermentFromMeViewModelTag"
        private const val PAGE_SIZE = 20
    }

    private val empowermentFromMeUiMapper: EmpowermentFromMeUiMapper by inject()
    private val getEmpowermentFromMeUseCase: GetEmpowermentFromMeUseCase by inject()
    private val empowermentSortingModelUiMapper: EmpowermentSortingModelUiMapper by inject()

    override var mainTabsEnum: MainTabsEnum? = MainTabsEnum.TAB_MORE

    private var sortingModel = MutableStateFlow(
        EmpowermentSortingModel(
            sortBy = EmpowermentSortingByEnum.DEFAULT,
            sortDirection = EmpowermentSortingDirectionEnum.ASC
        )
    )

    override val empowermentsPagingDataFlow: Flow<PagingData<EmpowermentAdapterMarker>> = combine(
        filteringModel.debounce(DELAY_250),
        sortingModel.debounce(DELAY_250)
    ) { filter, sortCriteria ->
        Pair(filter, sortCriteria)
    }.distinctUntilChanged()
        .flatMapLatest { (filter, sortCriteria) ->
            Pager(
                config = PagingConfig(
                    pageSize = PAGE_SIZE,
                    initialLoadSize = PAGE_SIZE,
                    enablePlaceholders = false,
                ),
                pagingSourceFactory = {
                    EmpowermentFromMeDataSource(
                        sortingModel = sortCriteria,
                        filterModel = filter,
                        empowermentFromMeUiMapper = empowermentFromMeUiMapper,
                        getEmpowermentFromMeUseCase = getEmpowermentFromMeUseCase,
                    )
                }
            ).flow
        }.cachedIn(viewModelScope)

    override fun onFirstAttach() {
        logDebug("onFirstAttach", TAG)
        viewModelScope.launchWithDispatcher {
            updateSortingModel()
        }
    }

    override fun onDetached() {
        logDebug("onDetached", TAG)
    }

    override fun updateSortingModel() {
        logDebug("updateSortingModel", TAG)
        _currentSortingCriteriaData.value = getSortingTitleByModel(
            empowermentSortingModelUiMapper.map(sortingModel.value)
        )
    }

    override fun updateSortingModel(
        sortBy: EmpowermentSortingByEnum,
        sortDirection: EmpowermentSortingDirectionEnum,
    ) {
        logDebug("updateSortingModel", TAG)
        sortingModel.value = sortingModel.value.copy(sortBy = sortBy, sortDirection = sortDirection)
    }

    override fun toFilter() {
        logDebug("toFilter", TAG)
        navigateInFlow(
            EmpowermentFromMeFragmentDirections.toEmpowermentFromMeFilterFragment(
                model = filteringModel.value
            )
        )
    }

    fun toDetails(model: EmpowermentFromMeUi) {
        logDebug("toDetails", TAG)
        navigateInFlow(
            EmpowermentFromMeFragmentDirections.toEmpowermentFromMeDetailsFragment(
                model = model.originalModel
            )
        )
    }

    fun toSigning(model: EmpowermentFromMeUi) {
        logDebug("toSigning", TAG)
        navigateInFlow(
            EmpowermentFromMeFragmentDirections.toEmpowermentFromMeSigningFragment(
                model = model.originalModel
            )
        )
    }

    override fun toInquiry() {
        logDebug("toInquiry", TAG)
        navigateInFlow(
            EmpowermentFromMeFragmentDirections.toEmpowermentLegalFlowFragment()
        )
    }

    override fun toCreate(model: CommonSpinnerUi?) {
        logDebug("toCreate", TAG)
        val empowermentItem = model?.selectedValue?.originalModel as? EmpowermentItem
        navigateInTab(
            MainTabMoreFragmentDirections.toEmpowermentCreateFlowFragment(
                model = empowermentItem
            )
        )
    }

    override fun toCancel(model: CommonSpinnerUi) {
        logDebug("toCancel", TAG)
        val empowermentItem = model.selectedValue?.originalModel as? EmpowermentItem ?: return
        navigateInFlow(
            EmpowermentFromMeFragmentDirections.toEmpowermentFromMeCancelFragment(
                model = empowermentItem
            )
        )
    }

    override fun onSortingSpinnerClicked() {
        logDebug("onSortingCriteriaClicked", TAG)
        viewModelScope.launchWithDispatcher {
            val currentSortingModel = sortingModel.value
            val data = CommonSpinnerUi(
                required = false,
                question = false,
                selectedValue = null,
                title = StringSource(""),
                elementEnum = EmpowermentElementsEnumUi.SPINNER_SORTING_CRITERIA,
                list = buildList {
                    var identifier = EmpowermentSortingModelUi(
                        sortBy = EmpowermentSortingByEnum.DEFAULT,
                        sortDirection = EmpowermentSortingDirectionEnum.ASC
                    )
                    add(
                        CommonSpinnerMenuItemUi(
                            isSelected = currentSortingModel.sortBy == identifier.sortBy &&
                                    currentSortingModel.sortDirection == identifier.sortDirection,
                            elementEnum = identifier,
                            text = getSortingTitleByModel(identifier),
                        )
                    )
                    identifier = EmpowermentSortingModelUi(
                        sortBy = EmpowermentSortingByEnum.CREATED_ON,
                        sortDirection = EmpowermentSortingDirectionEnum.ASC
                    )
                    add(
                        CommonSpinnerMenuItemUi(
                            isSelected = currentSortingModel.sortBy == identifier.sortBy &&
                                    currentSortingModel.sortDirection == identifier.sortDirection,
                            elementEnum = identifier,
                            text = getSortingTitleByModel(identifier),
                        )
                    )
                    identifier = EmpowermentSortingModelUi(
                        sortBy = EmpowermentSortingByEnum.CREATED_ON,
                        sortDirection = EmpowermentSortingDirectionEnum.DESC
                    )
                    add(
                        CommonSpinnerMenuItemUi(
                            isSelected = currentSortingModel.sortBy == identifier.sortBy &&
                                    currentSortingModel.sortDirection == identifier.sortDirection,
                            text = getSortingTitleByModel(identifier),
                            elementEnum = identifier,
                        )
                    )
                    identifier = EmpowermentSortingModelUi(
                        sortBy = EmpowermentSortingByEnum.NAME,
                        sortDirection = EmpowermentSortingDirectionEnum.ASC
                    )
                    add(
                        CommonSpinnerMenuItemUi(
                            isSelected = currentSortingModel.sortBy == identifier.sortBy &&
                                    currentSortingModel.sortDirection == identifier.sortDirection,
                            text = getSortingTitleByModel(identifier),
                            elementEnum = identifier,
                        )
                    )
                    identifier = EmpowermentSortingModelUi(
                        sortBy = EmpowermentSortingByEnum.NAME,
                        sortDirection = EmpowermentSortingDirectionEnum.DESC
                    )
                    add(
                        CommonSpinnerMenuItemUi(
                            isSelected = currentSortingModel.sortBy == identifier.sortBy &&
                                    currentSortingModel.sortDirection == identifier.sortDirection,
                            text = getSortingTitleByModel(identifier),
                            elementEnum = identifier,
                        )
                    )
                    identifier = EmpowermentSortingModelUi(
                        sortBy = EmpowermentSortingByEnum.PROVIDER_NAME,
                        sortDirection = EmpowermentSortingDirectionEnum.ASC
                    )
                    add(
                        CommonSpinnerMenuItemUi(
                            isSelected = currentSortingModel.sortBy == identifier.sortBy &&
                                    currentSortingModel.sortDirection == identifier.sortDirection,
                            text = getSortingTitleByModel(identifier),
                            elementEnum = identifier,
                        )
                    )
                    identifier = EmpowermentSortingModelUi(
                        sortBy = EmpowermentSortingByEnum.PROVIDER_NAME,
                        sortDirection = EmpowermentSortingDirectionEnum.DESC
                    )
                    add(
                        CommonSpinnerMenuItemUi(
                            isSelected = currentSortingModel.sortBy == identifier.sortBy &&
                                    currentSortingModel.sortDirection == identifier.sortDirection,
                            text = getSortingTitleByModel(identifier),
                            elementEnum = identifier,
                        )
                    )
                    identifier = EmpowermentSortingModelUi(
                        sortBy = EmpowermentSortingByEnum.SERVICE_NAME,
                        sortDirection = EmpowermentSortingDirectionEnum.ASC
                    )
                    add(
                        CommonSpinnerMenuItemUi(
                            isSelected = currentSortingModel.sortBy == identifier.sortBy &&
                                    currentSortingModel.sortDirection == identifier.sortDirection,
                            text = getSortingTitleByModel(identifier),
                            elementEnum = identifier,
                        )
                    )
                    identifier = EmpowermentSortingModelUi(
                        sortBy = EmpowermentSortingByEnum.SERVICE_NAME,
                        sortDirection = EmpowermentSortingDirectionEnum.DESC
                    )
                    add(
                        CommonSpinnerMenuItemUi(
                            isSelected = currentSortingModel.sortBy == identifier.sortBy &&
                                    currentSortingModel.sortDirection == identifier.sortDirection,
                            text = getSortingTitleByModel(identifier),
                            elementEnum = identifier,
                        )
                    )
                    identifier = EmpowermentSortingModelUi(
                        sortBy = EmpowermentSortingByEnum.STATUS,
                        sortDirection = EmpowermentSortingDirectionEnum.ASC
                    )
                    add(
                        CommonSpinnerMenuItemUi(
                            isSelected = currentSortingModel.sortBy == identifier.sortBy &&
                                    currentSortingModel.sortDirection == identifier.sortDirection,
                            text = getSortingTitleByModel(identifier),
                            elementEnum = identifier,
                        )
                    )
                    identifier = EmpowermentSortingModelUi(
                        sortBy = EmpowermentSortingByEnum.STATUS,
                        sortDirection = EmpowermentSortingDirectionEnum.DESC
                    )
                    add(
                        CommonSpinnerMenuItemUi(
                            isSelected = currentSortingModel.sortBy == identifier.sortBy &&
                                    currentSortingModel.sortDirection == identifier.sortDirection,
                            text = getSortingTitleByModel(identifier),
                            elementEnum = identifier,
                        )
                    )
                },
            )
            _sortingCriteriaSpinnerData.setValueOnMainThread(data)
        }
    }

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        popBackStackToFragmentInTab(R.id.empowermentFlowFragment)
    }

}