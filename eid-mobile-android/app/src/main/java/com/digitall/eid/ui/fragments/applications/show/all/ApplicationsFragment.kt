/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.applications.show.all

import android.view.View
import androidx.core.graphics.drawable.toDrawable
import androidx.fragment.app.clearFragmentResult
import androidx.fragment.app.clearFragmentResultListener
import androidx.fragment.app.setFragmentResultListener
import androidx.lifecycle.lifecycleScope
import androidx.navigation.fragment.navArgs
import androidx.paging.LoadState
import com.digitall.eid.R
import com.digitall.eid.data.extensions.getParcelableCompat
import com.digitall.eid.databinding.FragmentWithListAndSortBinding
import com.digitall.eid.domain.DELAY_1000
import com.digitall.eid.domain.ENVIRONMENT
import com.digitall.eid.domain.FILTER_MODEL_KEY
import com.digitall.eid.domain.FILTER_MODEL_REQUEST_KEY
import com.digitall.eid.domain.REFRESH_APPLICATIONS_KEY
import com.digitall.eid.domain.REFRESH_APPLICATIONS_REQUEST_KEY
import com.digitall.eid.domain.models.base.ErrorType
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.findParentFragmentResultListenerFragmentManager
import com.digitall.eid.extensions.launchWithDispatcher
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.extensions.openUrlInBrowser
import com.digitall.eid.extensions.registerChangeStateObserver
import com.digitall.eid.extensions.setTextSource
import com.digitall.eid.extensions.unregisterChangeStateObserver
import com.digitall.eid.models.applications.all.ApplicationUi
import com.digitall.eid.models.applications.filter.ApplicationsFilterModel
import com.digitall.eid.models.common.PagingError
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.ui.fragments.applications.show.all.list.ApplicationsAdapter
import com.digitall.eid.ui.fragments.base.BaseFragment
import com.digitall.eid.utils.RecyclerViewAdapterDataObserver
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.Job
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.collectLatest
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.viewModel

