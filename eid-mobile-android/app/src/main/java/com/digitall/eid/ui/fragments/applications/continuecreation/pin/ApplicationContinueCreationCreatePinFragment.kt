/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.applications.continuecreation.pin

import android.os.Bundle
import androidx.core.view.isVisible
import androidx.navigation.fragment.navArgs
import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentApplicationCreatePinBinding
import com.digitall.eid.domain.REFRESH_APPLICATIONS_KEY
import com.digitall.eid.domain.REFRESH_APPLICATIONS_REQUEST_KEY
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.findParentFragmentResultListenerFragmentManager
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.ui.fragments.base.pin.BaseCreatePinFragment
import org.koin.androidx.viewmodel.ext.android.viewModel

class ApplicationContinueCreationCreatePinFragment :
    BaseCreatePinFragment<FragmentApplicationCreatePinBinding, ApplicationContinueCreationCreatePinViewModel>() {

    companion object {
        private const val TAG = "ApplicationContinueCreationCreatePinFragmentTag"
        private const val NAVIGATE_TO_APPLICATIONS_REQUEST_KEY =
            "NAVIGATE_TO_APPLICATIONS_REQUEST_KEY"
        private const val NAVIGATE_TO_APPLICATIONS_KEY = "NAVIGATE_TO_APPLICATIONS_KEY"
    }

    override fun getViewBinding() = FragmentApplicationCreatePinBinding.inflate(layoutInflater)

    override val viewModel: ApplicationContinueCreationCreatePinViewModel by viewModel()

    private val args: ApplicationContinueCreationCreatePinFragmentArgs by navArgs()

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

    override fun subscribeToLiveData() {
        super.subscribeToLiveData()
        viewModel.creationCompletedEventLiveData.observe(viewLifecycleOwner) {
            val bundle = Bundle().apply {
                putBoolean(NAVIGATE_TO_APPLICATIONS_KEY, true)
                putBoolean(REFRESH_APPLICATIONS_KEY, true)
            }
            findParentFragmentResultListenerFragmentManager()?.setFragmentResult(
                NAVIGATE_TO_APPLICATIONS_REQUEST_KEY,
                bundle
            )
            findParentFragmentResultListenerFragmentManager()?.setFragmentResult(
                REFRESH_APPLICATIONS_REQUEST_KEY,
                bundle
            )
        }
    }

    override fun setEnterPinState() {
        binding.btnCreate.isEnabled = false
        binding.tvDescription.text =
            StringSource(R.string.application_create_pin_description).getString(
                binding.root.context
            )
    }

    override fun setConfirmPinState() {
        binding.btnCreate.isEnabled = false
        binding.tvDescription.text =
            StringSource(R.string.application_create_pin_description_confirm).getString(
                binding.root.context
            )
    }

    override fun setReadyPinState() {
        binding.btnCreate.isEnabled = true
        binding.tvDescription.text =
            StringSource(R.string.application_create_pin_description_ready).getString(
                binding.root.context
            )
    }

    override fun clearPin() {
        binding.pinView.clearPin()
    }

    override fun setPinErrorText(error: StringSource?) {
        binding.tvError.text = error?.getString(binding.root.context)
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