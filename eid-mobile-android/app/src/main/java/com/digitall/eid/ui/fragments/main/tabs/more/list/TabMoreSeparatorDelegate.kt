package com.digitall.eid.ui.fragments.main.tabs.more.list

import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.digitall.eid.databinding.ListItemSeparatorBinding
import com.digitall.eid.extensions.inflateBinding
import com.digitall.eid.models.main.more.TabMoreAdapterMarker
import com.digitall.eid.models.main.more.TabMoreSeparatorUi
import com.hannesdorfmann.adapterdelegates4.AdapterDelegate

class TabMoreSeparatorDelegate : AdapterDelegate<MutableList<TabMoreAdapterMarker>>() {

    override fun isForViewType(items: MutableList<TabMoreAdapterMarker>, position: Int): Boolean {
        return items[position] is TabMoreSeparatorUi
    }

    override fun onCreateViewHolder(parent: ViewGroup): RecyclerView.ViewHolder {
        return ViewHolder(parent.inflateBinding(ListItemSeparatorBinding::inflate))
    }

    override fun onBindViewHolder(
        items: MutableList<TabMoreAdapterMarker>,
        position: Int,
        holder: RecyclerView.ViewHolder,
        payloads: MutableList<Any>
    ) {
        (holder as ViewHolder).bind(items[position] as TabMoreSeparatorUi)
    }

    private inner class ViewHolder(
        private val binding: ListItemSeparatorBinding
    ) : RecyclerView.ViewHolder(binding.root) {

        fun bind(model: TabMoreSeparatorUi) {
            val layoutParams = binding.rootLayout.layoutParams as RecyclerView.LayoutParams
            layoutParams.setMargins(
                model.marginLeft,
                model.marginTop,
                model.marginRight,
                model.marginBottom
            )
            binding.rootLayout.layoutParams = layoutParams
            binding.rootLayout.requestLayout()
        }
    }

}