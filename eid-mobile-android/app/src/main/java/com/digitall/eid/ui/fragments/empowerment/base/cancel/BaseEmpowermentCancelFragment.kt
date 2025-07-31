/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.empowerment.base.cancel

import android.view.View
import androidx.annotation.CallSuper
import androidx.lifecycle.lifecycleScope
import com.digitall.eid.databinding.FragmentWithListBinding
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.enableChangeAnimations
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.extensions.launchWhenResumed
import com.digitall.eid.models.common.BannerMessage
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.empowerment.common.cancel.EmpowermentCancelAdapterMarker
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonEditTextUi
import com.digitall.eid.models.list.CommonSpinnerUi
import com.digitall.eid.ui.fragments.base.BaseFragment
import com.digitall.eid.ui.fragments.empowerment.base.cancel.list.BaseEmpowermentCancelAdapter
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.onEach

abstract class BaseEmpowermentCancelFragment<VM : BaseEmpowermentCancelViewModel> :
    BaseFragment<FragmentWithListBinding, VM>(),
    BaseEmpowermentCancelAdapter.ClickListener {

    companion object {
        private const val TAG = "BaseEmpowermentCancelFragmentTag"
    }

    final override fun getViewBinding() = FragmentWithListBinding.inflate(layoutInflater)
    abstract val adapter: BaseEmpowermentCancelAdapter

    abstract val toolbarTitleText: StringSource

    override fun setupView() {
        binding.recyclerView.adapter = adapter
        binding.recyclerView.enableChangeAnimations(false)
        binding.toolbar.setTitleText(toolbarTitleText)
        adapter.clickListener = this
    }

    @CallSuper
    override fun setupControls() {
        binding.refreshLayout.isGestureAllowed = false
        binding.toolbar.navigationClickListener = {
            viewModel.onBackPressed()
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
    }

    @CallSuper
    override fun subscribeToLiveData() {
        viewModel.adapterListLiveData.onEach {
            setAdapterData(it)
        }.launchInScope(lifecycleScope)
    }

    private fun setAdapterData(data: List<EmpowermentCancelAdapterMarker>) {
        logDebug("setAdapterData size: ${data.size}", TAG)
        adapter.items = data
        binding.recyclerView.post {
            launchWhenResumed {
                delay(100)
                try {
                    logDebug("scrollToPosition 0", TAG)
                    binding.recyclerView.scrollToPosition(0)
                } catch (e: Exception) {
                    logError("scrollToPosition Exception", e, TAG)
                }
            }
        }
    }

    override fun onSpinnerClicked(model: CommonSpinnerUi, anchor: View) {
        logDebug("onSpinnerClicked", TAG)
        if (model.list.isEmpty()) {
            logError("onListElementChangeState list is empty", TAG)
            showMessage(BannerMessage.error(StringSource("No elements")))
            return
        }
        showSpinner(
            anchor = anchor,
            model = model,
        )
    }

    override fun onButtonClicked(model: CommonButtonUi) {
        logDebug("onButtonClicked", TAG)
        viewModel.onButtonClicked(model)
    }

    final override fun onEnterTextDone(model: CommonEditTextUi) {
        logDebug("onEnterTextDone text: ${model.selectedValue}", TAG)
        viewModel.onEnterTextDone(model)
    }

    final override fun onEditTextChanged(model: CommonEditTextUi) {
        logDebug("onEditTextChanged text: ${model.selectedValue}", TAG)
        viewModel.onEditTextChanged(model)
    }

    final override fun onFocusChanged(model: CommonEditTextUi) {
        logDebug("onFocusChanged hasFocus: ${model.hasFocus}", TAG)
        viewModel.onFocusChanged(model)
    }

}