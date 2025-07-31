package com.digitall.eid.ui.fragments.common.search.multiselect.list

import com.digitall.eid.models.list.CommonDialogWithSearchAdapterMarker
import com.digitall.eid.models.list.CommonDialogWithSearchMultiselectItemUi
import com.digitall.eid.ui.fragments.common.search.normal.list.CommonBottomSheetWithSearchDelegate
import com.digitall.eid.utils.DefaultDiffUtilCallback
import com.hannesdorfmann.adapterdelegates4.AdapterDelegate
import com.hannesdorfmann.adapterdelegates4.AsyncListDifferDelegationAdapter
import org.koin.core.component.KoinComponent
import org.koin.core.component.inject

class CommonBottomSheetWithSearchMultiselectAdapter :
    AsyncListDifferDelegationAdapter<CommonDialogWithSearchAdapterMarker>(DefaultDiffUtilCallback()),
    KoinComponent {

    private val commonBottomSheetWithSearchMultiselectDelegate: CommonBottomSheetWithSearchMultiselectDelegate by inject()
    private val commonBottomSheetWithSearchDelegate: CommonBottomSheetWithSearchDelegate by inject()

    var clickListener: ClickListener? = null
        set(value) {
            field = value
            commonBottomSheetWithSearchMultiselectDelegate.changeListener = { model ->
                clickListener?.onChecked(model)
            }
        }

    init {
        items = mutableListOf()
        delegatesManager.apply {
            addDelegate(commonBottomSheetWithSearchMultiselectDelegate as AdapterDelegate<MutableList<CommonDialogWithSearchAdapterMarker>>)
            addDelegate(commonBottomSheetWithSearchDelegate as AdapterDelegate<MutableList<CommonDialogWithSearchAdapterMarker>>)
        }
    }

    interface ClickListener {
        fun onChecked(selected: CommonDialogWithSearchMultiselectItemUi)
    }

}