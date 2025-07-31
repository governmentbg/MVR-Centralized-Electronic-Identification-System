package com.digitall.eid.ui.fragments.citizen.change.information

import android.os.Bundle
import androidx.fragment.app.setFragmentResult
import androidx.navigation.fragment.navArgs
import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentWithListBinding
import com.digitall.eid.domain.REFRESH_CITIZEN_INFORMATION_KEY
import com.digitall.eid.domain.REFRESH_CITIZEN_INFORMATION_REQUEST_KEY
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.enableChangeAnimations
import com.digitall.eid.extensions.launchWhenResumed
import com.digitall.eid.models.citizen.change.information.ChangeCitizenInformationAdapterMarker
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonEditTextUi
import com.digitall.eid.models.list.CommonPhoneTextUi
import com.digitall.eid.ui.fragments.base.BaseFragment
import com.digitall.eid.ui.fragments.citizen.change.information.list.ChangeCitizenInformationAdapter
import com.digitall.eid.ui.fragments.information.InformationBottomSheetFragment
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.viewModel

class ChangeCitizenInformationFragment :
    BaseFragment<FragmentWithListBinding, ChangeCitizenInformationViewModel>(),
    ChangeCitizenInformationAdapter.ClickListener {

    companion object {
        const val TAG = "ChangeCitizenPasswordFragmentTag"
    }

    override fun getViewBinding() = FragmentWithListBinding.inflate(layoutInflater)

    override val viewModel: ChangeCitizenInformationViewModel by viewModel()

    private val adapter: ChangeCitizenInformationAdapter by inject()
    private val args: ChangeCitizenInformationFragmentArgs by navArgs()

    override fun parseArguments() {
        try {
            viewModel.setupModel(information = args.information)
        } catch (e: IllegalStateException) {
            logError("parseArguments Exception: ${e.message}", e, TAG)
        }
    }

    override fun setupView() {
        super.setupView()
        binding.toolbar.setTitleText(StringSource(R.string.change_citizen_information_screen_title))
        binding.recyclerView.adapter = adapter
        binding.recyclerView.enableChangeAnimations(false)
        binding.refreshLayout.isGestureAllowed = false
        binding.toolbar.setSettingsIcon(
            settingsIconRes = R.drawable.ic_information,
            settingsIconColorRes = R.color.color_white,
            settingsClickListener = { showInformationBottomSheet() }
        )
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
        viewModel.citizenInformationChangeLiveData.observe(viewLifecycleOwner) {
            val bundle = Bundle().apply {
                putBoolean(REFRESH_CITIZEN_INFORMATION_KEY, it)
            }
            setFragmentResult(REFRESH_CITIZEN_INFORMATION_REQUEST_KEY, bundle)
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

    private fun setAdapterData(data: List<ChangeCitizenInformationAdapterMarker>) {
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

    override fun onCharacterFilter(model: CommonEditTextUi, char: Char): Boolean {
        logDebug("onCharacterFilter text: ${model.selectedValue}", TAG)
        return viewModel.onCharacterFilter(model = model, char = char)
    }

    override fun onPhoneTextFocusChanged(model: CommonPhoneTextUi) {
        logDebug(
            "onPhoneTextFocusChanged hasFocus: ${model.hasFocus}",
            TAG
        )
        viewModel.onPhoneTextFocusChanged(model = model)
    }

    override fun onPhoneTextDone(model: CommonPhoneTextUi) {
        logDebug(
            "onPhoneTextDone text: ${model.selectedValue}",
            TAG
        )
        viewModel.onPhoneTextDone(model = model)
    }

    override fun onPhoneTextChanged(model: CommonPhoneTextUi) {
        logDebug(
            "onPhoneTextDone text: ${model.selectedValue}",
            TAG
        )
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
        InformationBottomSheetFragment.newInstance(content = StringSource(R.string.bottom_sheet_information_change_citizen_information))
            .also { bottomSheet ->
                bottomSheet.show(parentFragmentManager, null)
            }
    }

}