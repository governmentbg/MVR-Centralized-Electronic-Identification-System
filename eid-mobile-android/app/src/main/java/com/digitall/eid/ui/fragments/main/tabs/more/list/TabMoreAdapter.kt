/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.main.tabs.more.list

import com.digitall.eid.models.main.more.TabMoreAdapterMarker
import com.digitall.eid.models.main.more.TabMoreItems
import com.digitall.eid.utils.DefaultDiffUtilCallback
import com.hannesdorfmann.adapterdelegates4.AsyncListDifferDelegationAdapter
import org.koin.core.component.KoinComponent
import org.koin.core.component.inject

class TabMoreAdapter :
    AsyncListDifferDelegationAdapter<TabMoreAdapterMarker>(DefaultDiffUtilCallback()),
    KoinComponent {

    private val tabFourItemDelegate: TabMoreItemDelegate by inject()
    private val tabFourTitleDelegate: TabMoreTitleDelegate by inject()
    private val tabFourSeparatorDelegate: TabMoreSeparatorDelegate by inject()

    var clickListener: ClickListener? = null
        set(value) {
            field = value
            tabFourItemDelegate.clickListener = { clickListener?.onElementClicked(it) }
        }

    init {
        items = mutableListOf()
        delegatesManager.apply {
            addDelegate(tabFourItemDelegate)
            addDelegate(tabFourTitleDelegate)
            addDelegate(tabFourSeparatorDelegate)
        }
    }

    interface ClickListener {
        fun onElementClicked(type: TabMoreItems)
    }
}