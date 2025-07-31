package com.digitall.eid.ui.fragments.certificates.change.pin.list

import androidx.recyclerview.widget.RecyclerView
import com.digitall.eid.models.card.change.pin.CertificateChangePinAdapterMarker
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonEditTextUi
import com.digitall.eid.ui.common.list.CommonButtonDelegate
import com.digitall.eid.ui.common.list.CommonDisclaimerTextDelegate
import com.digitall.eid.ui.common.list.CommonEditTextDelegate
import com.digitall.eid.utils.DefaultDiffUtilCallback
import com.hannesdorfmann.adapterdelegates4.AdapterDelegate
import com.hannesdorfmann.adapterdelegates4.AsyncListDifferDelegationAdapter
import org.koin.core.component.KoinComponent
import org.koin.core.component.inject

class CertificateChangePinAdapter:
    AsyncListDifferDelegationAdapter<CertificateChangePinAdapterMarker>(DefaultDiffUtilCallback()),
    KoinComponent {

    private val commonButtonDelegate: CommonButtonDelegate by inject()
    private val commonEditTextDelegate: CommonEditTextDelegate by inject()
    private val commonDisclaimerTextDelegate: CommonDisclaimerTextDelegate by inject()

    var clickListener: ClickListener? = null
        set(value) {
            field = value
            commonEditTextDelegate.changeListener = {
                clickListener?.onEditTextChanged(it)
            }
            commonEditTextDelegate.focusChangedListener = {
                clickListener?.onEditTextFocusChanged(it)
            }
            commonEditTextDelegate.doneListener = {
                clickListener?.onEditTextDone(it)
            }
            commonEditTextDelegate.characterFilterListener = { model, char ->
                clickListener?.onCharacterFilter(model, char) ?: false
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
            addDelegate(commonButtonDelegate as AdapterDelegate<MutableList<CertificateChangePinAdapterMarker>>)
            addDelegate(commonEditTextDelegate as AdapterDelegate<MutableList<CertificateChangePinAdapterMarker>>)
            addDelegate(commonDisclaimerTextDelegate as AdapterDelegate<MutableList<CertificateChangePinAdapterMarker>>)
        }
    }

    interface ClickListener {
        fun onEditTextFocusChanged(model: CommonEditTextUi)
        fun onEditTextDone(model: CommonEditTextUi)
        fun onEditTextChanged(model: CommonEditTextUi)
        fun onCharacterFilter(model: CommonEditTextUi, char: Char): Boolean
        fun onButtonClicked(model: CommonButtonUi)
    }

}