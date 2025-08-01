/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.empowerment.base.filter

import android.os.Bundle
import android.view.View
import androidx.annotation.CallSuper
import androidx.fragment.app.setFragmentResult
import com.digitall.eid.databinding.FragmentWithTopCloseButtonBinding
import com.digitall.eid.domain.FILTER_MODEL_KEY
import com.digitall.eid.domain.FILTER_MODEL_REQUEST_KEY
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.enableChangeAnimations
import com.digitall.eid.extensions.launchWhenResumed
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.models.empowerment.common.filter.EmpowermentFilterAdapterMarker
import com.digitall.eid.models.list.CommonButtonTransparentUi
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonCheckBoxUi
import com.digitall.eid.models.list.CommonDatePickerUi
import com.digitall.eid.models.list.CommonDialogWithSearchUi
import com.digitall.eid.models.list.CommonEditTextUi
import com.digitall.eid.models.list.CommonSpinnerUi
import com.digitall.eid.ui.fragments.base.BaseFragment
import com.digitall.eid.ui.fragments.empowerment.base.filter.list.BaseEmpowermentFilterAdapter

abstract class BaseEmpowermentFilterFragment<VM : BaseEmpowermentFilterViewModel> :
    BaseFragment<FragmentWithTopCloseButtonBinding, VM>(),
    BaseEmpowermentFilterAdapter.ClickListener {

    companion object {
        private const val TAG = "BaseEmpowermentFilterFragmentTag"
    }

    final override fun getViewBinding() = FragmentWithTopCloseButtonBinding.inflate(layoutInflater)

    protected abstract val adapter: BaseEmpowermentFilterAdapter

    @CallSuper
    override fun setupView() {
        binding.recyclerView.adapter = adapter
        binding.recyclerView.enableChangeAnimations(false)
    }

    final override fun setupControls() {
        adapter.clickListener = this
        adapter.recyclerViewProvider = { binding.recyclerView }
        binding.toolbar.navigationClickListener = {
            viewModel.onBackPressed()
        }
        setupDialogWithSearchResultListener()
        binding.icClose.onClickThrottle {
            viewModel.onBackPressed()
        }
        binding.clearFilters.onClickThrottle {
            viewModel.clearFilterData()
        }
        binding.applyFilters.onClickThrottle {
            viewModel.tryApplyFilterData()
        }
    }

    final override fun subscribeToLiveData() {
        viewModel.adapterListLiveData.observe(viewLifecycleOwner) { items ->
            setAdapterData(data = items)
        }
        viewModel.scrollToErrorPositionLiveData.observe(viewLifecycleOwner) { position ->
            binding.recyclerView.post {
                launchWhenResumed {
                    try {
                        binding.recyclerView.smoothScrollToPosition(position)
                    } catch (e: Exception) {
                        logError(
                            "scrollToPosition Exception", e,
                            TAG
                        )
                    }
                }
            }
        }
        viewModel.applyFilterDataLiveData.observe(viewLifecycleOwner) { filter ->
            val bundle = Bundle().apply {
                putParcelable(FILTER_MODEL_KEY, filter)
            }
            setFragmentResult(FILTER_MODEL_REQUEST_KEY, bundle)
        }
    }

    override fun onDetached() {
        super.onDetached()
        setAdapterData(data = emptyList())
    }

    private fun setAdapterData(data: List<EmpowermentFilterAdapterMarker>) {
        logDebug("setAdapterData size: ${data.size}", TAG)
        adapter.items = data
    }

    final override fun onEraseClicked(model: EmpowermentFilterAdapterMarker) {
        logDebug("onEraseClicked", TAG)
        viewModel.onEraseClicked(model)
    }

    final override fun onButtonClicked(model: CommonButtonUi) {
        logDebug("onButtonClicked", TAG)
    }

    final override fun onEnterTextDone(model: CommonEditTextUi) {
        logDebug("onEnterTextDone text: ${model.selectedValue}", TAG)
        viewModel.onEnterTextDone(model)
    }

    final override fun onEditTextChanged(model: CommonEditTextUi) {
        logDebug("onEditTextChanged text: ${model.selectedValue}", TAG)
        viewModel.onEditTextChanged(model)
    }

    final override fun onDatePickerClicked(model: CommonDatePickerUi) {
        logDebug("onDatePickerClicked", TAG)
        showDatePicker(model)
    }

    final override fun onDialogWithSearchClicked(model: CommonDialogWithSearchUi) {
        logDebug("onDialogWithSearchClicked", TAG)
    }

    final override fun onButtonTransparentClicked(model: CommonButtonTransparentUi) {
        logDebug("onButtonTransparentClicked", TAG)
        viewModel.onButtonTransparentClicked(model)
    }

    final override fun onCheckBoxChangeState(model: CommonCheckBoxUi) {
        logDebug("onCheckBoxChangeState", TAG)
        viewModel.onCheckBoxChangeState(model)
    }

    final override fun onSpinnerClicked(model: CommonSpinnerUi, anchor: View) {
        logDebug("onSpinnerClicked", TAG)
        showSpinner(
            anchor = anchor,
            model = model,
        )
    }

    final override fun onFocusChanged(model: CommonEditTextUi) {
        logDebug("onFocusChanged hasFocus: ${model.hasFocus}", TAG)
        viewModel.onFocusChanged(model)
    }

}