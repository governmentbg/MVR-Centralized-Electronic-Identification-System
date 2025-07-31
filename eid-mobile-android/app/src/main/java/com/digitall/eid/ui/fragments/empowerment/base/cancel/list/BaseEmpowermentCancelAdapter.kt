/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.empowerment.base.cancel.list

import android.view.View
import com.digitall.eid.models.empowerment.common.cancel.EmpowermentCancelAdapterMarker
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonEditTextUi
import com.digitall.eid.models.list.CommonSpinnerUi
import com.digitall.eid.ui.common.list.CommonButtonDelegate
import com.digitall.eid.ui.common.list.CommonButtonTransparentDelegate
import com.digitall.eid.ui.common.list.CommonCheckBoxDelegate
import com.digitall.eid.ui.common.list.CommonDatePickerDelegate
import com.digitall.eid.ui.common.list.CommonDialogWithSearchDelegate
import com.digitall.eid.ui.common.list.CommonDoubleButtonDelegate
import com.digitall.eid.ui.common.list.CommonEditTextDelegate
import com.digitall.eid.ui.common.list.CommonSeparatorDelegate
import com.digitall.eid.ui.common.list.CommonSimpleInFieldTextDelegate
import com.digitall.eid.ui.common.list.CommonSimpleTextDelegate
import com.digitall.eid.ui.common.list.CommonSpinnerDelegate
import com.digitall.eid.ui.common.list.CommonTextFieldDelegate
import com.digitall.eid.ui.common.list.CommonTitleDelegate
import com.digitall.eid.ui.common.list.CommonTitleSmallDelegate
import com.digitall.eid.ui.common.list.CommonTitleSmallInFieldDelegate
import com.digitall.eid.utils.DefaultDiffUtilCallback
import com.hannesdorfmann.adapterdelegates4.AdapterDelegate
import com.hannesdorfmann.adapterdelegates4.AsyncListDifferDelegationAdapter
import org.koin.core.component.KoinComponent
import org.koin.core.component.inject

abstract class BaseEmpowermentCancelAdapter :
    AsyncListDifferDelegationAdapter<EmpowermentCancelAdapterMarker>(DefaultDiffUtilCallback()),
    KoinComponent {

    private val commonTitleDelegate: CommonTitleDelegate by inject()
    private val commonButtonDelegate: CommonButtonDelegate by inject()
    private val commonSpinnerDelegate: CommonSpinnerDelegate by inject()
    private val commonCheckBoxDelegate: CommonCheckBoxDelegate by inject()
    private val commonEditTextDelegate: CommonEditTextDelegate by inject()
    private val commonTextFieldDelegate: CommonTextFieldDelegate by inject()
    private val commonSeparatorDelegate: CommonSeparatorDelegate by inject()
    private val commonSimpleTextDelegate: CommonSimpleTextDelegate by inject()
    private val commonTitleSmallDelegate: CommonTitleSmallDelegate by inject()
    private val commonDatePickerDelegate: CommonDatePickerDelegate by inject()
    private val commonDoubleButtonDelegate: CommonDoubleButtonDelegate by inject()
    private val commonDialogWithSearchDelegate: CommonDialogWithSearchDelegate by inject()
    private val commonButtonTransparentDelegate: CommonButtonTransparentDelegate by inject()
    private val commonSimpleInFieldTextDelegate: CommonSimpleInFieldTextDelegate by inject()
    private val commonTitleSmallInFieldDelegate: CommonTitleSmallInFieldDelegate by inject()

    var clickListener: ClickListener? = null
        set(value) {
            field = value
            commonSpinnerDelegate.clickListener = { model, anchor ->
                clickListener?.onSpinnerClicked(
                    model = model,
                    anchor = anchor,
                )
            }
            commonButtonDelegate.clickListener = {
                clickListener?.onButtonClicked(it)
            }
            commonEditTextDelegate.changeListener = {
                clickListener?.onEditTextChanged(it)
            }
            commonEditTextDelegate.doneListener = {
                clickListener?.onEnterTextDone(it)
            }
            commonEditTextDelegate.focusChangedListener = {
                clickListener?.onFocusChanged(it)
            }
        }

    init {
        items = mutableListOf()
        @Suppress("UNCHECKED_CAST")
        delegatesManager.apply {
            addDelegate(commonTitleDelegate as AdapterDelegate<MutableList<EmpowermentCancelAdapterMarker>>)
            addDelegate(commonButtonDelegate as AdapterDelegate<MutableList<EmpowermentCancelAdapterMarker>>)
            addDelegate(commonSpinnerDelegate as AdapterDelegate<MutableList<EmpowermentCancelAdapterMarker>>)
            addDelegate(commonEditTextDelegate as AdapterDelegate<MutableList<EmpowermentCancelAdapterMarker>>)
            addDelegate(commonCheckBoxDelegate as AdapterDelegate<MutableList<EmpowermentCancelAdapterMarker>>)
            addDelegate(commonTextFieldDelegate as AdapterDelegate<MutableList<EmpowermentCancelAdapterMarker>>)
            addDelegate(commonSeparatorDelegate as AdapterDelegate<MutableList<EmpowermentCancelAdapterMarker>>)
            addDelegate(commonDatePickerDelegate as AdapterDelegate<MutableList<EmpowermentCancelAdapterMarker>>)
            addDelegate(commonSimpleTextDelegate as AdapterDelegate<MutableList<EmpowermentCancelAdapterMarker>>)
            addDelegate(commonTitleSmallDelegate as AdapterDelegate<MutableList<EmpowermentCancelAdapterMarker>>)
            addDelegate(commonDoubleButtonDelegate as AdapterDelegate<MutableList<EmpowermentCancelAdapterMarker>>)
            addDelegate(commonDialogWithSearchDelegate as AdapterDelegate<MutableList<EmpowermentCancelAdapterMarker>>)
            addDelegate(commonButtonTransparentDelegate as AdapterDelegate<MutableList<EmpowermentCancelAdapterMarker>>)
            addDelegate(commonSimpleInFieldTextDelegate as AdapterDelegate<MutableList<EmpowermentCancelAdapterMarker>>)
            addDelegate(commonTitleSmallInFieldDelegate as AdapterDelegate<MutableList<EmpowermentCancelAdapterMarker>>)
        }
    }

    interface ClickListener {
        fun onButtonClicked(model: CommonButtonUi)
        fun onFocusChanged(model: CommonEditTextUi)
        fun onEnterTextDone(model: CommonEditTextUi)
        fun onEditTextChanged(model: CommonEditTextUi)
        fun onSpinnerClicked(model: CommonSpinnerUi, anchor: View)
    }

}