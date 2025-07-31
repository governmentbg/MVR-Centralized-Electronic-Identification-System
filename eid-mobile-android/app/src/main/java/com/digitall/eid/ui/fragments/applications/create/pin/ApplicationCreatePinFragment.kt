/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.applications.create.pin

import androidx.core.view.isVisible
import androidx.navigation.fragment.navArgs
import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentApplicationCreatePinBinding
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.ui.fragments.base.pin.BaseCreatePinFragment
import org.koin.androidx.viewmodel.ext.android.viewModel

class ApplicationCreatePinFragment :
    BaseCreatePinFragment<FragmentApplicationCreatePinBinding, ApplicationCreatePinViewModel>() {

    companion object {
        private const val TAG = "ApplicationCreatePinFragmentTag"
    }

    override fun getViewBinding() = FragmentApplicationCreatePinBinding.inflate(layoutInflater)

    override val viewModel: ApplicationCreatePinViewModel by viewModel()

    private val args: ApplicationCreatePinFragmentArgs by navArgs()

    override fun setupControls() {
        binding.toolbar.navigationClickListener = {
            viewModel.onBackPressed()
        }
        binding.btnCreate.onClickThrottle {
            viewModel.onCreateClicked()
        }
        binding.pinView.onPinStartEnter = {
            binding.btnCreate.isEnabled = false
            viewModel.onPinStartEnter()
        }
        binding.pinView.onPinEntered = {
            viewModel.onPinEntered(it)
        }
    }

    override fun setEnterPinState() {
        binding.btnCreate.isEnabled = false
        binding.tvDescription.text = StringSource(R.string.create_application_create_pin_title).getString(binding.root.context)
    }

    override fun setConfirmPinState() {
        binding.btnCreate.isEnabled = false
        binding.tvDescription.text = StringSource(R.string.create_application_confirm_pin_title).getString(binding.root.context)
    }

    override fun setReadyPinState() {
        binding.btnCreate.isEnabled = true
        binding.tvDescription.text = StringSource(R.string.create_application_finish_pin_title).getString(binding.root.context)
    }

    override fun clearPin() {
        binding.pinView.clearPin()
    }

    override fun setPinErrorText(error: StringSource?) {
        binding.tvError.text = error?.getString(requireContext()) ?: ""
        binding.tvError.isVisible = error != null
    }

    override fun parseArguments() {
        logDebug("parseArguments", TAG)
        try {
            viewModel.setupModel(args.model)
        } catch (e: IllegalStateException) {
            logError("parseArguments Exception: ${e.message}", e, TAG)
            viewModel.setupModel(null)
        }
    }

}