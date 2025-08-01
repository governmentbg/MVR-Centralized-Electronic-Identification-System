package com.digitall.eid.ui.fragments.main.tabs.requests.list

import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.digitall.eid.R
import com.digitall.eid.databinding.ListItemPendingRequestBinding
import com.digitall.eid.domain.UiDateFormats
import com.digitall.eid.domain.extensions.fromServerDateToTextDate
import com.digitall.eid.extensions.inflateBinding
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.extensions.setTextSource
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.requests.RequestUi
import com.hannesdorfmann.adapterdelegates4.AbsFallbackAdapterDelegate

class TabRequestsDelegate : AbsFallbackAdapterDelegate<MutableList<RequestUi>>() {

    companion object {
        private const val TAG = "TabRequestsDelegateTag"
    }

    var acceptClickListener: ((model: RequestUi) -> Unit)? = null
    var declineClickListener: ((model: RequestUi) -> Unit)? = null

    override fun onCreateViewHolder(parent: ViewGroup): RecyclerView.ViewHolder {
        return ViewHolder(parent.inflateBinding(ListItemPendingRequestBinding::inflate))
    }

    override fun onBindViewHolder(
        items: MutableList<RequestUi>,
        position: Int,
        holder: RecyclerView.ViewHolder,
        payloads: MutableList<Any>
    ) {
        (holder as ViewHolder).bind(items[position])
    }

    private inner class ViewHolder(
        private val binding: ListItemPendingRequestBinding
    ) : RecyclerView.ViewHolder(binding.root) {

        fun bind(model: RequestUi) {
            binding.tvTitle.setTextSource(StringSource(model.requestFrom?.type))
            binding.tvDescription.setTextSource(StringSource(model.requestFrom?.system))
            binding.tvCreateDate.setTextSource(
                StringSource(
                    model.createDate?.fromServerDateToTextDate(dateFormat = UiDateFormats.WITH_TIME)
                )
            )
            binding.btnAccept.setTextSource(StringSource(R.string.tab_requests_element_accept_button_title))
            binding.btnAccept.onClickThrottle {
                acceptClickListener?.invoke(model)
            }
            binding.btnDecline.setTextSource(StringSource(R.string.tab_requests_element_decline_button_title))
            binding.btnDecline.onClickThrottle {
                declineClickListener?.invoke(model)
            }
        }
    }
}