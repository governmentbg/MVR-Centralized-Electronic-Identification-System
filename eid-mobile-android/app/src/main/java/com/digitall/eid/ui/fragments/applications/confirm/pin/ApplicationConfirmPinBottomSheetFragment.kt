package com.digitall.eid.ui.fragments.applications.confirm.pin

import android.content.DialogInterface
import android.content.res.Resources
import androidx.core.view.isVisible
import com.digitall.eid.R
import com.digitall.eid.databinding.BottomSheetApplicationCreatePinBinding
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.models.common.CreatePinScreenStates
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.ui.fragments.base.BaseBottomSheetFragment
import org.koin.androidx.viewmodel.ext.android.viewModel

class ApplicationConfirmPinBottomSheetFragment(private val listener: Listener) :
    BaseBottomSheetFragment<BottomSheetApplicationCreatePinBinding, ApplicationConfirmPinBottomSheetViewModel>() {

    companion object {
        private const val TAG = "ApplicationConfirmPinBottomSheetFragmentTag"

        // Factory method to create a new instance with arguments
        fun newInstance(listener: Listener) = ApplicationConfirmPinBottomSheetFragment(listener)
    }

    override fun getViewBinding() = BottomSheetApplicationCreatePinBinding.inflate(layoutInflater)

    override val viewModel: ApplicationConfirmPinBottomSheetViewModel by viewModel()

    override val maxHeight = Resources.getSystem().displayMetrics.heightPixels * 90 / 100

    override fun setupControls() {
        binding.btnComplete.onClickThrottle {
            viewModel.completePinCreation()
        }
        binding.pinView.onPinStartEnter = {
            viewModel.onPinStartEnter()
        }
        binding.pinView.onPinEntered = { pin ->
            viewModel.onPinEntered(pin = pin)
        }
    }

    override fun subscribeToLiveData() {
        viewModel.screenStateLiveData.observe(viewLifecycleOwner) { state ->
            when (state) {
                CreatePinScreenStates.ENTER -> setEnterPinState()
                CreatePinScreenStates.READY -> setReadyPinState()
                CreatePinScreenStates.CONFIRM -> setConfirmPinState()
                else -> {}
            }
        }

        viewModel.errorMessageLiveData.observe(viewLifecycleOwner) { message ->
            setErrorMessage(message = message)
        }
        viewModel.clearPinLiveData.observe(viewLifecycleOwner) {
            binding.pinView.clearPin()
        }
        viewModel.pinLiveData.observe(viewLifecycleOwner) { pin ->
            listener.operationCompleted(pin = pin ?: return@observe)
        }
    }

    override fun onDismissed(dialog: DialogInterface) {
        super.onDismissed(dialog)
        viewModel.showCancellationMessage()
    }

    private fun setErrorMessage(message: StringSource?) {
        binding.tvError.text = message?.getString(binding.root.context)
        binding.tvError.isVisible = message != null
    }

    private fun setEnterPinState() {
        binding.btnComplete.isEnabled = false
        binding.tvDescription.text =
            StringSource(R.string.bottom_sheet_application_confirm_pin_description).getString(
                binding.root.context
            )
    }

    private fun setConfirmPinState() {
        binding.btnComplete.isEnabled = false
        binding.tvDescription.text =
            StringSource(R.string.bottom_sheet_application_confirm_pin_description_confirm).getString(
                binding.root.context
            )
    }

    private fun setReadyPinState() {
        binding.btnComplete.isEnabled = true
        binding.tvDescription.text =
            StringSource(R.string.bottom_sheet_application_confirm_pin_description_ready).getString(
                binding.root.context
            )
    }

    interface Listener {
        fun operationCompleted(pin: String?)
    }
}