package com.digitall.eid.ui.fragments.journal.base.filter

import android.os.Bundle
import androidx.annotation.CallSuper
import androidx.fragment.app.setFragmentResult
import androidx.lifecycle.lifecycleScope
import com.digitall.eid.databinding.FragmentWithTopCloseButtonBinding
import com.digitall.eid.domain.FILTER_MODEL_KEY
import com.digitall.eid.domain.FILTER_MODEL_REQUEST_KEY
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.extensions.enableChangeAnimations
import com.digitall.eid.extensions.hideKeyboard
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.models.journal.common.filter.JournalFilterAdapterMarker
import com.digitall.eid.models.list.CommonDatePickerUi
import com.digitall.eid.models.list.CommonDialogWithSearchUi
import com.digitall.eid.models.list.CommonDoubleButtonUi
import com.digitall.eid.ui.fragments.base.BaseFragment
import com.digitall.eid.ui.fragments.journal.base.filter.list.BaseJournalFilterAdapter
import kotlinx.coroutines.flow.onEach

abstract class BaseJournalFilterFragment <VM : BaseJournalFilterViewModel> :
    BaseFragment<FragmentWithTopCloseButtonBinding, VM>(),
    BaseJournalFilterAdapter.ClickListener {

    companion object {
        private const val TAG = "BaseJournalFilterFragmentTag"
    }

    final override fun getViewBinding() = FragmentWithTopCloseButtonBinding.inflate(layoutInflater)

    protected abstract val adapter: BaseJournalFilterAdapter

    @CallSuper
    override fun setupView() {
        binding.recyclerView.adapter = adapter
        binding.recyclerView.enableChangeAnimations(false)
    }

    final override fun setupControls() {
        adapter.clickListener = this
        binding.toolbar.navigationClickListener = {
            viewModel.onBackPressed()
        }
        setupDialogWithSearchMultiselectResultListener()
        binding.icClose.onClickThrottle {
            viewModel.onBackPressed()
        }
        binding.applyFilters.onClickThrottle {
            viewModel.applyFilter()
        }
        binding.clearFilters.onClickThrottle {
            viewModel.clearFilter()
        }
    }

    final override fun subscribeToLiveData() {
        viewModel.adapterListLiveData.onEach {
            setAdapterData(it)
        }.launchInScope(lifecycleScope)
        viewModel.scrollToPositionLiveData.observe(viewLifecycleOwner) {
            binding.recyclerView.scrollToPosition(it)
        }
        viewModel.applyFilterDataLiveData.observe(viewLifecycleOwner) { filter ->
            val bundle = Bundle().apply {
                putParcelable(FILTER_MODEL_KEY, filter)
            }
            setFragmentResult(FILTER_MODEL_REQUEST_KEY, bundle)
        }
    }

    private fun setAdapterData(data: List<JournalFilterAdapterMarker>) {
        logDebug("setAdapterData size: ${data.size}", TAG)
        adapter.items = data
        hideKeyboard()
    }

    final override fun onDatePickerClicked(model: CommonDatePickerUi) {
        logDebug("onDatePickerClicked", TAG)
        showDatePicker(model)
    }

    fun onDialogWithSearchClicked(model: CommonDialogWithSearchUi) {
        logDebug("onDialogWithSearchClicked", TAG)
    }

}