/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.empowerment.from.me.all

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
import com.digitall.eid.models.empowerment.common.filter.EmpowermentFilterStatusEnumUi
import com.digitall.eid.models.empowerment.from.me.all.EmpowermentFromMeUi
import com.digitall.eid.ui.fragments.empowerment.base.all.BaseEmpowermentFragment
import com.digitall.eid.ui.fragments.empowerment.from.me.all.list.EmpowermentFromMeAdapter
import com.digitall.eid.utils.RecyclerViewAdapterDataObserver
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.Job
import kotlinx.coroutines.delay
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.viewModel

class EmpowermentFromMeFragment :
    BaseEmpowermentFragment<EmpowermentFromMeViewModel>(),
    EmpowermentFromMeAdapter.ClickListener {

    companion object {
        private const val TAG = "EmpowermentFromMeFragmentTag"
    }

    override val viewModel: EmpowermentFromMeViewModel by viewModel()
    override val adapter: EmpowermentFromMeAdapter by inject()
    private val adapterDataObserver: RecyclerViewAdapterDataObserver by inject()

    private var updateEmptyStateJob: Job? = null

    override var areOptionsVisible: Boolean = true

    override fun setupView() {
        super.setupView()
        binding.toolbar.setTitleText(StringSource(R.string.empowerment_from_me_title))
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

    override fun onCopyClicked(model: EmpowermentFromMeUi, anchor: View) {
        logDebug("onCopyClocked", TAG)
        showSpinner(
            anchor = anchor,
            model = model.spinnerModel,
        )
    }

    override fun onSignClicked(model: EmpowermentFromMeUi) {
        logDebug("onSignClicked", TAG)
        viewModel.toSigning(model)
    }

    override fun onOpenClicked(model: EmpowermentFromMeUi) {
        logDebug("onOpenClicked", TAG)

        when (model.status) {
            EmpowermentFilterStatusEnumUi.COLLECTING_AUTHORIZER_SIGNATURES,
            EmpowermentFilterStatusEnumUi.COLLECTING_WITHDRAWAL_SIGNATURES -> viewModel.toSigning(
                model
            )

            else -> viewModel.toDetails(model)
        }
    }

    override fun subscribeToLiveData() {
        super.subscribeToLiveData()
        viewModel.adapterListLiveData.observe(viewLifecycleOwner) { data ->
            viewLifecycleOwner.lifecycleScope.launchWithDispatcher {
                @Suppress("UNCHECKED_CAST")
                adapter.submitData(data as PagingData<EmpowermentFromMeUi>)
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