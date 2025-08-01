package com.digitall.eid.ui.fragments.electronic.delivery.system

import com.digitall.eid.R
import com.digitall.eid.domain.APPLICATION_LANGUAGE
import com.digitall.eid.ui.fragments.base.BaseWebViewFragment
import org.koin.androidx.viewmodel.ext.android.viewModel

class ElectronicDeliverySystemFragment : BaseWebViewFragment<ElectronicDeliverySystemViewModel>() {

    companion object {
        private const val TAG = "ElectronicDeliverySystemFragmentTag"
        private const val URL_TO_LOAD = "https://edelivery.egov.bg"
    }

    override val viewModel: ElectronicDeliverySystemViewModel by viewModel()

    override val showToolbar: Boolean = true

    override val showSettingsButton: Boolean = false

    override val toolbarNavigationIconRes: Int = R.drawable.ic_arrow_left

    override val toolbarNavigationTextRes: Int = R.string.tab_more_electronic_delivery_system

    override fun onCreated() {
        super.onCreated()
        val language = APPLICATION_LANGUAGE.type
        loadWebPage("${URL_TO_LOAD}?lang=$language")
    }
}