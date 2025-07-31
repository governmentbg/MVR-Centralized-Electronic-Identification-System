/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.empowerment.to.me.all.list

import android.view.View
import android.view.ViewGroup
import androidx.paging.PagingDataAdapter
import androidx.recyclerview.widget.RecyclerView
import com.digitall.eid.models.empowerment.to.me.all.EmpowermentToMeUi
import com.digitall.eid.utils.DefaultDiffUtilCallback
import com.hannesdorfmann.adapterdelegates4.AdapterDelegate
import com.hannesdorfmann.adapterdelegates4.AdapterDelegatesManager
import org.koin.core.component.KoinComponent
import org.koin.core.component.inject

class EmpowermentToMeAdapter :
    PagingDataAdapter<EmpowermentToMeUi, RecyclerView.ViewHolder>(DefaultDiffUtilCallback()),
    KoinComponent {

    private val empowermentToMeDelegate: EmpowermentToMeDelegate by inject()

    var clickListener: ClickListener? = null
        set(value) {
            field = value
            empowermentToMeDelegate.copyClickListener = { model, anchor ->
                clickListener?.onSpinnerClicked(
                    model = model,
                    anchor = anchor,
                )
            }
            empowermentToMeDelegate.openClickListener = {
                clickListener?.onOpenClicked(it)
            }
        }

    private val delegatesManager = AdapterDelegatesManager<List<Any>>()

    init {
        @Suppress("UNCHECKED_CAST")
        delegatesManager.apply {
            addDelegate(empowermentToMeDelegate as AdapterDelegate<List<Any>>)
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

    interface ClickListener {
        fun onSpinnerClicked(model: EmpowermentToMeUi, anchor: View)
        fun onOpenClicked(model: EmpowermentToMeUi)
    }

}