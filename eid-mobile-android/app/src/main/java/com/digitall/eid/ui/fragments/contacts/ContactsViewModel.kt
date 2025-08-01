package com.digitall.eid.ui.fragments.contacts

import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.models.main.MainTabsEnum
import com.digitall.eid.ui.BaseViewModel

class ContactsViewModel: BaseViewModel() {

    companion object {
        private const val TAG = "ContactsViewModelTag"
    }

    override var mainTabsEnum: MainTabsEnum? = MainTabsEnum.TAB_MORE

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        backToTab()
    }
}