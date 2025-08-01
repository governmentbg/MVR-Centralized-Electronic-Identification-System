package com.digitall.eid.ui.fragments.card.enter.pin.auth

import android.content.res.Resources
import android.view.View
import androidx.lifecycle.lifecycleScope
import com.digitall.eid.databinding.BottomSheetEnterCardPinBinding
import com.digitall.eid.extensions.hideKeyboard
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.models.common.CardScanBottomSheetContent
import com.digitall.eid.ui.fragments.base.BaseBottomSheetFragment
import kotlinx.coroutines.flow.onEach
import org.koin.androidx.viewmodel.ext.android.viewModel

class AuthCardBottomSheetFragment(
    private val listener: Listener,
    private var shouldShowCan: Boolean
) :
    BaseBottomSheetFragment<BottomSheetEnterCardPinBinding, AuthCardBottomSheetViewModel>() {

    companion object {
        private const val TAG = "ScanCardBottomSheetFragmentTag"

        // Factory method to create a new instance with arguments
        fun newInstance(listener: Listener, shouldShowCan: Boolean = false) =
            AuthCardBottomSheetFragment(listener, shouldShowCan)
    }

    override fun getViewBinding() = BottomSheetEnterCardPinBinding.inflate(layoutInflater)

    override val viewModel: AuthCardBottomSheetViewModel by viewModel()

    override val maxHeight = Resources.getSystem().displayMetrics.heightPixels * 70 / 100

    override fun setupView() {
        super.setupView()
        binding.tvEnterCanDescription.visibility =
            if (shouldShowCan) View.VISIBLE else View.GONE
        binding.canView.visibility =
            if (shouldShowCan) View.VISIBLE else View.GONE
        viewModel.state =
            if (shouldShowCan) AuthCardBottomSheetViewModel.Companion.AuthCardType.PINCAN
            else AuthCardBottomSheetViewModel.Companion.AuthCardType.PIN
    }

    override fun setupControls() {
        binding.pinView.onPinEntered = { pin ->
            hideKeyboard()
            viewModel.onPinEntered(pin = pin)
        }
        binding.pinView.onPinCleared = {
            viewModel.onPinCleared()
        }
        binding.canView.onPinEntered = { can ->
            hideKeyboard()
            viewModel.onCanEntered(can = can)
        }
        binding.canView.onPinCleared = {
            viewModel.onPinCleared()
        }
        binding.btnAuthenticate.onClickThrottle {
            viewModel.onAuthenticateButtonClicked()
        }
    }

    override fun subscribeToLiveData() {
        viewModel.enableAuthenticateStateLiveData.onEach { flag ->
            binding.btnAuthenticate.isEnabled = flag
        }.launchInScope(lifecycleScope)
        viewModel.cardScanningLiveData.observe(viewLifecycleOwner) { content ->
            listener.operationCompleted(result = content)
        }
    }

    interface Listener {
        fun operationCompleted(result: CardScanBottomSheetContent)
    }

}