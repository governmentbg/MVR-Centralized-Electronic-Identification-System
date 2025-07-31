/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.common.list

import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.digitall.eid.databinding.ListItemSeparatorBinding
import com.digitall.eid.extensions.inflateBinding
import com.digitall.eid.models.list.CommonListElementAdapterMarker
import com.digitall.eid.models.list.CommonSeparatorUi
import com.hannesdorfmann.adapterdelegates4.AdapterDelegate

class CommonSeparatorDelegate :
    AdapterDelegate<MutableList<CommonListElementAdapterMarker>>() {

    override fun isForViewType(
        items: MutableList<CommonListElementAdapterMarker>,
        position: Int
    ): Boolean {
        return items[position] is CommonSeparatorUi
    }

    override fun onCreateViewHolder(parent: ViewGroup): RecyclerView.ViewHolder {
        return ViewHolder(parent.inflateBinding(ListItemSeparatorBinding::inflate))
    }

    override fun onBindViewHolder(
        items: MutableList<CommonListElementAdapterMarker>,
        position: Int,
        holder: RecyclerView.ViewHolder,
        payloads: MutableList<Any>
    ) {
        (holder as ViewHolder).bind(items[position] as CommonSeparatorUi)
    }

    private inner class ViewHolder(
        private val binding: ListItemSeparatorBinding
    ) : RecyclerView.ViewHolder(binding.root) {

        fun bind(model: CommonSeparatorUi) {
            val layoutParams = binding.rootLayout.layoutParams as RecyclerView.LayoutParams
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