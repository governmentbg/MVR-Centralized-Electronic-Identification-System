/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.applications.confirm.intro

import androidx.navigation.fragment.navArgs
import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentWithListBinding
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.ui.fragments.applications.confirm.pin.ApplicationConfirmPinBottomSheetFragment
import com.digitall.eid.ui.fragments.base.BaseFragment
import org.koin.androidx.viewmodel.ext.android.viewModel

class ApplicationConfirmIntroFragment :
    BaseFragment<FragmentWithListBinding, ApplicationConfirmIntroViewModel>(),
    ApplicationConfirmPinBottomSheetFragment.Listener {

    companion object {
        private const val TAG = "ApplicationConfirmIntroFragmentTag"
    }

    override fun getViewBinding() = FragmentWithListBinding.inflate(layoutInflater)

    override val viewModel: ApplicationConfirmIntroViewModel by viewModel()
    private val args: ApplicationConfirmIntroFragmentArgs by navArgs()

    private var applicationConfirmPinBottomSheet: ApplicationConfirmPinBottomSheetFragment? = null

    override fun parseArguments() {
        try {
            val qrCode = args.qrCode
            viewModel.setupModel(qrCode)
        } catch (e: IllegalStateException) {
            logError("parseArguments Exception: ${e.message}", e, TAG)
        }
    }

    override fun setupView() {
        binding.toolbar.setTitleText(StringSource(R.string.create_application_screen_title))
    }

    override fun setupControls() {
        binding.toolbar.navigationClickListener = {
            viewModel.onBackPressed()
        }
        binding.errorView.actionOneClickListener = {
            viewModel.refreshScreen()
        }
    }

    override fun subscribeToLiveData() {
        viewModel.showCreatePinLiveData.observe(viewLifecycleOwner) {
            showCreatePinBottomSheet()
        }
    }

    override fun operationCompleted(pin: String?) = viewModel.saveCertificate(pin = pin)

    private fun showCreatePinBottomSheet() {
        applicationConfirmPinBottomSheet =
            ApplicationConfirmPinBottomSheetFragment.newInstance(listener = this)
                .also { bottomSheet ->
                    bottomSheet.show(
                        parentFragmentManager,
                        "ApplicationConfirmPinBottomSheetFragmentTag"
                    )
                }
    }
}