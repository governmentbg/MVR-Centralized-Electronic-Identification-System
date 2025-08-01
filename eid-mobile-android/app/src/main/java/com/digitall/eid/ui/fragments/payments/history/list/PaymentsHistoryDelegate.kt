package com.digitall.eid.ui.fragments.payments.history.list

import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.digitall.eid.databinding.ListItemPaymentHistoryBinding
import com.digitall.eid.domain.UiDateFormats
import com.digitall.eid.domain.extensions.fromServerDateToTextDate
import com.digitall.eid.extensions.inflateBinding
import com.digitall.eid.extensions.setTextSource
import com.digitall.eid.extensions.setTextWithIcon
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.payments.history.all.PaymentHistoryUi
import com.hannesdorfmann.adapterdelegates4.AbsFallbackAdapterDelegate

class PaymentsHistoryDelegate :
    AbsFallbackAdapterDelegate<MutableList<PaymentHistoryUi>>() {

    companion object {
        private const val TAG = "PaymentsHistoryDelegateTag"
    }

    override fun onCreateViewHolder(parent: ViewGroup): RecyclerView.ViewHolder {
        return ViewHolder(parent.inflateBinding(ListItemPaymentHistoryBinding::inflate))
    }

    override fun onBindViewHolder(
        items: MutableList<PaymentHistoryUi>,
        position: Int,
        holder: RecyclerView.ViewHolder,
        payloads: MutableList<Any>
    ) {
        (holder as ViewHolder).bind(items[position])
    }

    private inner class ViewHolder(
        private val binding: ListItemPaymentHistoryBinding
    ) : RecyclerView.ViewHolder(binding.root) {

        fun bind(model: PaymentHistoryUi) {
            binding.tvPaymentNumber.text = model.ePaymentId
            binding.tvCreatedOn.text =
                model.createdOn.fromServerDateToTextDate(dateFormat = UiDateFormats.WITHOUT_TIME)
            binding.tvSubject.text = model.reason.title.getString(binding.root.context)
            binding.tvPaymentDate.text =
                model.paymentDate.fromServerDateToTextDate(dateFormat = UiDateFormats.WITH_TIME)
            binding.tvValidUntil.text =
                model.paymentDeadline.fromServerDateToTextDate(dateFormat = UiDateFormats.WITH_TIME)
            val amount = model.amount.toString()
            val currency = model.currency
            binding.tvStatus.setTextWithIcon(
                textStringSource = model.status.title,
                textColorRes = model.status.colorRes,
                iconResLeft = model.status.iconRes
            )
            binding.tvAmount.setTextSource(StringSource("$amount $currency"))
            binding.tvLastUpdated.text =
                model.lastSync.fromServerDateToTextDate(dateFormat = UiDateFormats.WITH_TIME)
        }
    }
}