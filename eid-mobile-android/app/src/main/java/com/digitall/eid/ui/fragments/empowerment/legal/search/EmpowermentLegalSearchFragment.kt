package com.digitall.eid.ui.fragments.empowerment.legal.search

import androidx.lifecycle.lifecycleScope
import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentWithListBinding
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.extensions.enableChangeAnimations
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.empowerment.legal.search.EmpowermentLegalSearchAdapterMarker
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonEditTextUi
import com.digitall.eid.ui.fragments.base.BaseFragment
import com.digitall.eid.ui.fragments.empowerment.legal.search.list.EmpowermentLegalSearchAdapter
import kotlinx.coroutines.flow.onEach
import org.koin.android.ext.android.inject

class EmpowermentLegalSearchFragment :
    BaseFragment<FragmentWithListBinding, EmpowermentLegalSearchViewModel>(),
    EmpowermentLegalSearchAdapter.ClickListener {

    override val viewModel: EmpowermentLegalSearchViewModel by inject()

    private val adapter: EmpowermentLegalSearchAdapter by inject()

    override fun getViewBinding() = FragmentWithListBinding.inflate(layoutInflater)


    companion object {
        private const val TAG = "EmpowermentLegalSearchFragmentTag"
    }


    override fun setupView() {
        binding.toolbar.setTitleText(StringSource(R.string.empowerments_inquiry_legal_entity_search_title))
        binding.recyclerView.adapter = adapter
        binding.recyclerView.enableChangeAnimations(false)
    }

    override fun setupControls() {
        binding.refreshLayout.isGestureAllowed = false
        adapter.clickListener = this
        binding.toolbar.navigationClickListener = {
            viewModel.onBackPressed()
        }
    }

    override fun subscribeToLiveData() {
        viewModel.adapterListLiveData.onEach {
            setAdapterData(it)
        }.launchInScope(lifecycleScope)
    }

    private fun setAdapterData(data: List<EmpowermentLegalSearchAdapterMarker>) {
        logDebug("setAdapterData size: ${data.size}", TAG)
        adapter.items = data
    }

    override fun onButtonClicked(model: CommonButtonUi) {
        logDebug("onButtonClicked", TAG)
        viewModel.onButtonClicked(model)
    }

    override fun onEditTextFocusChanged(model: CommonEditTextUi) {
        logDebug("onEditTextFocusChanged", TAG)
        viewModel.onEditTextFocusChanged(model)
    }

    override fun onEditTextDone(model: CommonEditTextUi) {
        logDebug("onEditTextDone", TAG)
        viewModel.onEditTextDone(model)
    }

    override fun onEditTextChanged(model: CommonEditTextUi) {
        logDebug("onEditTextChanged", TAG)
        viewModel.onEditTextChanged(model)
    }
}