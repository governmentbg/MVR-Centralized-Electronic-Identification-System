/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.empowerment.create.preview.list

import com.digitall.eid.models.empowerment.create.preview.EmpowermentCreatePreviewAdapterMarker
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.ui.common.list.CommonButtonDelegate
import com.digitall.eid.ui.common.list.CommonSeparatorDelegate
import com.digitall.eid.ui.common.list.CommonTextFieldDelegate
import com.digitall.eid.ui.common.list.CommonTextFieldMultipleDelegate
import com.digitall.eid.ui.common.list.CommonTitleDelegate
import com.digitall.eid.ui.common.list.CommonTitleSmallDelegate
import com.digitall.eid.utils.DefaultDiffUtilCallback
import com.hannesdorfmann.adapterdelegates4.AdapterDelegate
import com.hannesdorfmann.adapterdelegates4.AsyncListDifferDelegationAdapter
import org.koin.core.component.KoinComponent
import org.koin.core.component.inject

class EmpowermentCreatePreviewAdapter :
    AsyncListDifferDelegationAdapter<EmpowermentCreatePreviewAdapterMarker>(DefaultDiffUtilCallback()),
    KoinComponent {

    private val commonTitleDelegate: CommonTitleDelegate by inject()
    private val commonButtonDelegate: CommonButtonDelegate by inject()
    private val commonSeparatorDelegate: CommonSeparatorDelegate by inject()
    private val commonTextFieldDelegate: CommonTextFieldDelegate by inject()
    private val commonTextFieldMultipleDelegate: CommonTextFieldMultipleDelegate by inject()
    private val commonTitleSmallDelegate: CommonTitleSmallDelegate by inject()

    var clickListener: ClickListener? = null
        set(value) {
            field = value
            commonButtonDelegate.clickListener = { model ->
                clickListener?.onButtonClicked(
                    model = model,
                )
            }
        }

    init {
        items = mutableListOf()
        @Suppress("UNCHECKED_CAST")
        delegatesManager.apply {
            addDelegate(commonTitleDelegate as AdapterDelegate<MutableList<EmpowermentCreatePreviewAdapterMarker>>)
            addDelegate(commonButtonDelegate as AdapterDelegate<MutableList<EmpowermentCreatePreviewAdapterMarker>>)
            addDelegate(commonSeparatorDelegate as AdapterDelegate<MutableList<EmpowermentCreatePreviewAdapterMarker>>)
            addDelegate(commonTextFieldDelegate as AdapterDelegate<MutableList<EmpowermentCreatePreviewAdapterMarker>>)
            addDelegate(commonTextFieldMultipleDelegate as AdapterDelegate<MutableList<EmpowermentCreatePreviewAdapterMarker>>)
            addDelegate(commonTitleSmallDelegate as AdapterDelegate<MutableList<EmpowermentCreatePreviewAdapterMarker>>)
        }
    }

    fun interface ClickListener {
        fun onButtonClicked(model: CommonButtonUi)
    }

}