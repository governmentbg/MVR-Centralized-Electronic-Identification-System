/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.common.search.normal.list

import com.digitall.eid.models.list.CommonDialogWithSearchAdapterMarker
import com.digitall.eid.models.list.CommonDialogWithSearchItemUi
import com.digitall.eid.utils.DefaultDiffUtilCallback
import com.hannesdorfmann.adapterdelegates4.AdapterDelegate
import com.hannesdorfmann.adapterdelegates4.AsyncListDifferDelegationAdapter
import org.koin.core.component.KoinComponent
import org.koin.core.component.inject

class CommonBottomSheetWithSearchAdapter :
    AsyncListDifferDelegationAdapter<CommonDialogWithSearchAdapterMarker>(DefaultDiffUtilCallback()),
    KoinComponent {

    private val commonBottomSheetWithSearchDelegate: CommonBottomSheetWithSearchDelegate by inject()

    var clickListener: ClickListener? = null
        set(value) {
            field = value
            commonBottomSheetWithSearchDelegate.clickListener = { model ->
                clickListener?.onClicked(model)
            }
        }

    init {
        items = mutableListOf()
        delegatesManager.apply {
            addDelegate(commonBottomSheetWithSearchDelegate as AdapterDelegate<MutableList<CommonDialogWithSearchAdapterMarker>>)
        }
    }

    fun interface ClickListener {
        fun onClicked(selected: CommonDialogWithSearchItemUi)
    }

}