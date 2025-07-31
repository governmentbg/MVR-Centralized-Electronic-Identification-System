package com.digitall.eid.ui.fragments.registration

import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentRegisterBinding
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.enableChangeAnimations
import com.digitall.eid.extensions.launchWhenResumed
import com.digitall.eid.extensions.setTextResource
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonEditTextUi
import com.digitall.eid.models.list.CommonPhoneTextUi
import com.digitall.eid.models.registration.RegistrationAdapterMarker
import com.digitall.eid.ui.fragments.base.BaseFragment
import com.digitall.eid.ui.fragments.information.InformationBottomSheetFragment
import com.digitall.eid.ui.fragments.registration.list.RegistrationAdapter
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.viewModel

class RegistrationFragment :
    BaseFragment<FragmentRegisterBinding, RegistrationViewModel>(),
    RegistrationAdapter.ClickListener {

    companion object {
        const val TAG = "AuthRegistrationFragmentTag"
    }

    override fun getViewBinding() = FragmentRegisterBinding.inflate(layoutInflater)

    override val viewModel: RegistrationViewModel by viewModel()

    private val adapter: RegistrationAdapter by inject()

    override fun setupView() {
        binding.tvRegistration.setTextResource(R.string.registration_title)
        binding.recyclerView.adapter = adapter
        binding.recyclerView.enableChangeAnimations(false)
    }

    override fun setupControls() {
        adapter.clickListener = this
        adapter.recyclerViewProvider = { binding.recyclerView }
        binding.toolbar.navigationClickListener = {
            viewModel.onBackPressed()
        }
        binding.toolbar.settingsClickListener = {
            showInformationBottomSheet()
        }
    }

    override fun subscribeToLiveData() {
        viewModel.adapterListLiveData.observe(viewLifecycleOwner) { items ->
            setAdapterData(data = items)
        }
        viewModel.scrollToErrorPositionLiveData.observe(viewLifecycleOwner) { position ->
            binding.recyclerView.post {
                launchWhenResumed {
                    try {
                        binding.recyclerView.smoothScrollToPosition(position)
                    } catch (e: Exception) {
                        logError("scrollToPosition Exception", e, TAG)
                    }
                }
            }
        }
    }

    override fun onDetached() {
        super.onDetached()
        setAdapterData(data = emptyList())
    }

    private fun setAdapterData(data: List<RegistrationAdapterMarker>) {
        logDebug("setAdapterData size: ${data.size}", TAG)
        adapter.items = data
    }

    override fun onEditTextFocusChanged(model: CommonEditTextUi) {
        logDebug("onEditTextFocusChanged hasFocus: ${model.hasFocus}", TAG)
        viewModel.onEditTextFocusChanged(model = model)
    }

    override fun onEditTextDone(model: CommonEditTextUi) {
        logDebug("onEditTextDone text: ${model.selectedValue}", TAG)
        viewModel.onEditTextDone(model = model)
    }

    override fun onEditTextChanged(model: CommonEditTextUi) {
        logDebug("onEditTextChanged text: ${model.selectedValue}", TAG)
        viewModel.onEditTextChanged(model = model)
    }

    override fun onCharacterFilter(model: CommonEditTextUi, char: Char): Boolean {
        logDebug("onCharacterFilter text: ${model.selectedValue}", TAG)
        return viewModel.onCharacterFilter(model = model, char = char)
    }

    override fun onPhoneTextFocusChanged(model: CommonPhoneTextUi) {
        logDebug("onPhoneTextFocusChanged hasFocus: ${model.hasFocus}", TAG)
        viewModel.onPhoneTextFocusChanged(model = model)
    }

    override fun onPhoneTextDone(model: CommonPhoneTextUi) {
        logDebug("onPhoneTextDone text: ${model.selectedValue}", TAG)
        viewModel.onPhoneTextDone(model = model)
    }

    override fun onPhoneTextChanged(model: CommonPhoneTextUi) {
        logDebug("onPhoneTextDone text: ${model.selectedValue}", TAG)
        viewModel.onPhoneTextChanged(model = model)
    }

    override fun onPhoneCharacterFilter(model: CommonPhoneTextUi, char: Char): Boolean {
        logDebug("onPhoneCharacterFilter text: ${model.selectedValue}", TAG)
        return viewModel.onPhoneCharacterFilter(model = model, char = char)
    }

    override fun onButtonClicked(model: CommonButtonUi) {
        logDebug(
            "onButtonClicked type: ${model.elementEnum}",
            TAG
        )
        viewModel.onButtonClicked(model)
    }

    private fun showInformationBottomSheet() {
        InformationBottomSheetFragment.newInstance(content = StringSource(R.string.bottom_sheet_information_registration))
            .also { bottomSheet ->
                bottomSheet.show(parentFragmentManager, null)
            }
    }

}