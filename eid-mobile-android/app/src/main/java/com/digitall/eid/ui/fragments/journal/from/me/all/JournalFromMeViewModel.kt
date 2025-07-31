/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.journal.from.me.all

import androidx.lifecycle.viewModelScope
import androidx.paging.Pager
import androidx.paging.PagingConfig
import androidx.paging.PagingData
import androidx.paging.cachedIn
import com.digitall.eid.R
import com.digitall.eid.domain.DELAY_250
import com.digitall.eid.domain.LOCALIZATIONS
import com.digitall.eid.domain.usecase.journal.GetJournalFromMeUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.mappers.journal.JournalFromMeUiMapper
import com.digitall.eid.models.journal.common.all.JournalAdapterMarker
import com.digitall.eid.models.main.MainTabsEnum
import com.digitall.eid.ui.fragments.journal.base.all.BaseJournalViewModel
import com.digitall.eid.ui.fragments.journal.from.me.all.list.JournalFromMeDataSource
import kotlinx.coroutines.ExperimentalCoroutinesApi
import kotlinx.coroutines.FlowPreview
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.debounce
import kotlinx.coroutines.flow.distinctUntilChanged
import kotlinx.coroutines.flow.flatMapLatest
import org.koin.core.component.inject

@OptIn(ExperimentalCoroutinesApi::class, FlowPreview::class)
class JournalFromMeViewModel : BaseJournalViewModel() {

    companion object {
        private const val TAG = "JournalFromMeViewModelTag"
        const val CURSOR_SIZE = 20
    }

    private val journalFromMeUiMapper: JournalFromMeUiMapper by inject()
    private val getJournalFromMeUseCase: GetJournalFromMeUseCase by inject()

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
                        JournalFromMeDataSource(
                            filterModel = filter,
                            logsLocalizations = localizations,
                            journalFromMeUiMapper = journalFromMeUiMapper,
                            getJournalFromMeUseCase = getJournalFromMeUseCase,
                        )
                    }
                ).flow
            }.cachedIn(viewModelScope)

    override fun toFilter() {
        logDebug("toFilter", TAG)
        navigateInFlow(
            JournalFromMeFragmentDirections.toJournalFromMeFilterFragment(
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