package com.digitall.eid.ui.fragments.card.scan

import android.content.res.Resources
import androidx.core.os.bundleOf
import com.digitall.eid.R
import com.digitall.eid.data.extensions.getParcelableCompat
import com.digitall.eid.databinding.BottomSheetScanNfcCardBinding
import com.digitall.eid.domain.SCAN_CARD_BOTTOM_SHEET_CONTENT_KEY
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.models.common.CardScanBottomSheetContent
import com.digitall.eid.models.common.CardScanResult
import com.digitall.eid.models.common.CardScanScreenStates
import com.digitall.eid.ui.fragments.base.BaseBottomSheetFragment
import org.koin.androidx.viewmodel.ext.android.viewModel

class ScanCardBottomSheetFragment(private val listener: Listener) :
    BaseBottomSheetFragment<BottomSheetScanNfcCardBinding, ScanCardBottomSheetViewModel>() {

    companion object {
        private const val TAG = "ScanCardBottomSheetFragmentTag"

        // Factory method to create a new instance with arguments
        fun newInstance(content: CardScanBottomSheetContent, listener: Listener) =
            ScanCardBottomSheetFragment(listener).apply {
                arguments = bundleOf(SCAN_CARD_BOTTOM_SHEET_CONTENT_KEY to content)
            }
    }

    override fun getViewBinding() = BottomSheetScanNfcCardBinding.inflate(layoutInflater)

    override val viewModel: ScanCardBottomSheetViewModel by viewModel()

    override val maxHeight = Resources.getSystem().displayMetrics.heightPixels * 35 / 100

    override fun onBottomSheetDialogShown() = viewModel.enableNfcReading(activity = activity)

    override fun setupControls() {
        parseArguments()
    }

    override fun subscribeToLiveData() {
        viewModel.screenStateLiveData.observe(viewLifecycleOwner) { state ->
            when (state) {
                is CardScanScreenStates.Scanning -> {
                    binding.ltScanCard.setAnimation(R.raw.lottie_card_scan_scanning)
                    binding.tvMessage.setText(R.string.bottom_sheet_scan_nfc_card_scan_title)
                }

                is CardScanScreenStates.Processing -> {
                    binding.ltScanCard.setAnimation(R.raw.lottie_card_scan_processing)
                    binding.tvMessage.setText(R.string.bottom_sheet_scan_nfc_card_processing_title)
                }

                is CardScanScreenStates.Error -> {
                    binding.ltScanCard.setAnimation(R.raw.lottie_card_scan_error)
                    binding.tvMessage.text = state.message.getString(binding.root.context)
                    viewModel.disableNfcReading(activity).also {
                        listener.operationFailed(state)
                    }
                }

                is CardScanScreenStates.Success -> {
                    binding.ltScanCard.setAnimation(R.raw.lottie_card_scan_success)
                    binding.tvMessage.setText(R.string.bottom_sheet_scan_nfc_card_scan_succees_title)
                    listener.operationCompleted(state.result)
                }

                is CardScanScreenStates.Suspended -> {
                    binding.ltScanCard.setAnimation(R.raw.lottie_card_scan_error)
                    binding.tvMessage.setText(R.string.nfc_card_scan_suspended_error)
                    viewModel.disableNfcReading(activity).also {
                        listener.operationFailed(state)
                    }
                }

                else -> {}
            }
        }
    }

    private fun parseArguments() {
        try {
            val content = arguments?.getParcelableCompat<CardScanBottomSheetContent>(
                SCAN_CARD_BOTTOM_SHEET_CONTENT_KEY
            )
            content?.let {
                viewModel.setupContentType(
                    cardContentType = content.type
                )
            }
        } catch (exception: IllegalStateException) {
            logError("parseArguments Exception: ${exception.message}", exception, TAG)
        }
    }

    interface Listener {
        fun operationCompleted(result: CardScanResult)
        fun operationFailed(state: CardScanScreenStates)
    }

}