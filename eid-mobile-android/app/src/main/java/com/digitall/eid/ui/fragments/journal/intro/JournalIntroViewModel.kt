/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.journal.intro

import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.models.main.MainTabsEnum
import com.digitall.eid.ui.BaseViewModel
import com.digitall.eid.ui.fragments.main.tabs.more.MainTabMoreFragmentDirections

class JournalIntroViewModel: BaseViewModel() {

    companion object {
        private const val TAG = "JournalIntroViewModelTag"
    }

    override var mainTabsEnum: MainTabsEnum? = MainTabsEnum.TAB_MORE

    fun toJournalFromMe() {
        logDebug("toJournalFromMe", TAG)
        navigateInTab(MainTabMoreFragmentDirections.toJournalFromMeFlowFragment())
    }

    fun toJournalToMe() {
        logDebug("toJournalToMe", TAG)
        navigateInTab(MainTabMoreFragmentDirections.toJournalToMeFlowFragment())
    }

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        backToTab()
    }
}