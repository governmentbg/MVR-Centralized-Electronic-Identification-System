/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.journal.to.me.all.list

import android.view.ViewGroup
import androidx.paging.PagingDataAdapter
import androidx.recyclerview.widget.RecyclerView
import com.digitall.eid.models.journal.to.me.JournalToMeUi
import com.digitall.eid.utils.DefaultDiffUtilCallback
import com.hannesdorfmann.adapterdelegates4.AdapterDelegate
import com.hannesdorfmann.adapterdelegates4.AdapterDelegatesManager
import org.koin.core.component.KoinComponent
import org.koin.core.component.inject

class JournalToMeAdapter :
    PagingDataAdapter<JournalToMeUi, RecyclerView.ViewHolder>(DefaultDiffUtilCallback()),
    KoinComponent {

    companion object {
        private const val TAG = "JournalToMeAdapterTag"
    }

    private val journalToMeDelegate: JournalToMeDelegate by inject()

    private val delegatesManager = AdapterDelegatesManager<List<Any>>()

    init {
        @Suppress("UNCHECKED_CAST")
        delegatesManager.apply {
            addDelegate(journalToMeDelegate as AdapterDelegate<List<Any>>)
        }
    }

    override fun getItemViewType(position: Int): Int {
        val item = getItem(position)
        return if (item != null) {
            delegatesManager.getItemViewType(listOf(item), 0)
        } else {
            super.getItemViewType(position)
        }
    }


    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): RecyclerView.ViewHolder {
        return delegatesManager.onCreateViewHolder(parent, viewType)
    }

    override fun onBindViewHolder(holder: RecyclerView.ViewHolder, position: Int) {
        val item = getItem(position)
        if (item != null) {
            delegatesManager.onBindViewHolder(
                listOf(item),
                0,
                holder,
                mutableListOf<Any>()
            )
        } else {
            // Handle binding for placeholder ViewHolders if any
        }
    }

    override fun onBindViewHolder(
        holder: RecyclerView.ViewHolder,
        position: Int,
        payloads: MutableList<Any>
    ) {
        if (payloads.isNotEmpty()) {
            val item = getItem(position)
            if (item != null) {
                delegatesManager.onBindViewHolder(
                    mutableListOf(item),
                    0,
                    holder,
                    payloads
                )
            }
        } else {
            onBindViewHolder(holder, position)
        }
    }

}