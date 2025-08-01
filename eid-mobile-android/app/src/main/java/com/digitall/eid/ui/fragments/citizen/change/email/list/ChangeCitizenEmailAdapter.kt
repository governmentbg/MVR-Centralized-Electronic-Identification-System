package com.digitall.eid.ui.fragments.citizen.change.email.list

import androidx.recyclerview.widget.RecyclerView
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonEditTextUi
import com.digitall.eid.models.citizen.change.email.ChangeCitizenEmailAdapterMarker
import com.digitall.eid.ui.common.list.CommonButtonDelegate
import com.digitall.eid.ui.common.list.CommonEditTextDelegate
import com.digitall.eid.utils.DefaultDiffUtilCallback
import com.hannesdorfmann.adapterdelegates4.AdapterDelegate
import com.hannesdorfmann.adapterdelegates4.AsyncListDifferDelegationAdapter
import org.koin.core.component.KoinComponent
import org.koin.core.component.inject

class ChangeCitizenEmailAdapter :
    AsyncListDifferDelegationAdapter<ChangeCitizenEmailAdapterMarker>(DefaultDiffUtilCallback()),
    KoinComponent {

    private val commonButtonDelegate: CommonButtonDelegate by inject()
    private val commonEditTextDelegate: CommonEditTextDelegate by inject()

    var clickListener: ClickListener? = null
        set(value) {
            field = value
            commonEditTextDelegate.changeListener = {
                clickListener?.onEditTextChanged(it)
            }
            commonEditTextDelegate.doneListener = {
                clickListener?.onEditTextDone(it)
            }
            commonEditTextDelegate.focusChangedListener = {
                clickListener?.onEditTextFocusChanged(it)
            }
            commonButtonDelegate.clickListener = {
                clickListener?.onButtonClicked(it)
            }
        }

    var recyclerViewProvider: (() -> RecyclerView?)? = null
        set(value) {
            field = value
            commonEditTextDelegate.recyclerViewProvider = value
        }

    init {
        items = mutableListOf()
        @Suppress("UNCHECKED_CAST")
        delegatesManager.apply {
            addDelegate(commonButtonDelegate as AdapterDelegate<MutableList<ChangeCitizenEmailAdapterMarker>>)
            addDelegate(commonEditTextDelegate as AdapterDelegate<MutableList<ChangeCitizenEmailAdapterMarker>>)
        }
    }

    interface ClickListener {
        fun onEditTextFocusChanged(model: CommonEditTextUi)
        fun onEditTextDone(model: CommonEditTextUi)
        fun onEditTextChanged(model: CommonEditTextUi)
        fun onButtonClicked(model: CommonButtonUi)
    }
}