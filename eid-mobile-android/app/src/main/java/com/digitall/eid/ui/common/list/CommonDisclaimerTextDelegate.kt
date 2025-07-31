package com.digitall.eid.ui.common.list

import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.digitall.eid.databinding.ListItemDisclaimerTextBinding
import com.digitall.eid.extensions.inflateBinding
import com.digitall.eid.models.list.CommonDisclaimerTextUi
import com.digitall.eid.models.list.CommonListElementAdapterMarker
import com.hannesdorfmann.adapterdelegates4.AdapterDelegate

class CommonDisclaimerTextDelegate: AdapterDelegate<MutableList<CommonListElementAdapterMarker>>() {

    override fun isForViewType(
        items: MutableList<CommonListElementAdapterMarker>,
        position: Int
    ): Boolean {
        return items[position] is CommonDisclaimerTextUi
    }

    override fun onCreateViewHolder(parent: ViewGroup): RecyclerView.ViewHolder {
        return ViewHolder(parent.inflateBinding(ListItemDisclaimerTextBinding::inflate))
    }

    override fun onBindViewHolder(
        items: MutableList<CommonListElementAdapterMarker>,
        position: Int,
        holder: RecyclerView.ViewHolder,
        payloads: MutableList<Any>
    ) {
        (holder as CommonDisclaimerTextDelegate.ViewHolder).bind(items[position] as CommonDisclaimerTextUi)
    }

    private inner class ViewHolder(
        private val binding: ListItemDisclaimerTextBinding
    ) : RecyclerView.ViewHolder(binding.root) {

        fun bind(model: CommonDisclaimerTextUi) {
            binding.tvValue.text = model.text.getString(binding.root.context)
        }

    }
}