package com.digitall.eid.ui.fragments.centers.certification.services

import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.models.main.MainTabsEnum
import com.digitall.eid.ui.BaseViewModel

class CentersCertificationServicesViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "CentersCertificationServicesViewModelTag"
    }

    override var mainTabsEnum: MainTabsEnum? = MainTabsEnum.TAB_MORE

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        backToTab()
    }
}