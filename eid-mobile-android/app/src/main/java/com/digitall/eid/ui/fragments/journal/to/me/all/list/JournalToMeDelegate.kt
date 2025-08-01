/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.journal.to.me.all.list

import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.digitall.eid.databinding.ListItemJournalToMeBinding
import com.digitall.eid.extensions.inflateBinding
import com.digitall.eid.models.journal.to.me.JournalToMeUi
import com.hannesdorfmann.adapterdelegates4.AbsFallbackAdapterDelegate

class JournalToMeDelegate :
    AbsFallbackAdapterDelegate<MutableList<JournalToMeUi>>() {

    companion object {
        private const val TAG = "JournalToMeDelegateTag"
    }

    override fun onCreateViewHolder(parent: ViewGroup): RecyclerView.ViewHolder {
        return ViewHolder(parent.inflateBinding(ListItemJournalToMeBinding::inflate))
    }

    override fun onBindViewHolder(
        items: MutableList<JournalToMeUi>,
        position: Int,
        holder: RecyclerView.ViewHolder,
        payloads: MutableList<Any>
    ) {
        (holder as ViewHolder).bind(items[position])
    }

    private inner class ViewHolder(
        private val binding: ListItemJournalToMeBinding
    ) : RecyclerView.ViewHolder(binding.root) {

        fun bind(model: JournalToMeUi) {
            binding.tvTitle.text = model.eventType
            binding.tvSystem.text = model.system
            binding.tvIdentifier.text = model.eventId
            binding.tvDate.text = model.eventDate
        }
    }
}