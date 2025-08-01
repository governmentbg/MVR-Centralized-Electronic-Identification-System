/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.start.splash

import com.digitall.eid.BuildConfig
import com.digitall.eid.NavActivityDirections
import com.digitall.eid.domain.DEBUG_LOGOUT_FROM_PREFERENCES
import com.digitall.eid.domain.DEBUG_PRINT_PREFERENCES_INFO
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.ui.BaseViewModel
import kotlin.system.exitProcess

class SplashViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "SplashViewModelTag"
        private const val PREFERENCES_INFO_TAG = "PreferencesInfoTag"
    }

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        exitProcess(0)
    }

    override fun onFirstAttach() {
        logDebug("onFirstAttach", TAG)
        if (BuildConfig.DEBUG && DEBUG_LOGOUT_FROM_PREFERENCES) {
            preferences.logoutFromPreferences()
            logDebug("logoutFromPreferences", TAG)
        }
        if (BuildConfig.DEBUG && DEBUG_PRINT_PREFERENCES_INFO) {
            logDebug("PRINT_PREFERENCES_INFO", TAG)
            val applicationInfo = preferences.readApplicationInfo()
            logDebug("access token:\n${applicationInfo?.accessToken}\n", PREFERENCES_INFO_TAG)
            logDebug(
                "applicationPin:\n${preferences.readApplicationInfo()?.applicationPin}\n",
                PREFERENCES_INFO_TAG
            )
        }
        navigateInActivity(NavActivityDirections.toAuthFlowFragment())
    }
}