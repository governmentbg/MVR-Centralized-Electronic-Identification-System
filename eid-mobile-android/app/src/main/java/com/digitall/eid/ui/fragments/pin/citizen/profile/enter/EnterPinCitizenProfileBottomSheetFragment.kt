package com.digitall.eid.ui.fragments.pin.citizen.profile.enter

import android.content.res.Resources
import androidx.core.view.isVisible
import com.digitall.eid.databinding.BottomSheetEnterPinCitizenProfileBinding
import com.digitall.eid.domain.models.common.ApplicationCredentials
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.models.common.AuthenticationResult
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.ui.fragments.base.BaseBottomSheetFragment
import org.koin.androidx.viewmodel.ext.android.viewModel

class EnterPinCitizenProfileBottomSheetFragment(private val listener: Listener) :
    BaseBottomSheetFragment<BottomSheetEnterPinCitizenProfileBinding, EnterPinCitizenProfileBottomSheetViewModel>() {

    companion object {
        private const val TAG = "EnterPinCitizenProfileBottomSheetFragmentTag"

        // Factory method to create a new instance with arguments
        fun newInstance(listener: Listener) = EnterPinCitizenProfileBottomSheetFragment(listener)
    }

    override fun getViewBinding() =
        BottomSheetEnterPinCitizenProfileBinding.inflate(layoutInflater)

    override val viewModel: EnterPinCitizenProfileBottomSheetViewModel by viewModel()

    override val maxHeight = Resources.getSystem().displayMetrics.heightPixels * 75 / 100

    override fun setupControls() {
        binding.btnComplete.onClickThrottle {
            viewModel.verifyPin()
        }
        binding.pinView.onPinEntered = { pin ->
            viewModel.onPinEntered(pin = pin)
        }
    }

    override fun subscribeToLiveData() {
        viewModel.authenticationResultEvent.observe(viewLifecycleOwner) { result ->
            when (result) {
                is AuthenticationResult.Success ->
                    listener.operationCompleted(result.credentials)

                is AuthenticationResult.Failure -> {
                    binding.pinView.clearPin()
                    binding.btnComplete.isEnabled = false
                    setErrorMessage(result.message)
                }

                is AuthenticationResult.FallbackToPassword -> listener.operationFailed()

                else -> {}
            }
        }
        viewModel.readyStateEvent.observe(viewLifecycleOwner) {
            binding.btnComplete.isEnabled = true
        }
    }

    private fun setErrorMessage(message: StringSource?) {
        binding.tvError.text = message?.getString(binding.root.context)
        binding.tvError.isVisible = message != null
    }

    interface Listener {
        fun operationCompleted(credentials: ApplicationCredentials)
        fun operationFailed()
    }
}