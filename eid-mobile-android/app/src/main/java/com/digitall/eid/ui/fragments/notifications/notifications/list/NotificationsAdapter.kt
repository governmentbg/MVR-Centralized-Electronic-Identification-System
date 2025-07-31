/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.notifications.notifications.list

import com.digitall.eid.models.notifications.notifications.NotificationAdapterMarker
import com.digitall.eid.utils.DefaultDiffUtilCallback
import com.hannesdorfmann.adapterdelegates4.AsyncListDifferDelegationAdapter
import org.koin.core.component.KoinComponent
import org.koin.core.component.inject

class NotificationsAdapter :
    AsyncListDifferDelegationAdapter<NotificationAdapterMarker>(DefaultDiffUtilCallback()),
    KoinComponent {

    private val notificationChildDelegate: NotificationChildDelegate by inject()
    private val notificationParentDelegate: NotificationParentDelegate by inject()

    var clickListener: ClickListener? = null
        set(value) {
            field = value
            notificationChildDelegate.clickListener = { clickListener?.onChildElementClicked(it) }
            notificationParentDelegate.clickListener = { clickListener?.onParentElementClicked(it) }
            notificationParentDelegate.checkBoxClickListener =
                { clickListener?.onParentElementCheckBoxClicked(it) }
        }

    init {
        items = mutableListOf()
        delegatesManager.apply {
            addDelegate(notificationChildDelegate)
            addDelegate(notificationParentDelegate)
        }
    }

    interface ClickListener {
        fun onChildElementClicked(id: String)
        fun onParentElementClicked(id: String)
        fun onParentElementCheckBoxClicked(id: String)
    }

}