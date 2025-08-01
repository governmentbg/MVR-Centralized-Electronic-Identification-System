package com.digitall.eid.ui.fragments.main.tabs.requests.list

import com.digitall.eid.models.requests.RequestUi
import com.digitall.eid.utils.DefaultDiffUtilCallback
import com.hannesdorfmann.adapterdelegates4.AsyncListDifferDelegationAdapter
import org.koin.core.component.KoinComponent
import org.koin.core.component.inject

class TabRequestsAdapter :
    AsyncListDifferDelegationAdapter<RequestUi>(DefaultDiffUtilCallback()),
    KoinComponent {

    private val tabRequestsDelegate: TabRequestsDelegate by inject()

    var clickListener: ClickListener? = null
        set(value) {
            field = value
            tabRequestsDelegate.acceptClickListener = { clickListener?.onRequestAccept(it) }
            tabRequestsDelegate.declineClickListener = { clickListener?.onRequestDecline(it) }
        }

    init {
        delegatesManager.apply {
            addDelegate(tabRequestsDelegate)
        }
    }

    interface ClickListener {
        fun onRequestAccept(model: RequestUi)
        fun onRequestDecline(model: RequestUi)
    }
}