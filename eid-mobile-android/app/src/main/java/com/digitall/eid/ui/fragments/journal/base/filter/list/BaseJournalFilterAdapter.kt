package com.digitall.eid.ui.fragments.journal.base.filter.list

import com.digitall.eid.models.journal.common.filter.JournalFilterAdapterMarker
import com.digitall.eid.models.list.CommonDatePickerUi
import com.digitall.eid.models.list.CommonDialogWithSearchMultiselectUi
import com.digitall.eid.models.list.CommonDoubleButtonUi
import com.digitall.eid.ui.common.list.CommonDatePickerDelegate
import com.digitall.eid.ui.common.list.CommonDialogWithSearchMultiselectDelegate
import com.digitall.eid.ui.common.list.CommonDoubleButtonDelegate
import com.digitall.eid.utils.DefaultDiffUtilCallback
import com.hannesdorfmann.adapterdelegates4.AdapterDelegate
import com.hannesdorfmann.adapterdelegates4.AsyncListDifferDelegationAdapter
import org.koin.core.component.KoinComponent
import org.koin.core.component.inject

abstract class BaseJournalFilterAdapter:
    AsyncListDifferDelegationAdapter<JournalFilterAdapterMarker>(DefaultDiffUtilCallback()),
    KoinComponent {

    private val commonDatePickerDelegate: CommonDatePickerDelegate by inject()
    private val commonDialogWithSearchMultiselectDelegate: CommonDialogWithSearchMultiselectDelegate by inject()

    var clickListener: ClickListener? = null
        set(value) {
            field = value
            commonDatePickerDelegate.clickListener = {
                clickListener?.onDatePickerClicked(it)
            }
            commonDialogWithSearchMultiselectDelegate.clickListener = {
                clickListener?.onDialogWithSearchMultiselectClicked(it)
            }
//            commonDialogWithSearchDelegate.eraseClickListener = {
//                clickListener?.onEraseClicked(it)
//            }
        }

    init {
        items = mutableListOf()
        @Suppress("UNCHECKED_CAST")
        delegatesManager.apply {
            addDelegate(commonDatePickerDelegate as AdapterDelegate<MutableList<JournalFilterAdapterMarker>>)
            addDelegate(commonDialogWithSearchMultiselectDelegate as AdapterDelegate<MutableList<JournalFilterAdapterMarker>>)
        }
    }

    interface ClickListener {
        fun onDatePickerClicked(model: CommonDatePickerUi)
        fun onDialogWithSearchMultiselectClicked(model: CommonDialogWithSearchMultiselectUi)
    }

}