/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.empowerment.from.me.signing.list

import android.view.View
import com.digitall.eid.models.empowerment.signing.EmpowermentFromMeSigningAdapterMarker
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonSpinnerUi
import com.digitall.eid.ui.common.list.CommonButtonDelegate
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

class EmpowermentFromMeSigningAdapter :
    AsyncListDifferDelegationAdapter<EmpowermentFromMeSigningAdapterMarker>(DefaultDiffUtilCallback()),
    KoinComponent {

    private val commonTitleDelegate: CommonTitleDelegate by inject()
    private val commonButtonDelegate: CommonButtonDelegate by inject()
    private val commonSpinnerDelegate: CommonSpinnerDelegate by inject()
    private val commonSeparatorDelegate: CommonSeparatorDelegate by inject()
    private val commonTextFieldDelegate: CommonTextFieldDelegate by inject()
    private val commonSimpleTextDelegate: CommonSimpleTextDelegate by inject()
    private val commonTitleSmallDelegate: CommonTitleSmallDelegate by inject()
    private val commonLabeledSimpleTextDelegate: CommonLabeledSimpleTextDelegate by inject()
    private val commonTitleSmallInFieldDelegate: CommonTitleSmallInFieldDelegate by inject()
    private val commonSimpleInFieldTextDelegate: CommonSimpleInFieldTextDelegate by inject()

    var clickListener: ClickListener? = null
        set(value) {
            field = value
            commonButtonDelegate.clickListener = {
                clickListener?.onButtonClicked(it)
            }
            commonSpinnerDelegate.clickListener = { model, anchor ->
                clickListener?.onSpinnerClicked(
                    model = model,
                    anchor = anchor,
                )
            }
        }

    init {
        items = mutableListOf()
        @Suppress("UNCHECKED_CAST")
        delegatesManager.apply {
            addDelegate(commonTitleDelegate as AdapterDelegate<MutableList<EmpowermentFromMeSigningAdapterMarker>>)
            addDelegate(commonButtonDelegate as AdapterDelegate<MutableList<EmpowermentFromMeSigningAdapterMarker>>)
            addDelegate(commonSpinnerDelegate as AdapterDelegate<MutableList<EmpowermentFromMeSigningAdapterMarker>>)
            addDelegate(commonSeparatorDelegate as AdapterDelegate<MutableList<EmpowermentFromMeSigningAdapterMarker>>)
            addDelegate(commonTextFieldDelegate as AdapterDelegate<MutableList<EmpowermentFromMeSigningAdapterMarker>>)
            addDelegate(commonTitleSmallDelegate as AdapterDelegate<MutableList<EmpowermentFromMeSigningAdapterMarker>>)
            addDelegate(commonSimpleTextDelegate as AdapterDelegate<MutableList<EmpowermentFromMeSigningAdapterMarker>>)
            addDelegate(commonSimpleInFieldTextDelegate as AdapterDelegate<MutableList<EmpowermentFromMeSigningAdapterMarker>>)
            addDelegate(commonTitleSmallInFieldDelegate as AdapterDelegate<MutableList<EmpowermentFromMeSigningAdapterMarker>>)
            addDelegate(commonLabeledSimpleTextDelegate as AdapterDelegate<MutableList<EmpowermentFromMeSigningAdapterMarker>>)
        }
    }

    interface ClickListener {
        fun onButtonClicked(model: CommonButtonUi)
        fun onSpinnerClicked(model: CommonSpinnerUi, anchor: View)
    }

}