class ApplicationsFragment :
    BaseFragment<FragmentWithListAndSortBinding, ApplicationsViewModel>(),
    ApplicationsAdapter.ClickListener {

    companion object {
        private const val TAG = "ApplicationsFragmentTag"
    }

    override fun getViewBinding() = FragmentWithListAndSortBinding.inflate(layoutInflater)
    override val viewModel: ApplicationsViewModel by viewModel()
    private val adapter: ApplicationsAdapter by inject()
    private val adapterDataObserver: RecyclerViewAdapterDataObserver by inject()
    private val args: ApplicationsFragmentArgs by navArgs()

    private var updateEmptyStateJob: Job? = null

    override fun parseArguments() {
        try {
            viewModel.setupModel(
                applicationId = args.applicationId,
                certificateId = args.certificateId,
            )
        } catch (e: IllegalStateException) {
            logError("parseArguments Exception: ${e.message}", e, TAG)
            viewModel.setupModel(
                applicationId = null,
                certificateId = null,
            )
        }
    }

    override fun setupView() {
        binding.recyclerView.adapter = adapter
        binding.toolbar.setTitleText(StringSource(R.string.applications_title))

        viewLifecycleOwner.lifecycleScope.launch {
            adapter.loadStateFlow.collectLatest { loadStates ->
                val refreshState = loadStates.refresh

                when (refreshState) {
                    is LoadState.Loading -> {
                        viewModel.showLoader()
                        viewModel.hideErrorState()
                    }
                    is LoadState.NotLoading -> {
                        viewModel.hideErrorState()
                        if (adapter.itemCount == 0) {
                            showEmptyState()
                        } else {
                            showReadyState()
                        }
                        viewModel.hideLoader()
                    }
                    is LoadState.Error -> {
                        viewModel.hideLoader()
                        val throwableError = refreshState.error
                        logError("Paging refresh error: ${throwableError.message}", throwableError, TAG)

                        when (throwableError) {
                            is PagingError -> {
                                when (throwableError.errorType) {
                                    ErrorType.AUTHORIZATION -> viewModel.toLoginFragment()
                                    else -> viewModel.showErrorState(
                                        title = throwableError.title,
                                        description = throwableError.displayMessage,
                                    )
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    override fun setupControls() {
        binding.toolbar.navigationClickListener = {
            viewModel.onBackPressed()
        }
        binding.refreshLayout.setOnRefreshListener {
            binding.refreshLayout.isRefreshing = false
            adapter.refresh()
        }
        binding.errorView.actionOneClickListener = {
            adapter.refresh()
        }
        binding.errorView.actionTwoClickListener = {
            adapter.refresh()
        }
        binding.emptyStateView.reloadClickListener = {
            adapter.refresh()
        }
        binding.tvSort.onClickThrottle {
            viewModel.onSortingSpinnerClicked()
        }
        binding.icFilter.onClickThrottle {
            viewModel.onFilterClicked()
        }
        adapter.clickListener = this
        setFragmentResultListener(FILTER_MODEL_REQUEST_KEY) { _, bundle ->
            val filterModel = bundle.getParcelableCompat<ApplicationsFilterModel>(FILTER_MODEL_KEY)
            filterModel?.let {
                viewModel.updateFilteringModel(it)
            }
        }
        findParentFragmentResultListenerFragmentManager()?.setFragmentResultListener(
            REFRESH_APPLICATIONS_REQUEST_KEY,
            viewLifecycleOwner
        ) { _, bundle ->
            if (bundle.getBoolean(REFRESH_APPLICATIONS_KEY, false)) {
                adapter.refresh()
            }
        }
        setFragmentResultListener(REFRESH_APPLICATIONS_REQUEST_KEY) { _, bundle ->
            val shouldRefresh = bundle.getBoolean(REFRESH_APPLICATIONS_KEY, false)
            if (shouldRefresh) {
                adapter.refresh()
            }
        }
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
        viewModel.adapterListLiveData.observe(viewLifecycleOwner) { data ->
            viewLifecycleOwner.lifecycleScope.launchWithDispatcher {
                adapter.submitData(data)
                withContext(Dispatchers.Main) {
                    setupEmptyState(adapter.itemCount)
                    binding.recyclerView.smoothScrollToPosition(0)
                }
            }
        }
        viewModel.sortingCriteriaSpinnerLiveData.observe(viewLifecycleOwner) {
            showSpinner(
                model = it,
                anchor = binding.tvSort
            )
        }
        viewModel.currentSortingCriteriaLiveData.observe(viewLifecycleOwner) {
            binding.tvSort.setTextSource(it.title)
            adapter.refresh()
        }

        viewModel.isFilterInitEvent.observe(viewLifecycleOwner) { flag ->
            binding.icFilter.background = binding.root.context.getColor(
                if (flag) R.color.color_F59E0B else R.color.color_transparent
            ).toDrawable()
        }

        viewModel.openPaymentEvent.observe(viewLifecycleOwner) { paymentAccessCode ->
            val basePaymentUrl = ENVIRONMENT.urlPayment
            val paymentUrl =
                paymentAccessCode?.let { "$basePaymentUrl?code=$it" } ?: run { basePaymentUrl }
            context?.openUrlInBrowser(url = paymentUrl)
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

    override fun onOpenClicked(model: ApplicationUi) {
        logDebug("onOpenClicked", TAG)
        viewModel.onOpenClicked(model)
    }

    override fun onSpinnerClicked(model: ApplicationUi, anchor: View) {
        logDebug("onSpinnerClicked", TAG)
        model.spinnerModel?.let {
            showSpinner(model = it, anchor = anchor)
        }
    }

    override fun onPaused() {
        logDebug("onPaused", TAG)
        adapter.unregisterChangeStateObserver(adapterDataObserver)
    }

    override fun onDestroyed() {
        findParentFragmentResultListenerFragmentManager()?.clearFragmentResult(
            REFRESH_APPLICATIONS_KEY
        )
        findParentFragmentResultListenerFragmentManager()?.clearFragmentResultListener(
            REFRESH_APPLICATIONS_REQUEST_KEY
        )
        clearFragmentResult(FILTER_MODEL_KEY)
        clearFragmentResultListener(FILTER_MODEL_REQUEST_KEY)
        clearFragmentResult(REFRESH_APPLICATIONS_KEY)
        clearFragmentResultListener(REFRESH_APPLICATIONS_REQUEST_KEY)
    }

}