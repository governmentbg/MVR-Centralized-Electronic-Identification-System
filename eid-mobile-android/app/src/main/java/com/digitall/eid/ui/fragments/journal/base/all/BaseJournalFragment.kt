/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.journal.base.all

import androidx.annotation.CallSuper
import androidx.core.graphics.drawable.toDrawable
import androidx.fragment.app.setFragmentResultListener
import androidx.lifecycle.lifecycleScope
import androidx.paging.LoadState
import androidx.paging.PagingDataAdapter
import androidx.recyclerview.widget.RecyclerView
import com.digitall.eid.R
import com.digitall.eid.data.extensions.getParcelableCompat
import com.digitall.eid.databinding.FragmentWithListAndFilterBinding
import com.digitall.eid.domain.FILTER_MODEL_KEY
import com.digitall.eid.domain.FILTER_MODEL_REQUEST_KEY
import com.digitall.eid.domain.models.base.ErrorType
import com.digitall.eid.domain.models.journal.filter.JournalFilterModel
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.enableChangeAnimations
import com.digitall.eid.extensions.launchWithDispatcher
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.models.common.PagingError
import com.digitall.eid.models.journal.common.all.JournalAdapterMarker
import com.digitall.eid.ui.fragments.base.BaseFragment
import kotlinx.coroutines.flow.collectLatest
import kotlinx.coroutines.launch

abstract class BaseJournalFragment<VM : BaseJournalViewModel> :
    BaseFragment<FragmentWithListAndFilterBinding, VM>() {

    companion object {
        private const val TAG = "BaseJournalFragmentTag"
    }

    override fun getViewBinding() = FragmentWithListAndFilterBinding.inflate(layoutInflater)

    abstract val adapter: PagingDataAdapter<out JournalAdapterMarker, RecyclerView.ViewHolder>

    @CallSuper
    override fun setupView() {
        binding.recyclerView.enableChangeAnimations(false)
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
        setFragmentResultListener(FILTER_MODEL_REQUEST_KEY) { _, bundle ->
            val filterModel =
                bundle.getParcelableCompat<JournalFilterModel>(FILTER_MODEL_KEY)
            filterModel?.let { filter ->
                viewModel.updateFilteringModel(filter)
            }
        }
    }

    override fun subscribeToLiveData() {
        viewModel.isFilterInitEvent.observe(viewLifecycleOwner) { flag ->
            binding.icFilter.background = binding.root.context.getColor(
                if (flag) R.color.color_F59E0B else R.color.color_transparent
            ).toDrawable()
        }
    }
}