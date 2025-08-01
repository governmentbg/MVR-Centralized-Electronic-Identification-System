package com.digitall.eid.ui.fragments.empowerment.legal.all

import android.view.View
import androidx.lifecycle.lifecycleScope
import androidx.navigation.fragment.navArgs
import androidx.paging.PagingData
import com.digitall.eid.R
import com.digitall.eid.domain.DELAY_1000
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.launchWithDispatcher
import com.digitall.eid.extensions.registerChangeStateObserver
import com.digitall.eid.extensions.unregisterChangeStateObserver
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.empowerment.legal.all.EmpowermentLegalUi
import com.digitall.eid.ui.fragments.empowerment.base.all.BaseEmpowermentFragment
import com.digitall.eid.ui.fragments.empowerment.legal.all.list.EmpowermentLegalAdapter
import com.digitall.eid.utils.RecyclerViewAdapterDataObserver
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.Job
import kotlinx.coroutines.delay
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.viewModel
import kotlin.properties.Delegates

class EmpowermentLegalFragment : BaseEmpowermentFragment<EmpowermentLegalViewModel>(),
    EmpowermentLegalAdapter.ClickListener {

    companion object {
        private const val TAG = "EmpowermentLegalFragmentTag"
    }

    override val viewModel: EmpowermentLegalViewModel by viewModel()
    override val adapter: EmpowermentLegalAdapter by inject()
    private val adapterDataObserver: RecyclerViewAdapterDataObserver by inject()
    private val args: EmpowermentLegalFragmentArgs by navArgs()

    private var updateEmptyStateJob: Job? = null

    private var legalNumber: String? by Delegates.observable(
        null
    ) { _, _, newValue ->
        binding.toolbar.setTitleText(StringSource(R.string.empowerments_legal_entity_title, listOf(newValue ?: "")))
    }

    override var areOptionsVisible: Boolean = true

    override fun parseArguments() {
        try {
            legalNumber = args.legalNumber
            viewModel.setupModel(legalNumber)
        } catch (exception: IllegalStateException) {
            logError(
                "parseArguments Exception: ${exception.message}",
                exception,
                TAG
            )
        }
    }

    override fun setupView() {
        super.setupView()
        binding.toolbar.setTitleText(StringSource(R.string.empowerments_legal_entity_title, listOf(legalNumber ?: "")))
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
    }

    override fun onCopyClicked(model: EmpowermentLegalUi, anchor: View) {
        logDebug("onCopyClocked", TAG)
        showSpinner(
            anchor = anchor,
            model = model.spinnerModel,
        )
    }

    override fun onSignClicked(model: EmpowermentLegalUi) {
        logDebug("onSignClicked", TAG)
        viewModel.toSigning(model)
    }

    override fun onOpenClicked(model: EmpowermentLegalUi) {
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
                adapter.submitData(data as PagingData<EmpowermentLegalUi>)
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