/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.main.tabs.requests

import androidx.lifecycle.lifecycleScope
import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentMainTabRequestsBinding
import com.digitall.eid.domain.DELAY_1000
import com.digitall.eid.domain.DELAY_2500
import com.digitall.eid.domain.DELAY_500
import com.digitall.eid.domain.models.challenge.request.SignedChallengeRequestModel
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.extensions.enableChangeAnimations
import com.digitall.eid.extensions.launchWithDispatcher
import com.digitall.eid.extensions.registerChangeStateObserver
import com.digitall.eid.extensions.unregisterChangeStateObserver
import com.digitall.eid.models.common.AlertDialogResult
import com.digitall.eid.models.common.CardScanBottomSheetContent
import com.digitall.eid.models.common.CardScanResult
import com.digitall.eid.models.common.CardScanScreenStates
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.requests.RequestUi
import com.digitall.eid.ui.fragments.card.enter.pin.auth.AuthCardBottomSheetFragment
import com.digitall.eid.ui.fragments.card.scan.ScanCardBottomSheetFragment
import com.digitall.eid.ui.fragments.main.base.BaseMainTabFragment
import com.digitall.eid.ui.fragments.main.tabs.requests.list.TabRequestsAdapter
import com.digitall.eid.utils.RecyclerViewAdapterDataObserver
import kotlinx.coroutines.Job
import kotlinx.coroutines.delay
import kotlinx.coroutines.launch
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.viewModel
import kotlin.system.exitProcess

class MainTabRequestsFragment :
    BaseMainTabFragment<FragmentMainTabRequestsBinding, MainTabRequestsViewModel>(),
    TabRequestsAdapter.ClickListener,
    AuthCardBottomSheetFragment.Listener,
    ScanCardBottomSheetFragment.Listener {

    companion object {
        private const val TAG = "MainTabRequestsFragmentTag"
    }

    override fun getViewBinding() = FragmentMainTabRequestsBinding.inflate(layoutInflater)
    override val viewModel: MainTabRequestsViewModel by viewModel()

    private val adapter: TabRequestsAdapter by inject()
    private val adapterDataObserver: RecyclerViewAdapterDataObserver by inject()

    private var updateEmptyStateJob: Job? = null

    private var scanCardBottomSheet: ScanCardBottomSheetFragment? = null
    private var authCardBottomSheet: AuthCardBottomSheetFragment? = null

    private var approvedRequestId: String? = null

    private var state: CardScanScreenStates? = null

    override fun setupView() {
        binding.recyclerView.adapter = adapter
        binding.recyclerView.enableChangeAnimations(false)
        binding.toolbar.setTitleText(StringSource(R.string.tab_requests_title))
    }

    override fun setupControls() {
        binding.toolbar.navigationClickListener = {
            viewModel.onBackPressed()
        }
        binding.refreshLayout.setOnRefreshListener {
            binding.refreshLayout.isRefreshing = false
            viewModel.refreshData()
        }
        binding.errorView.actionOneClickListener = {
            viewModel.refreshData()
        }
        binding.errorView.actionTwoClickListener = {
            viewModel.refreshData()
        }
        binding.emptyStateView.reloadClickListener = {
            viewModel.refreshData()
        }
        viewModel.adapterListLiveData.observe(viewLifecycleOwner) {
            setAdapterData(it)
        }
        adapter.clickListener = this
    }

    override fun onAlertDialogResult(result: AlertDialogResult) {
        if (result.messageId == DIALOG_EXIT && result.isPositive) {
            exitProcess(0)
        }
    }

    override fun onResumed() {
        logDebug("onResumed", TAG)
        adapter.registerChangeStateObserver(
            observer = adapterDataObserver,
            changeStateListener = {
                logDebug("adapterDataObserver stateChanged", TAG)
                setupEmptyState(adapter.items.size)
            }
        )
        viewModel.refreshData()
    }

    override fun onPaused() {
        logDebug("onPaused", TAG)
        adapter.unregisterChangeStateObserver(adapterDataObserver)
    }

    override fun onHiddenChanged(hidden: Boolean) {
        super.onHiddenChanged(hidden)
        viewModel.onHiddenChanged(hidden = hidden)
    }

    override fun onRequestAccept(model: RequestUi) {
        approvedRequestId = model.id
        authCardBottomSheet =
            AuthCardBottomSheetFragment.newInstance(
                listener = this,
                shouldShowCan = state == CardScanScreenStates.Suspended
            ).also { bottomSheet ->
                bottomSheet.show(parentFragmentManager, "AuthCardBottomSheetFragmentTag")
            }
    }

    override fun onRequestDecline(model: RequestUi) {
        viewModel.declineRequest(requestId = model.id)
    }

    override fun operationCompleted(result: CardScanBottomSheetContent) {
        lifecycleScope.launchWithDispatcher {
            delay(DELAY_500)
            authCardBottomSheet?.dismiss().also {
                authCardBottomSheet = null
                scanCardBottomSheet = ScanCardBottomSheetFragment.newInstance(
                    content = result,
                    listener = this@MainTabRequestsFragment
                ).also { bottomSheet ->
                    bottomSheet.show(parentFragmentManager, "ScanCardBottomSheetFragmentTag")
                }
            }
        }
    }

    override fun operationCompleted(result: CardScanResult) {
        when (result) {
            is CardScanResult.ChallengeSigned -> {
                lifecycleScope.launchWithDispatcher {
                    delay(DELAY_500)
                    scanCardBottomSheet?.dismiss().also {
                        scanCardBottomSheet = null
                        viewModel.acceptRequest(
                            requestId = approvedRequestId,
                            signedChallenge = SignedChallengeRequestModel(
                                signature = result.signature,
                                challenge = result.challenge,
                                certificate = result.certificate,
                                certificateChain = result.certificateChain,
                            )
                        )
                    }
                }
            }

            else -> {}
        }
    }

    override fun operationFailed(state: CardScanScreenStates) {
        lifecycleScope.launchWithDispatcher {
            delay(DELAY_2500)
            this@MainTabRequestsFragment.state = state
            scanCardBottomSheet?.dismiss().also {
                scanCardBottomSheet = null
            }
        }
    }

    private fun setupEmptyState(size: Int?) {
        logDebug("setupEmptyState size: $size", TAG)
        updateEmptyStateJob?.cancel()
        updateEmptyStateJob = lifecycleScope.launch {
            when {
                size == null || size == 0 -> {
                    delay(DELAY_1000)
                    showEmptyState()
                }

                else -> showReadyState()
            }
        }
    }

    private fun setAdapterData(data: List<RequestUi>) {
        logDebug("setAdapterData size: ${data.size}", TAG)
        adapter.items = data
        setupEmptyState(data.size)
    }
}