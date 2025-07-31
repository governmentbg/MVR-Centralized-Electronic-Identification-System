/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.journal.from.me.all

import androidx.lifecycle.lifecycleScope
import androidx.paging.PagingData
import com.digitall.eid.R
import com.digitall.eid.domain.DELAY_1000
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.extensions.launchWithDispatcher
import com.digitall.eid.extensions.registerChangeStateObserver
import com.digitall.eid.extensions.unregisterChangeStateObserver
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.journal.from.me.JournalFromMeUi
import com.digitall.eid.ui.fragments.journal.base.all.BaseJournalFragment
import com.digitall.eid.ui.fragments.journal.from.me.all.list.JournalFromMeAdapter
import com.digitall.eid.utils.RecyclerViewAdapterDataObserver
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.Job
import kotlinx.coroutines.delay
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.viewModel

class JournalFromMeFragment :
    BaseJournalFragment<JournalFromMeViewModel>() {

    companion object {
        private const val TAG = "JournalFromMeFragmentTag"
    }

    override val viewModel: JournalFromMeViewModel by viewModel()
    override val adapter: JournalFromMeAdapter by inject()
    private val adapterDataObserver: RecyclerViewAdapterDataObserver by inject()

    private var updateEmptyStateJob: Job? = null

    override fun setupView() {
        super.setupView()
        binding.recyclerView.adapter = adapter
        binding.toolbar.setTitleText(StringSource(R.string.journals_from_me_title))
    }

    override fun onResumed() {
        logDebug("onResumed", TAG)
        adapter.registerChangeStateObserver(
            observer = adapterDataObserver,
            changeStateListener = {
                logDebug("adapterDataObserver stateChanged", TAG)
                setupEmptyState(adapter.itemCount)
            }
        )
        viewModel.checkFilteringModelState()
    }

    override fun subscribeToLiveData() {
        super.subscribeToLiveData()
        viewModel.adapterListLiveData.observe(viewLifecycleOwner) { data ->
            viewLifecycleOwner.lifecycleScope.launchWithDispatcher {
                @Suppress("UNCHECKED_CAST")
                adapter.submitData(data as PagingData<JournalFromMeUi>)
                withContext(Dispatchers.Main) {
                    setupEmptyState(adapter.itemCount)
                    binding.recyclerView.smoothScrollToPosition(0)
                }
            }
        }
    }

    private fun setupEmptyState(size: Int?) {
        logDebug("setupEmptyState size: $size", TAG)
        updateEmptyStateJob?.cancel()
        updateEmptyStateJob = lifecycleScope.launch {
            if (size == null || size == 0) {
                delay(DELAY_1000)
                showEmptyState()
            } else {
                showReadyState()
            }
        }
    }

    override fun onPaused() {
        logDebug("onPaused", TAG)
        adapter.unregisterChangeStateObserver(adapterDataObserver)
    }

}