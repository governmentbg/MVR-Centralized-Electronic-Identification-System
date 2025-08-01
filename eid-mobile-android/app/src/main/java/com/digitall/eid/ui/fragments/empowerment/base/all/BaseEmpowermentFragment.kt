/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.empowerment.base.all

import android.view.View
import androidx.annotation.CallSuper
import androidx.core.graphics.drawable.toDrawable
import androidx.fragment.app.setFragmentResultListener
import androidx.lifecycle.lifecycleScope
import androidx.paging.LoadState
import androidx.paging.PagingDataAdapter
import androidx.recyclerview.widget.RecyclerView
import com.digitall.eid.R
import com.digitall.eid.data.extensions.getParcelableCompat
import com.digitall.eid.databinding.FragmentWithListAndSortAndActionsButtonBinding
import com.digitall.eid.domain.FILTER_MODEL_KEY
import com.digitall.eid.domain.FILTER_MODEL_REQUEST_KEY
import com.digitall.eid.domain.models.base.ErrorType
import com.digitall.eid.domain.models.empowerment.common.filter.EmpowermentFilterModel
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.enableChangeAnimations
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.extensions.launchWhenResumed
import com.digitall.eid.extensions.launchWithDispatcher
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.models.common.PagingError
import com.digitall.eid.models.empowerment.common.all.EmpowermentAdapterMarker
import com.digitall.eid.ui.fragments.base.BaseFragment
import com.digitall.eid.ui.fragments.certificates.all.CertificatesFragment
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.collectLatest
import kotlinx.coroutines.flow.onEach
import kotlinx.coroutines.launch

abstract class BaseEmpowermentFragment<VM : BaseEmpowermentViewModel> :
    BaseFragment<FragmentWithListAndSortAndActionsButtonBinding, VM>() {

    companion object {
        private const val TAG = "BaseEmpowermentFragmentTag"
        private const val REQUEST_REFRESH_KEY = "REQUEST_REFRESH_KEY"
        private const val REFRESH_FLAG_KEY = "REFRESH_FLAG_KEY"
    }

    final override fun getViewBinding() =
        FragmentWithListAndSortAndActionsButtonBinding.inflate(layoutInflater)

    abstract val adapter: PagingDataAdapter<out EmpowermentAdapterMarker, RecyclerView.ViewHolder>

    open var areOptionsVisible = false

    private var areOptionsExpanded = false

    @CallSuper
    override fun setupView() {
        binding.recyclerView.adapter = adapter
        binding.recyclerView.enableChangeAnimations(false)
        binding.btnOptions.isExtended = false
        binding.btnOptions.visibility = if (areOptionsVisible) View.VISIBLE else View.GONE

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
                        logError("Paging refresh error: ${throwableError.message}", throwableError,
                            TAG
                        )

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

    @CallSuper
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
        binding.icFilter.onClickThrottle {
            viewModel.toFilter()
        }
        binding.tvSort.onClickThrottle {
            viewModel.onSortingSpinnerClicked()
        }
        binding.btnOptions.onClickThrottle {
            viewModel.expandHideOptions(isVisible = areOptionsExpanded).also {
                areOptionsExpanded = areOptionsExpanded.not()
            }
        }
        binding.btnInquiry.onClickThrottle {
            viewModel.toInquiry()
        }
        binding.btnCreate.onClickThrottle {
            viewModel.toCreate()
        }
        setFragmentResultListener(FILTER_MODEL_REQUEST_KEY) { _, bundle ->
            val filterModel =
                bundle.getParcelableCompat(FILTER_MODEL_KEY) as? EmpowermentFilterModel
            filterModel?.let { filter ->
                viewModel.updateFilteringModel(filter)
            }
        }
        setFragmentResultListener(REQUEST_REFRESH_KEY) { _, bundle ->
            val refreshFlag = bundle.getBoolean(REFRESH_FLAG_KEY)
            if (refreshFlag) {
                adapter.refresh()
            }
        }
    }

    override fun subscribeToLiveData() {
        viewModel.sortingCriteriaSpinnerData.observe(viewLifecycleOwner) {
            showSpinner(
                model = it,
                anchor = binding.tvSort
            )
        }
        viewModel.expandOptionsEvent.observe(viewLifecycleOwner) {
            when (it) {
                true -> {
                    binding.btnCreate.visibility = View.GONE
                    binding.tvCreate.visibility = View.GONE
                    binding.btnInquiry.visibility = View.GONE
                    binding.tvInquiry.visibility = View.GONE
                    binding.btnOptions.shrink()
                }

                else -> {
                    binding.btnCreate.visibility = View.VISIBLE
                    binding.tvCreate.visibility = View.VISIBLE
                    binding.btnInquiry.visibility = View.VISIBLE
                    binding.tvInquiry.visibility = View.VISIBLE
                    binding.btnOptions.extend()
                }
            }
        }
        viewModel.currentSortingCriteriaData.onEach {
            binding.tvSort.text = it.getString(requireContext())
        }.launchInScope(lifecycleScope)

        viewModel.isFilterInitEvent.observe(viewLifecycleOwner) { flag ->
            binding.icFilter.background = binding.root.context.getColor(
                if (flag) R.color.color_F59E0B else R.color.color_transparent
            ).toDrawable()
        }
    }
}