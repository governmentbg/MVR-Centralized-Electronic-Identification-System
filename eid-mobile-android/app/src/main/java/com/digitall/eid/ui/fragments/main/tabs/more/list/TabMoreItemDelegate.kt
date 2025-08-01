/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.main.tabs.more.list

import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.digitall.eid.databinding.ListItemSettingsItemBinding
import com.digitall.eid.extensions.inflateBinding
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.models.main.more.TabMoreAdapterMarker
import com.digitall.eid.models.main.more.TabMoreItemUi
import com.digitall.eid.models.main.more.TabMoreItems
import com.hannesdorfmann.adapterdelegates4.AdapterDelegate

class TabMoreItemDelegate : AdapterDelegate<MutableList<TabMoreAdapterMarker>>() {

    var clickListener: ((type: TabMoreItems) -> Unit)? = null

    override fun isForViewType(items: MutableList<TabMoreAdapterMarker>, position: Int): Boolean {
        return items[position] is TabMoreItemUi
    }

    override fun onCreateViewHolder(parent: ViewGroup): RecyclerView.ViewHolder {
        return ViewHolder(parent.inflateBinding(ListItemSettingsItemBinding::inflate))
    }

    override fun onBindViewHolder(
        items: MutableList<TabMoreAdapterMarker>,
        position: Int,
        holder: RecyclerView.ViewHolder,
        payloads: MutableList<Any>
    ) {
        (holder as ViewHolder).bind(items[position] as TabMoreItemUi)
    }

    private inner class ViewHolder(
        private val binding: ListItemSettingsItemBinding
    ) : RecyclerView.ViewHolder(binding.root) {

        fun bind(model: TabMoreItemUi) {
            binding.tvTitle.text = model.itemText.getString(binding.root.context)
            binding.rootLayout.onClickThrottle {
                clickListener?.invoke(model.type)
            }
        }
    }

}