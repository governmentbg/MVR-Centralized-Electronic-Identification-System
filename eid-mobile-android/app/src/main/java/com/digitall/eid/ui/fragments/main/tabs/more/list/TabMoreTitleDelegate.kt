/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.main.tabs.more.list

import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.digitall.eid.databinding.ListItemSettingsTitleBinding
import com.digitall.eid.extensions.inflateBinding
import com.digitall.eid.models.main.more.TabMoreAdapterMarker
import com.digitall.eid.models.main.more.TabMoreTitleUi
import com.hannesdorfmann.adapterdelegates4.AdapterDelegate

class TabMoreTitleDelegate : AdapterDelegate<MutableList<TabMoreAdapterMarker>>() {

    override fun isForViewType(items: MutableList<TabMoreAdapterMarker>, position: Int): Boolean {
        return items[position] is TabMoreTitleUi
    }

    override fun onCreateViewHolder(parent: ViewGroup): RecyclerView.ViewHolder {
        return ViewHolder(parent.inflateBinding(ListItemSettingsTitleBinding::inflate))
    }

    override fun onBindViewHolder(
        items: MutableList<TabMoreAdapterMarker>,
        position: Int,
        holder: RecyclerView.ViewHolder,
        payloads: MutableList<Any>
    ) {
        (holder as ViewHolder).bind(items[position] as TabMoreTitleUi)
    }

    private inner class ViewHolder(
        private val binding: ListItemSettingsTitleBinding
    ) : RecyclerView.ViewHolder(binding.root) {

        fun bind(model: TabMoreTitleUi) {
            binding.ivIcon.setImageResource(model.itemImageRes)
            binding.tvTitle.text = model.itemText.getString(binding.root.context)
        }
    }

}