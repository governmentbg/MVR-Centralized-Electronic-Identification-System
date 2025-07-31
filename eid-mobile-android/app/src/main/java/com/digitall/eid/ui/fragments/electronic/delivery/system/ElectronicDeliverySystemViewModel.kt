package com.digitall.eid.ui.fragments.electronic.delivery.system

import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.models.main.MainTabsEnum
import com.digitall.eid.ui.BaseViewModel

class ElectronicDeliverySystemViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "ElectronicDeliverySystemViewModelTag"
    }

    override var mainTabsEnum: MainTabsEnum? = MainTabsEnum.TAB_MORE

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        backToTab()
    }
}