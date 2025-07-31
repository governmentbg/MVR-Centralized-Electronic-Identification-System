package com.digitall.eid.ui.fragments.certificates.change.pin

import androidx.lifecycle.lifecycleScope
import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentWithListBinding
import com.digitall.eid.domain.DELAY_1000
import com.digitall.eid.domain.DELAY_2500
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.extensions.enableChangeAnimations
import com.digitall.eid.extensions.launchWithDispatcher
import com.digitall.eid.models.card.change.pin.CertificateChangePinAdapterMarker
import com.digitall.eid.models.common.CardScanBottomSheetContent
import com.digitall.eid.models.common.CardScanResult
import com.digitall.eid.models.common.CardScanScreenStates
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonEditTextUi
import com.digitall.eid.ui.fragments.base.BaseFragment
import com.digitall.eid.ui.fragments.card.scan.ScanCardBottomSheetFragment
import com.digitall.eid.ui.fragments.certificates.change.pin.list.CertificateChangePinAdapter
import kotlinx.coroutines.delay
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.viewModel

class CertificateChangePinFragment :
    BaseFragment<FragmentWithListBinding, CertificateChangePinViewModel>(),
    CertificateChangePinAdapter.ClickListener,
    ScanCardBottomSheetFragment.Listener {

    companion object {
        private const val TAG = "CardChangePinFragmentTag"
    }

    override fun getViewBinding() =
        FragmentWithListBinding.inflate(layoutInflater)

    override val viewModel: CertificateChangePinViewModel by viewModel()

    private val adapter: CertificateChangePinAdapter by inject()

    private var scanCardBottomSheet: ScanCardBottomSheetFragment? = null

    override fun setupView() {
        binding.toolbar.setTitleText(StringSource(R.string.change_certificate_pin_title))
        binding.recyclerView.adapter = adapter
        binding.recyclerView.enableChangeAnimations(false)
    }

    override fun setupControls() {
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

        viewModel.cardScanningLiveData.observe(viewLifecycleOwner) {
            showScanCardBottomSheet(it)
        }

        viewModel.changeLocalCertificatePinLiveData.observe(viewLifecycleOwner) {
            viewModel.successfulPinChange()
        }
    }

    override fun onDetached() {
        super.onDetached()
        setAdapterData(data = emptyList())
    }

    private fun setAdapterData(data: List<CertificateChangePinAdapterMarker>) {
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

    override fun onButtonClicked(model: CommonButtonUi) {
        logDebug("onButtonClicked type: ${model.elementEnum}", TAG)
        viewModel.onButtonClicked(model)
    }

    override fun operationCompleted(result: CardScanResult) {
        lifecycleScope.launchWithDispatcher {
            delay(DELAY_1000)
            scanCardBottomSheet?.dismiss().also {
                viewModel.successfulPinChange()
            }
        }
    }

    override fun operationFailed(state: CardScanScreenStates) {
        lifecycleScope.launchWithDispatcher {
            delay(DELAY_2500)
            scanCardBottomSheet?.dismiss().also {
                scanCardBottomSheet = null
                viewModel.state =
                    if (state == CardScanScreenStates.Suspended)
                        CertificateChangePinViewModel.Companion.CertificateChangePinType.PIN_CAN
                    else CertificateChangePinViewModel.Companion.CertificateChangePinType.PIN
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

}