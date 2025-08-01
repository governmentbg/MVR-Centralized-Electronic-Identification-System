/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.common.list

import android.view.ViewGroup
import androidx.core.view.isVisible
import androidx.recyclerview.widget.RecyclerView
import com.digitall.eid.databinding.ListItemBigTitleBinding
import com.digitall.eid.extensions.inflateBinding
import com.digitall.eid.models.list.CommonListElementAdapterMarker
import com.digitall.eid.models.list.CommonTitleBigUi
import com.hannesdorfmann.adapterdelegates4.AdapterDelegate

class CommonTitleBigDelegate :
    AdapterDelegate<MutableList<CommonListElementAdapterMarker>>() {

    override fun isForViewType(
        items: MutableList<CommonListElementAdapterMarker>,
        position: Int
    ): Boolean {
        return items[position] is CommonTitleBigUi
    }

    override fun onCreateViewHolder(parent: ViewGroup): RecyclerView.ViewHolder {
        return ViewHolder(parent.inflateBinding(ListItemBigTitleBinding::inflate))
    }

    override fun onBindViewHolder(
        items: MutableList<CommonListElementAdapterMarker>,
        position: Int,
        holder: RecyclerView.ViewHolder,
        payloads: MutableList<Any>
    ) {
        (holder as ViewHolder).bind(items[position] as CommonTitleBigUi)
    }

    private inner class ViewHolder(
        private val binding: ListItemBigTitleBinding
    ) : RecyclerView.ViewHolder(binding.root) {

        fun bind(model: CommonTitleBigUi) {
            binding.tvTitle.text = model.title.getString(binding.root.context)
            binding.tvTitle.maxLines = model.maxLinesTitle
            binding.tvDescription.isVisible = model.description != null
            binding.tvDescription.text = model.description?.getString(binding.root.context)
            binding.tvDescription.maxLines = model.maxLinesDescription
            binding.rootLayout.requestFocus()
        }
    }
}