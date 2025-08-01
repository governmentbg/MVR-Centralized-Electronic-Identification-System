package com.digitall.eid.ui.fragments.empowerment.legal.all.list

import android.view.View
import android.view.ViewGroup
import androidx.paging.PagingDataAdapter
import androidx.recyclerview.widget.RecyclerView
import com.digitall.eid.models.empowerment.legal.all.EmpowermentLegalUi
import com.digitall.eid.utils.DefaultDiffUtilCallback
import com.hannesdorfmann.adapterdelegates4.AdapterDelegate
import com.hannesdorfmann.adapterdelegates4.AdapterDelegatesManager
import org.koin.core.component.KoinComponent
import org.koin.core.component.inject

class EmpowermentLegalAdapter :
    PagingDataAdapter<EmpowermentLegalUi, RecyclerView.ViewHolder>(DefaultDiffUtilCallback()),
    KoinComponent {

    private val empowermentLegalDelegate: EmpowermentLegalDelegate by inject()

    var clickListener: ClickListener? = null
        set(value) {
            field = value
            empowermentLegalDelegate.copyClickListener = { model, anchor ->
                clickListener?.onCopyClicked(
                    model = model,
                    anchor = anchor,
                )
            }
            empowermentLegalDelegate.signClickListener = {
                clickListener?.onSignClicked(it)
            }
            empowermentLegalDelegate.openClickListener = {
                clickListener?.onOpenClicked(it)
            }
        }

    private val delegatesManager = AdapterDelegatesManager<List<Any>>()

    init {
        @Suppress("UNCHECKED_CAST")
        delegatesManager.apply {
            addDelegate(empowermentLegalDelegate as AdapterDelegate<List<Any>>)
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
        fun onSignClicked(model: EmpowermentLegalUi)
        fun onOpenClicked(model: EmpowermentLegalUi)
        fun onCopyClicked(model: EmpowermentLegalUi, anchor: View)
    }

}