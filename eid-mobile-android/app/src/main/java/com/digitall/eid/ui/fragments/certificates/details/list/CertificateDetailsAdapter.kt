/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.certificates.details.list

import com.digitall.eid.models.certificates.details.CertificateDetailsAdapterMarker
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonSimpleTextInFieldUi
import com.digitall.eid.models.list.CommonSimpleTextUi
import com.digitall.eid.ui.common.list.CommonButtonDelegate
import com.digitall.eid.ui.common.list.CommonButtonTransparentDelegate
import com.digitall.eid.ui.common.list.CommonCheckBoxDelegate
import com.digitall.eid.ui.common.list.CommonDatePickerDelegate
import com.digitall.eid.ui.common.list.CommonDialogWithSearchDelegate
import com.digitall.eid.ui.common.list.CommonDoubleButtonDelegate
import com.digitall.eid.ui.common.list.CommonEditTextDelegate
import com.digitall.eid.ui.common.list.CommonEmptySpaceDelegate
import com.digitall.eid.ui.common.list.CommonSeparatorDelegate
import com.digitall.eid.ui.common.list.CommonSeparatorInFieldDelegate
import com.digitall.eid.ui.common.list.CommonSimpleInFieldTextDelegate
import com.digitall.eid.ui.common.list.CommonSimpleTextDelegate
import com.digitall.eid.ui.common.list.CommonSimpleTextExpiringDelegate
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

class CertificateDetailsAdapter :
    AsyncListDifferDelegationAdapter<CertificateDetailsAdapterMarker>(DefaultDiffUtilCallback()),
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
    private val commonDialogWithSearchDelegate: CommonDialogWithSearchDelegate by inject()
    private val commonButtonTransparentDelegate: CommonButtonTransparentDelegate by inject()
    private val commonSimpleInFieldTextDelegate: CommonSimpleInFieldTextDelegate by inject()
    private val commonTitleSmallInFieldDelegate: CommonTitleSmallInFieldDelegate by inject()
    private val commonSeparatorInFieldDelegate: CommonSeparatorInFieldDelegate by inject()
    private val commonSimpleTextExpiringDelegate: CommonSimpleTextExpiringDelegate by inject()

    var clickListener: ClickListener? = null
        set(value) {
            field = value
            commonButtonDelegate.clickListener = {
                clickListener?.onButtonClicked(it)
            }
            commonSimpleInFieldTextDelegate.clickListener = {
                clickListener?.onInFieldTextClicked(it)
            }
            commonSimpleTextDelegate.actionListener = {
                clickListener?.onFieldTextAction(it)
            }
        }

    init {
        items = mutableListOf()
        @Suppress("UNCHECKED_CAST")
        delegatesManager.apply {
            addDelegate(commonTitleDelegate as AdapterDelegate<MutableList<CertificateDetailsAdapterMarker>>)
            addDelegate(commonButtonDelegate as AdapterDelegate<MutableList<CertificateDetailsAdapterMarker>>)
            addDelegate(commonSpinnerDelegate as AdapterDelegate<MutableList<CertificateDetailsAdapterMarker>>)
            addDelegate(commonEditTextDelegate as AdapterDelegate<MutableList<CertificateDetailsAdapterMarker>>)
            addDelegate(commonCheckBoxDelegate as AdapterDelegate<MutableList<CertificateDetailsAdapterMarker>>)
            addDelegate(commonTextFieldDelegate as AdapterDelegate<MutableList<CertificateDetailsAdapterMarker>>)
            addDelegate(commonSeparatorDelegate as AdapterDelegate<MutableList<CertificateDetailsAdapterMarker>>)
            addDelegate(commonEmptySpaceDelegate as AdapterDelegate<MutableList<CertificateDetailsAdapterMarker>>)
            addDelegate(commonDatePickerDelegate as AdapterDelegate<MutableList<CertificateDetailsAdapterMarker>>)
            addDelegate(commonSimpleTextDelegate as AdapterDelegate<MutableList<CertificateDetailsAdapterMarker>>)
            addDelegate(commonTitleSmallDelegate as AdapterDelegate<MutableList<CertificateDetailsAdapterMarker>>)
            addDelegate(commonDoubleButtonDelegate as AdapterDelegate<MutableList<CertificateDetailsAdapterMarker>>)
            addDelegate(commonDialogWithSearchDelegate as AdapterDelegate<MutableList<CertificateDetailsAdapterMarker>>)
            addDelegate(commonButtonTransparentDelegate as AdapterDelegate<MutableList<CertificateDetailsAdapterMarker>>)
            addDelegate(commonSimpleInFieldTextDelegate as AdapterDelegate<MutableList<CertificateDetailsAdapterMarker>>)
            addDelegate(commonTitleSmallInFieldDelegate as AdapterDelegate<MutableList<CertificateDetailsAdapterMarker>>)
            addDelegate(commonSeparatorInFieldDelegate as AdapterDelegate<MutableList<CertificateDetailsAdapterMarker>>)
            addDelegate(commonSimpleTextExpiringDelegate as AdapterDelegate<MutableList<CertificateDetailsAdapterMarker>>)
        }
    }

    interface ClickListener {
        fun onInFieldTextClicked(model: CommonSimpleTextInFieldUi)
        fun onFieldTextAction(model: CommonSimpleTextUi)
        fun onButtonClicked(model: CommonButtonUi)
    }

}