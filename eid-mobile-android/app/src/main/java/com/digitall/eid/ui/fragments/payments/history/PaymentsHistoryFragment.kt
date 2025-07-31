package com.digitall.eid.ui.fragments.payments.history

import androidx.core.graphics.drawable.toDrawable
import androidx.fragment.app.setFragmentResultListener
import androidx.lifecycle.lifecycleScope
import com.digitall.eid.R
import com.digitall.eid.data.extensions.getParcelableCompat
import com.digitall.eid.databinding.FragmentWithListAndSortBinding
import com.digitall.eid.domain.FILTER_MODEL_KEY
import com.digitall.eid.domain.FILTER_MODEL_REQUEST_KEY
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.extensions.registerChangeStateObserver
import com.digitall.eid.extensions.setTextSource
import com.digitall.eid.extensions.unregisterChangeStateObserver
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.payments.history.filter.PaymentsHistoryFilterModel
import com.digitall.eid.ui.fragments.base.BaseFragment
import com.digitall.eid.ui.fragments.payments.history.list.PaymentsHistoryAdapter
import com.digitall.eid.utils.RecyclerViewAdapterDataObserver
import kotlinx.coroutines.Job
import kotlinx.coroutines.launch
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.viewModel

class PaymentsHistoryFragment :
    BaseFragment<FragmentWithListAndSortBinding, PaymentsHistoryViewModel>() {

    companion object {
        private const val TAG = "PaymentHistoryFragmentTag"
    }

    override fun getViewBinding() = FragmentWithListAndSortBinding.inflate(layoutInflater)

    override val viewModel: PaymentsHistoryViewModel by viewModel()

    private val adapterDataObserver: RecyclerViewAdapterDataObserver by inject()
    private val adapter: PaymentsHistoryAdapter by inject()

    private var updateEmptyStateJob: Job? = null

    override fun setupView() {
        binding.recyclerView.adapter = adapter
        binding.toolbar.setTitleText(StringSource(R.string.payments_history_screen_title))
    }

    override fun onCreated() {
        super.onCreated()
        viewModel.checkIfFilterIsInit()
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

    override fun onPaused() {
        logDebug("onPaused", TAG)
        adapter.unregisterChangeStateObserver(adapterDataObserver)
    }

    override fun subscribeToLiveData() {
        viewModel.adapterListLiveData.observe(viewLifecycleOwner) { data ->
            logDebug("adapterListLiveData size: ${data.size}", TAG)
            adapter.items = data
            setupEmptyState(data.size)
        }
        viewModel.sortingCriteriaSpinnerLiveData.observe(viewLifecycleOwner) { model ->
            showSpinner(
                model =  model,
                anchor = binding.tvSort
            )
        }
        viewModel.currentSortingCriteriaLiveData.observe(viewLifecycleOwner) { criteria ->
            binding.tvSort.setTextSource(criteria.title)
        }
        viewModel.isFilterInitEvent.observe(viewLifecycleOwner) { flag ->
            binding.icFilter.background = binding.root.context.getColor(
                if (flag) R.color.color_F59E0B else R.color.color_transparent
            ).toDrawable()
        }
    }

    override fun setupControls() {
        binding.toolbar.navigationClickListener = {
            viewModel.onBackPressed()
        }
        binding.refreshLayout.setOnRefreshListener {
            binding.refreshLayout.isRefreshing = false
            viewModel.refreshScreen()
        }
        binding.errorView.actionOneClickListener = {
            viewModel.refreshScreen()
        }
        binding.errorView.actionTwoClickListener = {
            viewModel.refreshScreen()
        }
        binding.emptyStateView.reloadClickListener = {
            viewModel.refreshScreen()
        }
        binding.tvSort.onClickThrottle {
            viewModel.onSortingSpinnerClicked()
        }
        binding.icFilter.onClickThrottle {
            viewModel.onFilterClicked()
        }
        setFragmentResultListener(FILTER_MODEL_REQUEST_KEY) { _, bundle ->
            val filterModel = bundle.getParcelableCompat<PaymentsHistoryFilterModel>(FILTER_MODEL_KEY)
            filterModel?.let {
                viewModel.updateFilteringModel(it)
            }
        }
    }

    private fun setupEmptyState(size: Int?) {
        logDebug("setupEmptyState size: $size", TAG)
        updateEmptyStateJob?.cancel()
        updateEmptyStateJob = lifecycleScope.launch {
            if (size == null || size == 0) {
                showEmptyState()
            } else {
                showReadyState()
            }
        }
    }
}