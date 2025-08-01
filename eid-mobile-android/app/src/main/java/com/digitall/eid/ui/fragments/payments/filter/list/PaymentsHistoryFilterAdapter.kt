package com.digitall.eid.ui.fragments.payments.filter.list

import android.view.View
import com.digitall.eid.models.list.CommonDatePickerUi
import com.digitall.eid.models.list.CommonEditTextUi
import com.digitall.eid.models.list.CommonSpinnerUi
import com.digitall.eid.models.payments.history.filter.PaymentsHistoryFilterAdapterMarker
import com.digitall.eid.ui.common.list.CommonDatePickerDelegate
import com.digitall.eid.ui.common.list.CommonDoubleButtonDelegate
import com.digitall.eid.ui.common.list.CommonEditTextDelegate
import com.digitall.eid.ui.common.list.CommonSeparatorDelegate
import com.digitall.eid.ui.common.list.CommonSpinnerDelegate
import com.digitall.eid.utils.DefaultDiffUtilCallback
import com.hannesdorfmann.adapterdelegates4.AdapterDelegate
import com.hannesdorfmann.adapterdelegates4.AsyncListDifferDelegationAdapter
import org.koin.core.component.KoinComponent
import org.koin.core.component.inject

class PaymentsHistoryFilterAdapter :
    AsyncListDifferDelegationAdapter<PaymentsHistoryFilterAdapterMarker>(DefaultDiffUtilCallback()),
    KoinComponent {

    private val commonSpinnerDelegate: CommonSpinnerDelegate by inject()
    private val commonEditTextDelegate: CommonEditTextDelegate by inject()
    private val commonSeparatorDelegate: CommonSeparatorDelegate by inject()
    private val commonDatePickerDelegate: CommonDatePickerDelegate by inject()
    private val commonDoubleButtonDelegate: CommonDoubleButtonDelegate by inject()

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
            commonEditTextDelegate.focusChangedListener = {
                clickListener?.onFocusChanged(it)
            }
        }

    init {
        items = mutableListOf()
        @Suppress("UNCHECKED_CAST")
        delegatesManager.apply {
            addDelegate(commonSpinnerDelegate as AdapterDelegate<MutableList<PaymentsHistoryFilterAdapterMarker>>)
            addDelegate(commonEditTextDelegate as AdapterDelegate<MutableList<PaymentsHistoryFilterAdapterMarker>>)
            addDelegate(commonSeparatorDelegate as AdapterDelegate<MutableList<PaymentsHistoryFilterAdapterMarker>>)
            addDelegate(commonDatePickerDelegate as AdapterDelegate<MutableList<PaymentsHistoryFilterAdapterMarker>>)
            addDelegate(commonDoubleButtonDelegate as AdapterDelegate<MutableList<PaymentsHistoryFilterAdapterMarker>>)
        }
    }

    interface ClickListener {
        fun onFocusChanged(model: CommonEditTextUi)
        fun onEnterTextDone(model: CommonEditTextUi)
        fun onEditTextChanged(model: CommonEditTextUi)
        fun onDatePickerClicked(model: CommonDatePickerUi)
        fun onSpinnerClicked(model: CommonSpinnerUi, anchor: View)
    }

}