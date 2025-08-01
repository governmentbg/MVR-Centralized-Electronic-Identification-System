/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.empowerment.base.details

import androidx.annotation.CallSuper
import androidx.lifecycle.lifecycleScope
import com.digitall.eid.databinding.FragmentWithListBinding
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.extensions.enableChangeAnimations
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.models.empowerment.common.details.EmpowermentDetailsAdapterMarker
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.ui.fragments.base.BaseFragment
import com.digitall.eid.ui.fragments.empowerment.base.details.list.BaseEmpowermentDetailsAdapter
import kotlinx.coroutines.flow.onEach

abstract class BaseEmpowermentDetailsFragment<VM : BaseEmpowermentDetailsViewModel> :
    BaseFragment<FragmentWithListBinding, VM>(),
    BaseEmpowermentDetailsAdapter.ClickListener {

    companion object {
        private const val TAG = "BaseEmpowermentDetailsFragmentTag"
    }

    final override fun getViewBinding() = FragmentWithListBinding.inflate(layoutInflater)

    protected abstract val adapter: BaseEmpowermentDetailsAdapter

    @CallSuper
    override fun setupView() {
        binding.recyclerView.adapter = adapter
        binding.recyclerView.enableChangeAnimations(false)
    }

    @CallSuper
    override fun setupControls() {
        binding.refreshLayout.isGestureAllowed = false
        adapter.clickListener = this
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

    private fun setAdapterData(data: List<EmpowermentDetailsAdapterMarker>) {
        logDebug("setAdapterData size: ${data.size}", TAG)
        adapter.items = data
    }

    final override fun onButtonClicked(model: CommonButtonUi) {
        logDebug("onButtonClicked", TAG)
        viewModel.onButtonClicked(model)
    }

    final override fun onDestroyed() {
        logDebug("onDestroyFragment", TAG)
        adapter.items = emptyList()
    }

}