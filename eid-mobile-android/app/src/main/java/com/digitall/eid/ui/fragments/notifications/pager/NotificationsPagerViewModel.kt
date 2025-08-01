/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.notifications.pager

import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.models.main.MainTabsEnum
import com.digitall.eid.ui.BaseViewModel

class NotificationsPagerViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "NotificationsPagerViewModelTag"
    }

    override var mainTabsEnum: MainTabsEnum? = MainTabsEnum.TAB_MORE

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        backToTab()
    }

}