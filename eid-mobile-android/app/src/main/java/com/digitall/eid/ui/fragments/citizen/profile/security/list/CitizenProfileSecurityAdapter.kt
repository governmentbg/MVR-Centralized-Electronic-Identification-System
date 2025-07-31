package com.digitall.eid.ui.fragments.citizen.profile.security.list

import com.digitall.eid.models.citizen.profile.security.CitizenProfileSecurityAdapterMarker
import com.digitall.eid.models.list.CommonTitleCheckboxUi
import com.digitall.eid.ui.common.list.CommonTitleCheckboxDelegate
import com.digitall.eid.utils.DefaultDiffUtilCallback
import com.hannesdorfmann.adapterdelegates4.AdapterDelegate
import com.hannesdorfmann.adapterdelegates4.AsyncListDifferDelegationAdapter
import org.koin.core.component.KoinComponent
import org.koin.core.component.inject

class CitizenProfileSecurityAdapter :
    AsyncListDifferDelegationAdapter<CitizenProfileSecurityAdapterMarker>(DefaultDiffUtilCallback()),
    KoinComponent {

    private val commonTitleCheckboxDelegate: CommonTitleCheckboxDelegate by inject()

    var clickListener: ClickListener? = null
        set(value) {
            field = value
            commonTitleCheckboxDelegate.changeListener = {
                clickListener?.onCheckBoxChangeState(it)
            }
        }

    init {
        items = mutableListOf()
        @Suppress("UNCHECKED_CAST")
        delegatesManager.apply {
            addDelegate(commonTitleCheckboxDelegate as AdapterDelegate<MutableList<CitizenProfileSecurityAdapterMarker>>)
        }
    }

    interface ClickListener {
        fun onCheckBoxChangeState(model: CommonTitleCheckboxUi)
    }
}