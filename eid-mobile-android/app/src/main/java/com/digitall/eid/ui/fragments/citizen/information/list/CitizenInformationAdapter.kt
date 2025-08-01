package com.digitall.eid.ui.fragments.citizen.information.list

import com.digitall.eid.models.citizen.information.CitizenInformationAdapterMarker
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonTitleCheckboxUi
import com.digitall.eid.models.list.CommonTitleDescriptionUi
import com.digitall.eid.ui.common.list.CommonButtonDelegate
import com.digitall.eid.ui.common.list.CommonEmptySpaceDelegate
import com.digitall.eid.ui.common.list.CommonSeparatorDelegate
import com.digitall.eid.ui.common.list.CommonTitleCheckboxDelegate
import com.digitall.eid.ui.common.list.CommonTitleDescriptionDelegate
import com.digitall.eid.ui.common.list.CommonTitleSmallDelegate
import com.digitall.eid.utils.DefaultDiffUtilCallback
import com.hannesdorfmann.adapterdelegates4.AdapterDelegate
import com.hannesdorfmann.adapterdelegates4.AsyncListDifferDelegationAdapter
import org.koin.core.component.KoinComponent
import org.koin.core.component.inject

class CitizenInformationAdapter :
    AsyncListDifferDelegationAdapter<CitizenInformationAdapterMarker>(DefaultDiffUtilCallback()),
    KoinComponent {

    private val commonTitleSmallDelegate: CommonTitleSmallDelegate by inject()
    private val commonSeparatorDelegate: CommonSeparatorDelegate by inject()
    private val commonTextDescriptionDelegate: CommonTitleDescriptionDelegate by inject()
    private val commonEmptySpaceDelegate: CommonEmptySpaceDelegate by inject()
    private val commonButtonDelegate: CommonButtonDelegate by inject()
    private val commonTitleCheckboxDelegate: CommonTitleCheckboxDelegate by inject()

    var clickListener: ClickListener? = null
        set(value) {
            field = value
            commonTextDescriptionDelegate.actionListener = {
                clickListener?.onFieldTextAction(it)
            }
            commonButtonDelegate.clickListener = {
                clickListener?.onButtonClicked(it)
            }
            commonTitleCheckboxDelegate.changeListener = {
                clickListener?.onCheckBoxChangeState(it)
            }
        }

    init {
        items = mutableListOf()
        @Suppress("UNCHECKED_CAST")
        delegatesManager.apply {
            addDelegate(commonTitleSmallDelegate as AdapterDelegate<MutableList<CitizenInformationAdapterMarker>>)
            addDelegate(commonSeparatorDelegate as AdapterDelegate<MutableList<CitizenInformationAdapterMarker>>)
            addDelegate(commonTextDescriptionDelegate as AdapterDelegate<MutableList<CitizenInformationAdapterMarker>>)
            addDelegate(commonEmptySpaceDelegate as AdapterDelegate<MutableList<CitizenInformationAdapterMarker>>)
            addDelegate(commonButtonDelegate as AdapterDelegate<MutableList<CitizenInformationAdapterMarker>>)
            addDelegate(commonTitleCheckboxDelegate as AdapterDelegate<MutableList<CitizenInformationAdapterMarker>>)
        }
    }

    interface ClickListener {
        fun onFieldTextAction(model: CommonTitleDescriptionUi)
        fun onButtonClicked(model: CommonButtonUi)
        fun onCheckBoxChangeState(model: CommonTitleCheckboxUi)
    }
}