/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.common.list

import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.digitall.eid.databinding.ListItemSmallTitleBinding
import com.digitall.eid.extensions.inflateBinding
import com.digitall.eid.models.list.CommonListElementAdapterMarker
import com.digitall.eid.models.list.CommonTitleSmallUi
import com.hannesdorfmann.adapterdelegates4.AdapterDelegate

class CommonTitleSmallDelegate :
    AdapterDelegate<MutableList<CommonListElementAdapterMarker>>() {

    override fun isForViewType(
        items: MutableList<CommonListElementAdapterMarker>,
        position: Int
    ): Boolean {
        return items[position] is CommonTitleSmallUi
    }

    override fun onCreateViewHolder(parent: ViewGroup): RecyclerView.ViewHolder {
        return ViewHolder(parent.inflateBinding(ListItemSmallTitleBinding::inflate))
    }

    override fun onBindViewHolder(
        items: MutableList<CommonListElementAdapterMarker>,
        position: Int,
        holder: RecyclerView.ViewHolder,
        payloads: MutableList<Any>
    ) {
        (holder as ViewHolder).bind(items[position] as CommonTitleSmallUi)
    }

    private inner class ViewHolder(
        private val binding: ListItemSmallTitleBinding
    ) : RecyclerView.ViewHolder(binding.root) {

        fun bind(model: CommonTitleSmallUi) {
            binding.tvTitle.text = model.title.getString(binding.root.context)
            binding.tvTitle.maxLines = model.maxLines
            binding.rootLayout.requestFocus()
        }
    }
}