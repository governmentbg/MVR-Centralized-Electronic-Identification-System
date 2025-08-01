/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.journal.from.me.all.list

import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.digitall.eid.databinding.ListItemJournalFromMeBinding
import com.digitall.eid.extensions.inflateBinding
import com.digitall.eid.models.journal.from.me.JournalFromMeUi
import com.hannesdorfmann.adapterdelegates4.AbsFallbackAdapterDelegate

class JournalFromMeDelegate :
    AbsFallbackAdapterDelegate<MutableList<JournalFromMeUi>>() {

    companion object {
        private const val TAG = "JournalFromMeDelegateTag"
    }

    override fun onCreateViewHolder(parent: ViewGroup): RecyclerView.ViewHolder {
        return ViewHolder(parent.inflateBinding(ListItemJournalFromMeBinding::inflate))
    }

    override fun onBindViewHolder(
        items: MutableList<JournalFromMeUi>,
        position: Int,
        holder: RecyclerView.ViewHolder,
        payloads: MutableList<Any>
    ) {
        (holder as ViewHolder).bind(items[position])
    }

    private inner class ViewHolder(
        private val binding: ListItemJournalFromMeBinding
    ) : RecyclerView.ViewHolder(binding.root) {

        fun bind(model: JournalFromMeUi) {
            binding.tvTitle.text = model.eventType
            binding.tvIdentifier.text = model.eventId
            binding.tvDate.text = model.eventDate
        }
    }
}