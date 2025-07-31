/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.empowerment.base.filter.list

import android.view.View
import androidx.recyclerview.widget.RecyclerView
import com.digitall.eid.models.empowerment.common.filter.EmpowermentFilterAdapterMarker
import com.digitall.eid.models.list.CommonButtonTransparentUi
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonCheckBoxUi
import com.digitall.eid.models.list.CommonDatePickerUi
import com.digitall.eid.models.list.CommonDialogWithSearchUi
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
import com.digitall.eid.ui.common.list.CommonSpinnerDelegate
import com.digitall.eid.ui.common.list.CommonTextFieldDelegate
import com.digitall.eid.ui.common.list.CommonTitleDelegate
import com.digitall.eid.ui.common.list.CommonTitleSmallDelegate
import com.digitall.eid.utils.DefaultDiffUtilCallback
import com.hannesdorfmann.adapterdelegates4.AdapterDelegate
import com.hannesdorfmann.adapterdelegates4.AsyncListDifferDelegationAdapter
import org.koin.core.component.KoinComponent
import org.koin.core.component.inject

abstract class BaseEmpowermentFilterAdapter :
    AsyncListDifferDelegationAdapter<EmpowermentFilterAdapterMarker>(DefaultDiffUtilCallback()),
    KoinComponent {

    private val commonTitleDelegate: CommonTitleDelegate by inject()
    private val commonButtonDelegate: CommonButtonDelegate by inject()
    private val commonSpinnerDelegate: CommonSpinnerDelegate by inject()
    private val commonCheckBoxDelegate: CommonCheckBoxDelegate by inject()
    private val commonEditTextDelegate: CommonEditTextDelegate by inject()
    private val commonSeparatorDelegate: CommonSeparatorDelegate by inject()
    private val commonTextFieldDelegate: CommonTextFieldDelegate by inject()
    private val commonTitleSmallDelegate: CommonTitleSmallDelegate by inject()
    private val commonDatePickerDelegate: CommonDatePickerDelegate by inject()
    private val commonDoubleButtonDelegate: CommonDoubleButtonDelegate by inject()
    private val commonDialogWithSearchDelegate: CommonDialogWithSearchDelegate by inject()
    private val commonButtonTransparentDelegate: CommonButtonTransparentDelegate by inject()

    var clickListener: ClickListener? = null
        set(value) {
            field = value
            commonEditTextDelegate.changeListener = {
                clickListener?.onEditTextChanged(it)
            }
            commonEditTextDelegate.eraseClickListener = {
                clickListener?.onEraseClicked(it)
            }
            commonEditTextDelegate.doneListener = {
                clickListener?.onEnterTextDone(it)
            }
            commonButtonTransparentDelegate.clickListener = {
                clickListener?.onButtonTransparentClicked(it)
            }
            commonButtonDelegate.clickListener = {
                clickListener?.onButtonClicked(it)
            }
            commonCheckBoxDelegate.changeListener = {
                clickListener?.onCheckBoxChangeState(it)
            }
            commonSpinnerDelegate.clickListener = { model, anchor ->
                clickListener?.onSpinnerClicked(
                    model = model,
                    anchor = anchor,
                )
            }
            commonSpinnerDelegate.eraseClickListener = {
                clickListener?.onEraseClicked(it)
            }
            commonDatePickerDelegate.clickListener = {
                clickListener?.onDatePickerClicked(it)
            }
            commonDatePickerDelegate.eraseClickListener = {
                clickListener?.onEraseClicked(it)
            }
            commonDialogWithSearchDelegate.clickListener = {
                clickListener?.onDialogWithSearchClicked(it)
            }
            commonDialogWithSearchDelegate.eraseClickListener = {
                clickListener?.onEraseClicked(it)
            }
            commonEditTextDelegate.focusChangedListener = {
                clickListener?.onFocusChanged(it)
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
            addDelegate(commonTitleDelegate as AdapterDelegate<MutableList<EmpowermentFilterAdapterMarker>>)
            addDelegate(commonButtonDelegate as AdapterDelegate<MutableList<EmpowermentFilterAdapterMarker>>)
            addDelegate(commonSpinnerDelegate as AdapterDelegate<MutableList<EmpowermentFilterAdapterMarker>>)
            addDelegate(commonEditTextDelegate as AdapterDelegate<MutableList<EmpowermentFilterAdapterMarker>>)
            addDelegate(commonCheckBoxDelegate as AdapterDelegate<MutableList<EmpowermentFilterAdapterMarker>>)
            addDelegate(commonSeparatorDelegate as AdapterDelegate<MutableList<EmpowermentFilterAdapterMarker>>)
            addDelegate(commonTitleSmallDelegate as AdapterDelegate<MutableList<EmpowermentFilterAdapterMarker>>)
            addDelegate(commonTextFieldDelegate as AdapterDelegate<MutableList<EmpowermentFilterAdapterMarker>>)
            addDelegate(commonDatePickerDelegate as AdapterDelegate<MutableList<EmpowermentFilterAdapterMarker>>)
            addDelegate(commonDoubleButtonDelegate as AdapterDelegate<MutableList<EmpowermentFilterAdapterMarker>>)
            addDelegate(commonDialogWithSearchDelegate as AdapterDelegate<MutableList<EmpowermentFilterAdapterMarker>>)
            addDelegate(commonButtonTransparentDelegate as AdapterDelegate<MutableList<EmpowermentFilterAdapterMarker>>)
        }
    }

    interface ClickListener {
        fun onButtonClicked(model: CommonButtonUi)
        fun onFocusChanged(model: CommonEditTextUi)
        fun onEnterTextDone(model: CommonEditTextUi)
        fun onEditTextChanged(model: CommonEditTextUi)
        fun onDatePickerClicked(model: CommonDatePickerUi)
        fun onDialogWithSearchClicked(model: CommonDialogWithSearchUi)
        fun onEraseClicked(model: EmpowermentFilterAdapterMarker)
        fun onButtonTransparentClicked(model: CommonButtonTransparentUi)
        fun onCheckBoxChangeState(model: CommonCheckBoxUi)
        fun onSpinnerClicked(model: CommonSpinnerUi, anchor: View)
    }

}