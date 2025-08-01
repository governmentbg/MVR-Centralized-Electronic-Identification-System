package com.digitall.eid.ui.fragments.certificates.stop.list

import android.view.View
import androidx.recyclerview.widget.RecyclerView
import com.digitall.eid.models.certificates.stop.CertificateStopAdapterMarker
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonDatePickerUi
import com.digitall.eid.models.list.CommonDialogWithSearchUi
import com.digitall.eid.models.list.CommonEditTextUi
import com.digitall.eid.models.list.CommonSpinnerUi
import com.digitall.eid.ui.common.list.CommonButtonDelegate
import com.digitall.eid.ui.common.list.CommonButtonTransparentDelegate
import com.digitall.eid.ui.common.list.CommonDatePickerDelegate
import com.digitall.eid.ui.common.list.CommonDialogWithSearchDelegate
import com.digitall.eid.ui.common.list.CommonEditTextDelegate
import com.digitall.eid.ui.common.list.CommonSeparatorDelegate
import com.digitall.eid.ui.common.list.CommonSpinnerDelegate
import com.digitall.eid.ui.common.list.CommonTextFieldDelegate
import com.digitall.eid.ui.common.list.CommonTitleDelegate
import com.digitall.eid.ui.common.list.CommonTitleSmallDelegate
import com.digitall.eid.utils.DefaultDiffUtilCallback
import com.hannesdorfmann.adapterdelegates4.AdapterDelegate
import com.hannesdorfmann.adapterdelegates4.AsyncListDifferDelegationAdapter
import org.koin.core.component.KoinComponent
import org.koin.core.component.inject

class CertificateStopAdapter :
    AsyncListDifferDelegationAdapter<CertificateStopAdapterMarker>(DefaultDiffUtilCallback()),
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
            commonDatePickerDelegate.clickListener = {
                clickListener?.onDatePickerClicked(it)
            }
            commonSpinnerDelegate.clickListener = { model, anchor ->
                clickListener?.onSpinnerClicked(model, anchor)
            }
            commonButtonDelegate.clickListener = {
                clickListener?.onButtonClicked(it)
            }
            commonDialogWithSearchDelegate.clickListener = {
                clickListener?.onDialogWithSearchClicked(it)
            }
        }

    var recyclerViewProvider: (() -> RecyclerView?)? = null
        set(value) {
            field = value
            commonEditTextDelegate.recyclerViewProvider = value
        }

    private val commonTitleDelegate: CommonTitleDelegate by inject()
    private val commonButtonDelegate: CommonButtonDelegate by inject()
    private val commonSpinnerDelegate: CommonSpinnerDelegate by inject()
    private val commonEditTextDelegate: CommonEditTextDelegate by inject()
    private val commonSeparatorDelegate: CommonSeparatorDelegate by inject()
    private val commonTextFieldDelegate: CommonTextFieldDelegate by inject()
    private val commonDatePickerDelegate: CommonDatePickerDelegate by inject()
    private val commonTitleSmallDelegate: CommonTitleSmallDelegate by inject()
    private val commonDialogWithSearchDelegate: CommonDialogWithSearchDelegate by inject()
    private val commonButtonTransparentDelegate: CommonButtonTransparentDelegate by inject()

    init {
        items = mutableListOf()
        @Suppress("UNCHECKED_CAST")
        delegatesManager.apply {
            addDelegate(commonTitleDelegate as AdapterDelegate<MutableList<CertificateStopAdapterMarker>>)
            addDelegate(commonButtonDelegate as AdapterDelegate<MutableList<CertificateStopAdapterMarker>>)
            addDelegate(commonSpinnerDelegate as AdapterDelegate<MutableList<CertificateStopAdapterMarker>>)
            addDelegate(commonEditTextDelegate as AdapterDelegate<MutableList<CertificateStopAdapterMarker>>)
            addDelegate(commonSeparatorDelegate as AdapterDelegate<MutableList<CertificateStopAdapterMarker>>)
            addDelegate(commonTextFieldDelegate as AdapterDelegate<MutableList<CertificateStopAdapterMarker>>)
            addDelegate(commonTitleSmallDelegate as AdapterDelegate<MutableList<CertificateStopAdapterMarker>>)
            addDelegate(commonDatePickerDelegate as AdapterDelegate<MutableList<CertificateStopAdapterMarker>>)
            addDelegate(commonDialogWithSearchDelegate as AdapterDelegate<MutableList<CertificateStopAdapterMarker>>)
            addDelegate(commonButtonTransparentDelegate as AdapterDelegate<MutableList<CertificateStopAdapterMarker>>)
        }
    }

    interface ClickListener {
        fun onButtonClicked(model: CommonButtonUi)
        fun onEditTextFocusChanged(model: CommonEditTextUi)
        fun onEditTextDone(model: CommonEditTextUi)
        fun onEditTextChanged(model: CommonEditTextUi)
        fun onDatePickerClicked(model: CommonDatePickerUi)
        fun onSpinnerClicked(model: CommonSpinnerUi, anchor: View)
        fun onDialogWithSearchClicked(model: CommonDialogWithSearchUi)
    }

}