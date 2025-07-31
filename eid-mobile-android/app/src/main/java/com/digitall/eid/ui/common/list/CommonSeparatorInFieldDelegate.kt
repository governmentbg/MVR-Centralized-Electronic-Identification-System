package com.digitall.eid.ui.common.list

import android.view.ViewGroup
import androidx.appcompat.widget.LinearLayoutCompat
import androidx.recyclerview.widget.RecyclerView
import com.digitall.eid.databinding.ListItemSeparatorInFieldBinding
import com.digitall.eid.extensions.inflateBinding
import com.digitall.eid.models.list.CommonListElementAdapterMarker
import com.digitall.eid.models.list.CommonSeparatorInFieldUi
import com.hannesdorfmann.adapterdelegates4.AdapterDelegate

class CommonSeparatorInFieldDelegate:
    AdapterDelegate<MutableList<CommonListElementAdapterMarker>>() {

    override fun isForViewType(
        items: MutableList<CommonListElementAdapterMarker>,
        position: Int
    ): Boolean {
        return items[position] is CommonSeparatorInFieldUi
    }

    override fun onCreateViewHolder(parent: ViewGroup): RecyclerView.ViewHolder {
        return ViewHolder(parent.inflateBinding(ListItemSeparatorInFieldBinding::inflate))
    }

    override fun onBindViewHolder(
        items: MutableList<CommonListElementAdapterMarker>,
        position: Int,
        holder: RecyclerView.ViewHolder,
        payloads: MutableList<Any>
    ) {
        (holder as ViewHolder).bind(items[position] as CommonSeparatorInFieldUi)
    }

    private inner class ViewHolder(
        private val binding: ListItemSeparatorInFieldBinding
    ) : RecyclerView.ViewHolder(binding.root) {

        fun bind(model: CommonSeparatorInFieldUi) {
            val layoutParams = binding.separator.layoutParams as LinearLayoutCompat.LayoutParams
            layoutParams.setMargins(
                model.marginLeft,
                model.marginTop,
                model.marginRight,
                model.marginBottom
            )
            binding.rootLayout.layoutParams = layoutParams
            binding.rootLayout.requestFocus()
        }
    }
}