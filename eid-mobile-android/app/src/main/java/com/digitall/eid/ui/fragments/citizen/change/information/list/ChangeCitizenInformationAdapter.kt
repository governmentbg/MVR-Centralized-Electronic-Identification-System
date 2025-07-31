package com.digitall.eid.ui.fragments.citizen.change.information.list

import androidx.recyclerview.widget.RecyclerView
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonEditTextUi
import com.digitall.eid.models.citizen.change.information.ChangeCitizenInformationAdapterMarker
import com.digitall.eid.models.list.CommonPhoneTextUi
import com.digitall.eid.ui.common.list.CommonButtonDelegate
import com.digitall.eid.ui.common.list.CommonEditTextDelegate
import com.digitall.eid.ui.common.list.CommonPhoneTextDelegate
import com.digitall.eid.utils.DefaultDiffUtilCallback
import com.hannesdorfmann.adapterdelegates4.AdapterDelegate
import com.hannesdorfmann.adapterdelegates4.AsyncListDifferDelegationAdapter
import org.koin.core.component.KoinComponent
import org.koin.core.component.inject

class ChangeCitizenInformationAdapter :
    AsyncListDifferDelegationAdapter<ChangeCitizenInformationAdapterMarker>(DefaultDiffUtilCallback()),
    KoinComponent {

    private val commonButtonDelegate: CommonButtonDelegate by inject()
    private val commonEditTextDelegate: CommonEditTextDelegate by inject()
    private val commonPhoneTextDelegate: CommonPhoneTextDelegate by inject()

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
            commonEditTextDelegate.characterFilterListener = { model, char ->
                clickListener?.onCharacterFilter(model, char) ?: true
            }
            commonPhoneTextDelegate.changeListener = {
                clickListener?.onPhoneTextChanged(it)
            }
            commonPhoneTextDelegate.doneListener = {
                clickListener?.onPhoneTextDone(it)
            }
            commonPhoneTextDelegate.focusChangedListener = {
                clickListener?.onPhoneTextFocusChanged(it)
            }
            commonPhoneTextDelegate.characterFilterListener = { model, char ->
                clickListener?.onPhoneCharacterFilter(model, char) ?: true
            }
            commonButtonDelegate.clickListener = {
                clickListener?.onButtonClicked(it)
            }
        }

    var recyclerViewProvider: (() -> RecyclerView?)? = null
        set(value) {
            field = value
            commonEditTextDelegate.recyclerViewProvider = value
            commonPhoneTextDelegate.recyclerViewProvider = value
        }

    init {
        items = mutableListOf()
        @Suppress("UNCHECKED_CAST")
        delegatesManager.apply {
            addDelegate(commonButtonDelegate as AdapterDelegate<MutableList<ChangeCitizenInformationAdapterMarker>>)
            addDelegate(commonEditTextDelegate as AdapterDelegate<MutableList<ChangeCitizenInformationAdapterMarker>>)
            addDelegate(commonPhoneTextDelegate as AdapterDelegate<MutableList<ChangeCitizenInformationAdapterMarker>>)
        }
    }

    interface ClickListener {
        fun onEditTextFocusChanged(model: CommonEditTextUi)
        fun onEditTextDone(model: CommonEditTextUi)
        fun onEditTextChanged(model: CommonEditTextUi)
        fun onCharacterFilter(model: CommonEditTextUi, char: Char): Boolean
        fun onPhoneTextFocusChanged(model: CommonPhoneTextUi)
        fun onPhoneTextDone(model: CommonPhoneTextUi)
        fun onPhoneTextChanged(model: CommonPhoneTextUi)
        fun onPhoneCharacterFilter(model: CommonPhoneTextUi, char: Char): Boolean
        fun onButtonClicked(model: CommonButtonUi)
    }
}
