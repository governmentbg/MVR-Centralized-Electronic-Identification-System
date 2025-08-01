package com.digitall.eid.ui.fragments.empowerment.legal.search.list

import com.digitall.eid.models.empowerment.legal.search.EmpowermentLegalSearchAdapterMarker
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonEditTextUi
import com.digitall.eid.ui.common.list.CommonButtonDelegate
import com.digitall.eid.ui.common.list.CommonEditTextDelegate
import com.digitall.eid.utils.DefaultDiffUtilCallback
import com.hannesdorfmann.adapterdelegates4.AdapterDelegate
import com.hannesdorfmann.adapterdelegates4.AsyncListDifferDelegationAdapter
import org.koin.core.component.KoinComponent
import org.koin.core.component.inject

class EmpowermentLegalSearchAdapter :
    AsyncListDifferDelegationAdapter<EmpowermentLegalSearchAdapterMarker>(DefaultDiffUtilCallback()),
    KoinComponent {

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

    private val commonButtonDelegate: CommonButtonDelegate by inject()
    private val commonEditTextDelegate: CommonEditTextDelegate by inject()

    init {
        items = mutableListOf()
        @Suppress("UNCHECKED_CAST")
        delegatesManager.apply {
            addDelegate(commonButtonDelegate as AdapterDelegate<MutableList<EmpowermentLegalSearchAdapterMarker>>)
            addDelegate(commonEditTextDelegate as AdapterDelegate<MutableList<EmpowermentLegalSearchAdapterMarker>>)
        }
    }

    interface ClickListener {
        fun onButtonClicked(model: CommonButtonUi)
        fun onEditTextFocusChanged(model: CommonEditTextUi)
        fun onEditTextDone(model: CommonEditTextUi)
        fun onEditTextChanged(model: CommonEditTextUi)
    }

}