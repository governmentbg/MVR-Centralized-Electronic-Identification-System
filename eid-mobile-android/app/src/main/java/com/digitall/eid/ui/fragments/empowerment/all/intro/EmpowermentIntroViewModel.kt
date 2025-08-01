/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.empowerment.all.intro

import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.models.main.MainTabsEnum
import com.digitall.eid.ui.BaseViewModel
import com.digitall.eid.ui.fragments.main.tabs.more.MainTabMoreFragmentDirections

class EmpowermentIntroViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "EmpowermentIntroViewModelTag"
    }

    override var mainTabsEnum: MainTabsEnum? = MainTabsEnum.TAB_MORE

    override fun onFirstAttach() {
        logDebug("onFirstAttach", TAG)
    }

    fun toEmpowermentFromMe() {
        logDebug("toEmpowermentFromMe", TAG)
        navigateInTab(MainTabMoreFragmentDirections.toEmpowermentFromMeFlowFragment())
    }

    fun toEmpowermentToMe() {
        logDebug("toEmpowermentToMe", TAG)
        navigateInTab(MainTabMoreFragmentDirections.toEmpowermentToMeFlowFragment())
    }

    fun toEmpowermentCreate() {
        logDebug("toEmpowermentCreate", TAG)
        navigateInTab(
            MainTabMoreFragmentDirections.toEmpowermentCreateFlowFragment(
                model = null
            )
        )
    }

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        backToTab()
    }

}