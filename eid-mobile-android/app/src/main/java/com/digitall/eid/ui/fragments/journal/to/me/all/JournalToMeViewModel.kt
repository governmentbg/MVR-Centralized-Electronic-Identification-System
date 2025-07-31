/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.journal.to.me.all

import androidx.lifecycle.viewModelScope
import androidx.paging.Pager
import androidx.paging.PagingConfig
import androidx.paging.PagingData
import androidx.paging.cachedIn
import com.digitall.eid.R
import com.digitall.eid.domain.DELAY_250
import com.digitall.eid.domain.LOCALIZATIONS
import com.digitall.eid.domain.usecase.journal.GetJournalToMeUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.mappers.journal.JournalToMeUiMapper
import com.digitall.eid.models.journal.common.all.JournalAdapterMarker
import com.digitall.eid.models.main.MainTabsEnum
import com.digitall.eid.ui.fragments.journal.base.all.BaseJournalViewModel
import com.digitall.eid.ui.fragments.journal.to.me.all.list.JournalToMeDataSource
import kotlinx.coroutines.ExperimentalCoroutinesApi
import kotlinx.coroutines.FlowPreview
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.debounce
import kotlinx.coroutines.flow.distinctUntilChanged
import kotlinx.coroutines.flow.flatMapLatest
import org.koin.core.component.inject

@OptIn(ExperimentalCoroutinesApi::class, FlowPreview::class)
class JournalToMeViewModel : BaseJournalViewModel() {

    companion object {
        private const val TAG = "JournalToMeViewModelTag"
        const val CURSOR_SIZE = 20
    }

    private val journalToMeUiMapper: JournalToMeUiMapper by inject()
    private val getJournalToMeUseCase: GetJournalToMeUseCase by inject()

    override var mainTabsEnum: MainTabsEnum? = MainTabsEnum.TAB_MORE

    private val localizations
        get() = LOCALIZATIONS.logs

    override val logsPagingDataFlow: Flow<PagingData<JournalAdapterMarker>> =
        filteringModel.debounce(DELAY_250)
            .distinctUntilChanged()
            .flatMapLatest { filter ->
                Pager(
                    config = PagingConfig(
                        pageSize = CURSOR_SIZE,
                        initialLoadSize = CURSOR_SIZE,
                        enablePlaceholders = false,
                    ),
                    pagingSourceFactory = {
                        JournalToMeDataSource(
                            filterModel = filter,
                            logsLocalizations = localizations,
                            journalToMeUiMapper = journalToMeUiMapper,
                            getJournalToMeUseCase = getJournalToMeUseCase,
                        )
                    }
                ).flow
            }.cachedIn(viewModelScope)

    override fun toFilter() {
        logDebug("toFilter", TAG)
        navigateInFlow(
            JournalToMeFragmentDirections.toJournalToMeFilterFragment(
                filter = filteringModel.value,
                localizations = localizations.toTypedArray()
            )
        )
    }

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        popBackStackToFragmentInTab(R.id.journalFlowFragment)
    }

}