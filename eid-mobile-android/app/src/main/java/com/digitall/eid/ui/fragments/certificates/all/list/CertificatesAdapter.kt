/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.certificates.all.list

import android.view.View
import android.view.ViewGroup
import androidx.paging.PagingDataAdapter
import androidx.recyclerview.widget.RecyclerView
import com.digitall.eid.models.certificates.all.CertificateUi
import com.digitall.eid.utils.DefaultDiffUtilCallback
import com.hannesdorfmann.adapterdelegates4.AdapterDelegate
import com.hannesdorfmann.adapterdelegates4.AdapterDelegatesManager
import org.koin.core.component.KoinComponent
import org.koin.core.component.inject

class CertificatesAdapter :
    PagingDataAdapter<CertificateUi, RecyclerView.ViewHolder>(DefaultDiffUtilCallback()),
    KoinComponent {

    companion object {
        private const val TAG = "CertificatesAdapterTag"
    }

    private val certificatesDelegate: CertificatesDelegate by inject()

    private val delegatesManager = AdapterDelegatesManager<List<Any>>()

    var clickListener: ClickListener? = null
        set(value) {
            field = value
            certificatesDelegate.openClickListener = {
                clickListener?.onOpenClicked(it)
            }
            certificatesDelegate.menuClickListener = { model, anchor ->
                clickListener?.onSpinnerClicked(model = model, anchor = anchor)
            }
        }

    init {
        @Suppress("UNCHECKED_CAST")
        delegatesManager.apply {
            addDelegate(certificatesDelegate as AdapterDelegate<List<Any>?>)
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
        fun onOpenClicked(model: CertificateUi)
        fun onSpinnerClicked(model: CertificateUi, anchor: View)
    }

}