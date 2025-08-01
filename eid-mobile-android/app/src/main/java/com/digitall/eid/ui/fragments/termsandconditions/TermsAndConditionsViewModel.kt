package com.digitall.eid.ui.fragments.termsandconditions

import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.models.main.MainTabsEnum
import com.digitall.eid.ui.BaseViewModel

class TermsAndConditionsViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "TermsAndConditionsViewModelTag"
    }

    override var mainTabsEnum: MainTabsEnum? = MainTabsEnum.TAB_MORE

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        backToTab()
    }
}