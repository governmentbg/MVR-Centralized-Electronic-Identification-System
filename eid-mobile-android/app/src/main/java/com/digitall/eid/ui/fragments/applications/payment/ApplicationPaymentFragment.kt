package com.digitall.eid.ui.fragments.applications.payment

import androidx.navigation.fragment.navArgs
import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentApplicationPaymentBinding
import com.digitall.eid.domain.ENVIRONMENT
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.extensions.openUrlInBrowser
import com.digitall.eid.extensions.setTextSource
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.ui.fragments.base.BaseFragment
import org.koin.androidx.viewmodel.ext.android.viewModel
import java.util.Locale

class ApplicationPaymentFragment :
    BaseFragment<FragmentApplicationPaymentBinding, ApplicationPaymentViewModel>() {

    companion object {
        private const val TAG = "ApplicationCreatePaymentFragmentTag"
    }

    override fun getViewBinding() = FragmentApplicationPaymentBinding.inflate(layoutInflater)

    override val viewModel: ApplicationPaymentViewModel by viewModel()
    private val args: ApplicationPaymentFragmentArgs by navArgs()

    override fun parseArguments() {
        try {
            val notNullFees = args.model.fee.filterNotNull()
            val notNullCurrencies = args.model.currency.filterNotNull()

            val feeText = when (notNullFees.size) {
                1 -> StringSource(String.format(Locale.getDefault(), "%.2f", notNullFees.first()))
                2 -> StringSource(
                    String.format(
                        Locale.getDefault(),
                        "%.2f | %.2f",
                        notNullFees.first(),
                        notNullFees.last()
                    )
                )

                else -> StringSource(R.string.unknown)
            }

            val feeCurrency = when (notNullCurrencies.size) {
                1 -> StringSource(
                    String.format(
                        Locale.getDefault(),
                        "%s",
                        notNullCurrencies.first()
                    )
                )

                2 -> StringSource(
                    String.format(
                        Locale.getDefault(),
                        "%s | %s",
                        notNullCurrencies.first(),
                        notNullCurrencies.last()
                    )
                )

                else -> StringSource(R.string.unknown)
            }

            binding.tvReceiptPriceValue.setTextSource(feeText)
            binding.tvReceiptCarrierPriceValue.text =
                String.format(Locale.getDefault(), "%d", args.model.carrierPrice)
            binding.tvReceiptCurrencyValue.setTextSource(feeCurrency)
            viewModel.setupModel(paymentAccessCode = args.model.paymentAccessCode)
        } catch (e: IllegalStateException) {
            logError("parseArguments Exception: ${e.message}", e, TAG)
        }
    }

    override fun setupControls() {
        binding.btnClose.onClickThrottle {
            viewModel.onBackPressed()
        }
        binding.btnPayment.onClickThrottle {
            viewModel.openPayment()
        }
    }

    override fun subscribeToLiveData() {
        viewModel.openPaymentEvent.observe(viewLifecycleOwner) { paymentAccessCode ->
            val basePaymentUrl = ENVIRONMENT.urlPayment
            val paymentUrl =
                paymentAccessCode?.let { "$basePaymentUrl?code=$it" } ?: run { basePaymentUrl }
            context?.openUrlInBrowser(url = paymentUrl)
        }
    }

}