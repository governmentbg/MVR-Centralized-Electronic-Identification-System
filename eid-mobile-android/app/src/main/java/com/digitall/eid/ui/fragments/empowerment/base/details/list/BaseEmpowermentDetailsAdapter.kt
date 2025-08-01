/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.empowerment.base.details.list

import com.digitall.eid.models.empowerment.common.details.EmpowermentDetailsAdapterMarker
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.ui.common.list.CommonButtonDelegate
import com.digitall.eid.ui.common.list.CommonButtonTransparentDelegate
import com.digitall.eid.ui.common.list.CommonCheckBoxDelegate
import com.digitall.eid.ui.common.list.CommonDatePickerDelegate
import com.digitall.eid.ui.common.list.CommonDialogWithSearchDelegate
import com.digitall.eid.ui.common.list.CommonDoubleButtonDelegate
import com.digitall.eid.ui.common.list.CommonEditTextDelegate
import com.digitall.eid.ui.common.list.CommonEmptySpaceDelegate
import com.digitall.eid.ui.common.list.CommonLabeledSimpleTextDelegate
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

abstract class BaseEmpowermentDetailsAdapter :
    AsyncListDifferDelegationAdapter<EmpowermentDetailsAdapterMarker>(DefaultDiffUtilCallback()),
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
    private val commonEmptySpaceDelegate: CommonEmptySpaceDelegate by inject()
    private val commonDoubleButtonDelegate: CommonDoubleButtonDelegate by inject()
    private val commonLabeledSimpleTextDelegate: CommonLabeledSimpleTextDelegate by inject()
    private val commonDialogWithSearchDelegate: CommonDialogWithSearchDelegate by inject()
    private val commonButtonTransparentDelegate: CommonButtonTransparentDelegate by inject()
    private val commonSimpleInFieldTextDelegate: CommonSimpleInFieldTextDelegate by inject()
    private val commonTitleSmallInFieldDelegate: CommonTitleSmallInFieldDelegate by inject()

    var clickListener: ClickListener? = null
        set(value) {
            field = value
            commonButtonDelegate.clickListener = {
                clickListener?.onButtonClicked(it)
            }
        }

    init {
        items = mutableListOf()
        @Suppress("UNCHECKED_CAST")
        delegatesManager.apply {
            addDelegate(commonTitleDelegate as AdapterDelegate<MutableList<EmpowermentDetailsAdapterMarker>>)
            addDelegate(commonButtonDelegate as AdapterDelegate<MutableList<EmpowermentDetailsAdapterMarker>>)
            addDelegate(commonSpinnerDelegate as AdapterDelegate<MutableList<EmpowermentDetailsAdapterMarker>>)
            addDelegate(commonEditTextDelegate as AdapterDelegate<MutableList<EmpowermentDetailsAdapterMarker>>)
            addDelegate(commonCheckBoxDelegate as AdapterDelegate<MutableList<EmpowermentDetailsAdapterMarker>>)
            addDelegate(commonTextFieldDelegate as AdapterDelegate<MutableList<EmpowermentDetailsAdapterMarker>>)
            addDelegate(commonSeparatorDelegate as AdapterDelegate<MutableList<EmpowermentDetailsAdapterMarker>>)
            addDelegate(commonEmptySpaceDelegate as AdapterDelegate<MutableList<EmpowermentDetailsAdapterMarker>>)
            addDelegate(commonDatePickerDelegate as AdapterDelegate<MutableList<EmpowermentDetailsAdapterMarker>>)
            addDelegate(commonSimpleTextDelegate as AdapterDelegate<MutableList<EmpowermentDetailsAdapterMarker>>)
            addDelegate(commonTitleSmallDelegate as AdapterDelegate<MutableList<EmpowermentDetailsAdapterMarker>>)
            addDelegate(commonDoubleButtonDelegate as AdapterDelegate<MutableList<EmpowermentDetailsAdapterMarker>>)
            addDelegate(commonDialogWithSearchDelegate as AdapterDelegate<MutableList<EmpowermentDetailsAdapterMarker>>)
            addDelegate(commonButtonTransparentDelegate as AdapterDelegate<MutableList<EmpowermentDetailsAdapterMarker>>)
            addDelegate(commonSimpleInFieldTextDelegate as AdapterDelegate<MutableList<EmpowermentDetailsAdapterMarker>>)
            addDelegate(commonTitleSmallInFieldDelegate as AdapterDelegate<MutableList<EmpowermentDetailsAdapterMarker>>)
            addDelegate(commonLabeledSimpleTextDelegate as AdapterDelegate<MutableList<EmpowermentDetailsAdapterMarker>>)
        }
    }

    fun interface ClickListener {
        fun onButtonClicked(model: CommonButtonUi)
    }

}