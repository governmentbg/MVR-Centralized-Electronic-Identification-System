package com.digitall.eid.ui.fragments.citizen.change.email

import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentWithListBinding
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.extensions.enableChangeAnimations
import com.digitall.eid.models.citizen.change.email.ChangeCitizenEmailAdapterMarker
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonEditTextUi
import com.digitall.eid.ui.fragments.base.BaseFragment
import com.digitall.eid.ui.fragments.citizen.change.email.list.ChangeCitizenEmailAdapter
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.viewModel

class ChangeCitizenEmailFragment : BaseFragment<FragmentWithListBinding, ChangeCitizenEmailViewModel>(),
    ChangeCitizenEmailAdapter.ClickListener {

    companion object {
        const val TAG = "ChangeCitizenEmailFragmentTag"
    }

    override fun getViewBinding() = FragmentWithListBinding.inflate(layoutInflater)

    override val viewModel: ChangeCitizenEmailViewModel by viewModel()

    private val adapter: ChangeCitizenEmailAdapter by inject()

    override fun setupView() {
        super.setupView()
        binding.toolbar.setTitleText(StringSource(R.string.change_user_email_title))
        binding.recyclerView.adapter = adapter
        binding.recyclerView.enableChangeAnimations(false)
        binding.refreshLayout.isGestureAllowed = false
    }

    override fun setupControls() {
        super.setupControls()
        adapter.clickListener = this
        adapter.recyclerViewProvider = { binding.recyclerView }
        binding.toolbar.navigationClickListener = {
            viewModel.onBackPressed()
        }
    }

    override fun subscribeToLiveData() {
        viewModel.adapterListLiveData.observe(viewLifecycleOwner) { items ->
            setAdapterData(data = items)
        }
    }

    override fun onDetached() {
        super.onDetached()
        setAdapterData(data = emptyList())
    }

    private fun setAdapterData(data: List<ChangeCitizenEmailAdapterMarker>) {
        logDebug("setAdapterData size: ${data.size}", TAG)
        adapter.items = data
    }

    override fun onEditTextFocusChanged(model: CommonEditTextUi) {
        logDebug(
            "onEditTextFocusChanged hasFocus: ${model.hasFocus}",
            TAG
        )
        viewModel.onEditTextFocusChanged(model = model)
    }

    override fun onEditTextDone(model: CommonEditTextUi) {
        logDebug(
            "onEditTextDone text: ${model.selectedValue}",
            TAG
        )
        viewModel.onEditTextDone(model = model)
    }

    override fun onEditTextChanged(model: CommonEditTextUi) {
        logDebug(
            "onEditTextChanged text: ${model.selectedValue}",
            TAG
        )
        viewModel.onEditTextChanged(model = model)
    }

    override fun onButtonClicked(model: CommonButtonUi) {
        logDebug(
            "onButtonClicked type: ${model.elementEnum}",
            TAG
        )
        viewModel.onButtonClicked(model)
    }
}