/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.notifications.channels.list

import com.digitall.eid.models.notifications.channels.NotificationChannelsAdapterMarker
import com.digitall.eid.utils.DefaultDiffUtilCallback
import com.hannesdorfmann.adapterdelegates4.AsyncListDifferDelegationAdapter
import org.koin.core.component.KoinComponent
import org.koin.core.component.inject

class NotificationChannelsAdapter : AsyncListDifferDelegationAdapter<NotificationChannelsAdapterMarker>(DefaultDiffUtilCallback()),
    KoinComponent {

    private val notificationChannelDelegate: NotificationChannelDelegate by inject()

    var clickListener: ClickListener? = null
        set(value) {
            field = value
            notificationChannelDelegate.clickListener =
                { clickListener?.onNotificationSelected(it) }
        }

    init {
        items = mutableListOf()
        delegatesManager.apply {
            addDelegate(notificationChannelDelegate)
        }
    }

    interface ClickListener {
        fun onNotificationSelected(id: String)
    }

}