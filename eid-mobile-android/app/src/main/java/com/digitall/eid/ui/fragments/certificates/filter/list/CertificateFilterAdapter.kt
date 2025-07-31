/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.certificates.filter.list

import android.view.View
import com.digitall.eid.models.certificates.filter.CertificateFilterAdapterMarker
import com.digitall.eid.models.list.CommonDatePickerUi
import com.digitall.eid.models.list.CommonDialogWithSearchUi
import com.digitall.eid.models.list.CommonEditTextUi
import com.digitall.eid.models.list.CommonSpinnerUi
import com.digitall.eid.ui.common.list.CommonDatePickerDelegate
import com.digitall.eid.ui.common.list.CommonDialogWithSearchDelegate
import com.digitall.eid.ui.common.list.CommonDoubleButtonDelegate
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

class CertificateFilterAdapter :
    AsyncListDifferDelegationAdapter<CertificateFilterAdapterMarker>(DefaultDiffUtilCallback()),
    KoinComponent {

    private val commonTitleDelegate: CommonTitleDelegate by inject()
    private val commonSpinnerDelegate: CommonSpinnerDelegate by inject()
    private val commonEditTextDelegate: CommonEditTextDelegate by inject()
    private val commonSeparatorDelegate: CommonSeparatorDelegate by inject()
    private val commonTextFieldDelegate: CommonTextFieldDelegate by inject()
    private val commonTitleSmallDelegate: CommonTitleSmallDelegate by inject()
    private val commonDatePickerDelegate: CommonDatePickerDelegate by inject()
    private val commonDoubleButtonDelegate: CommonDoubleButtonDelegate by inject()
    private val commonDialogWithSearchDelegate: CommonDialogWithSearchDelegate by inject()

    var clickListener: ClickListener? = null
        set(value) {
            field = value
            commonEditTextDelegate.changeListener = {
                clickListener?.onEditTextChanged(it)
            }
            commonEditTextDelegate.doneListener = {
                clickListener?.onEnterTextDone(it)
            }
            commonSpinnerDelegate.clickListener = { model, anchor ->
                clickListener?.onSpinnerClicked(
                    model = model,
                    anchor = anchor,
                )
            }
            commonDatePickerDelegate.clickListener = {
                clickListener?.onDatePickerClicked(it)
            }
            commonDialogWithSearchDelegate.clickListener = {
                clickListener?.onDialogWithSearchClicked(it)
            }
            commonEditTextDelegate.focusChangedListener = {
                clickListener?.onFocusChanged(it)
            }
        }

    init {
        items = mutableListOf()
        @Suppress("UNCHECKED_CAST")
        delegatesManager.apply {
            addDelegate(commonTitleDelegate as AdapterDelegate<MutableList<CertificateFilterAdapterMarker>>)
            addDelegate(commonSpinnerDelegate as AdapterDelegate<MutableList<CertificateFilterAdapterMarker>>)
            addDelegate(commonEditTextDelegate as AdapterDelegate<MutableList<CertificateFilterAdapterMarker>>)
            addDelegate(commonSeparatorDelegate as AdapterDelegate<MutableList<CertificateFilterAdapterMarker>>)
            addDelegate(commonTitleSmallDelegate as AdapterDelegate<MutableList<CertificateFilterAdapterMarker>>)
            addDelegate(commonTextFieldDelegate as AdapterDelegate<MutableList<CertificateFilterAdapterMarker>>)
            addDelegate(commonDatePickerDelegate as AdapterDelegate<MutableList<CertificateFilterAdapterMarker>>)
            addDelegate(commonDoubleButtonDelegate as AdapterDelegate<MutableList<CertificateFilterAdapterMarker>>)
            addDelegate(commonDialogWithSearchDelegate as AdapterDelegate<MutableList<CertificateFilterAdapterMarker>>)
        }
    }

    interface ClickListener {
        fun onFocusChanged(model: CommonEditTextUi)
        fun onEnterTextDone(model: CommonEditTextUi)
        fun onEditTextChanged(model: CommonEditTextUi)
        fun onDatePickerClicked(model: CommonDatePickerUi)
        fun onDialogWithSearchClicked(model: CommonDialogWithSearchUi)
        fun onSpinnerClicked(model: CommonSpinnerUi, anchor: View)
    }

}