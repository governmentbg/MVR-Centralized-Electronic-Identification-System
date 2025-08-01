/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.empowerment.to.me.all

import android.view.View
import androidx.lifecycle.lifecycleScope
import androidx.paging.PagingData
import com.digitall.eid.R
import com.digitall.eid.domain.DELAY_1000
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.extensions.launchWithDispatcher
import com.digitall.eid.extensions.registerChangeStateObserver
import com.digitall.eid.extensions.unregisterChangeStateObserver
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.empowerment.to.me.all.EmpowermentToMeUi
import com.digitall.eid.ui.fragments.empowerment.base.all.BaseEmpowermentFragment
import com.digitall.eid.ui.fragments.empowerment.to.me.all.list.EmpowermentToMeAdapter
import com.digitall.eid.utils.RecyclerViewAdapterDataObserver
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.Job
import kotlinx.coroutines.delay
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.viewModel

class EmpowermentToMeFragment :
    BaseEmpowermentFragment<EmpowermentToMeViewModel>(),
    EmpowermentToMeAdapter.ClickListener {

    companion object {
        private const val TAG = "EmpowermentToMeFragmentTag"
    }

    override val viewModel: EmpowermentToMeViewModel by viewModel()
    override val adapter: EmpowermentToMeAdapter by inject()
    private val adapterDataObserver: RecyclerViewAdapterDataObserver by inject()

    private var updateEmptyStateJob: Job? = null

    override fun setupView() {
        super.setupView()
        binding.toolbar.setTitleText(StringSource(R.string.empowerment_to_me_title))
    }

    override fun setupControls() {
        super.setupControls()
        adapter.clickListener = this
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

    override fun onSpinnerClicked(model: EmpowermentToMeUi, anchor: View) {
        logDebug("onCopyClocked", TAG)
        if (model.spinnerModel == null) return
        showSpinner(
            anchor = anchor,
            model = model.spinnerModel,
        )
    }

    override fun onOpenClicked(model: EmpowermentToMeUi) {
        logDebug("onOpenClicked", TAG)
        viewModel.toDetails(
            model = model
        )
    }

    override fun subscribeToLiveData() {
        super.subscribeToLiveData()
        viewModel.adapterListLiveData.observe(viewLifecycleOwner) { data ->
            viewLifecycleOwner.lifecycleScope.launchWithDispatcher {
                @Suppress("UNCHECKED_CAST")
                adapter.submitData(data as PagingData<EmpowermentToMeUi>)
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