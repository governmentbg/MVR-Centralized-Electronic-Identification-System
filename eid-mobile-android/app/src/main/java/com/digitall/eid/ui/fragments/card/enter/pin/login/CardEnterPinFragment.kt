package com.digitall.eid.ui.fragments.card.enter.pin.login

import android.view.View
import androidx.lifecycle.lifecycleScope
import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentCardEnterPinBinding
import com.digitall.eid.domain.DELAY_2500
import com.digitall.eid.domain.DELAY_500
import com.digitall.eid.extensions.hideKeyboard
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.extensions.launchWithDispatcher
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.models.common.CardScanBottomSheetContent
import com.digitall.eid.models.common.CardScanResult
import com.digitall.eid.models.common.CardScanScreenStates
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.ui.fragments.base.BaseFragment
import com.digitall.eid.ui.fragments.card.scan.ScanCardBottomSheetFragment
import com.digitall.eid.ui.fragments.information.InformationBottomSheetFragment
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.onEach
import kotlinx.coroutines.withContext
import org.koin.androidx.viewmodel.ext.android.viewModel

class CardEnterPinFragment :
    BaseFragment<FragmentCardEnterPinBinding, CardEnterPinViewModel>(),
    ScanCardBottomSheetFragment.Listener {

    companion object {
        private const val TAG = "CardEnterPinFragmentTag"
    }

    private var scanCardBottomSheet: ScanCardBottomSheetFragment? = null

    override fun getViewBinding() = FragmentCardEnterPinBinding.inflate(layoutInflater)

    override val viewModel: CardEnterPinViewModel by viewModel()

    override fun setupControls() {
        binding.toolbar.navigationClickListener = {
            viewModel.onBackPressed()
        }
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
        binding.btnLogin.onClickThrottle {
            viewModel.onLoginButtonClicked()
        }
        binding.toolbar.settingsClickListener = {
            showInformationBottomSheet()
        }
        binding.errorView.actionOneClickListener = {
            viewModel.onLoginButtonClicked()
        }
        binding.errorView.actionTwoClickListener = {
            viewModel.onLoginButtonClicked()
        }
    }

    override fun subscribeToLiveData() {
        viewModel.enableLoginStateLiveData.onEach { flag ->
            binding.btnLogin.isEnabled = flag
        }.launchInScope(lifecycleScope)
        viewModel.cardScanningLiveData.observe(viewLifecycleOwner) { content ->
            showScanCardBottomSheet(content = content)
        }
    }

    override fun operationCompleted(result: CardScanResult) {
        lifecycleScope.launchWithDispatcher {
            delay(DELAY_500)
            scanCardBottomSheet?.dismiss()

            when (result) {
                is CardScanResult.ChallengeSigned -> {
                    viewModel.authenticateWithCertificate(
                        signature = result.signature,
                        challenge = result.challenge,
                        certificate = result.certificate,
                        certificateChain = result.certificateChain,
                    )
                }

                else -> {}
            }
        }
    }

    override fun operationFailed(state: CardScanScreenStates) {
        lifecycleScope.launchWithDispatcher {
            delay(DELAY_2500)
            scanCardBottomSheet?.dismiss().also {
                scanCardBottomSheet = null
                withContext(Dispatchers.Main) {
                    binding.tvEnterCanDescription.visibility =
                        if (state == CardScanScreenStates.Suspended) View.VISIBLE else View.GONE
                    binding.canView.visibility =
                        if (state == CardScanScreenStates.Suspended) View.VISIBLE else View.GONE
                    viewModel.state =
                        if (state == CardScanScreenStates.Suspended) CardEnterPinViewModel.Companion.CardEnterPinType.PINCAN
                        else CardEnterPinViewModel.Companion.CardEnterPinType.PIN
                }
            }
        }
    }

    private fun showScanCardBottomSheet(content: CardScanBottomSheetContent) {
        scanCardBottomSheet =
            ScanCardBottomSheetFragment.newInstance(content = content, listener = this)
                .also { bottomSheet ->
                    bottomSheet.show(parentFragmentManager, "ScanCardBottomSheetFragmentTag")
                }
    }

    private fun showInformationBottomSheet() {
        InformationBottomSheetFragment.newInstance(content = StringSource(R.string.bottom_sheet_information_nfc))
            .also { bottomSheet ->
                bottomSheet.show(parentFragmentManager, "InformationBottomSheetFragmentTag")
            }
    }
}