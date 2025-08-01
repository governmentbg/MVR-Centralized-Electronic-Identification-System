/**
 * Use single activity
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 */
package com.digitall.eid.ui.activity

import android.content.Intent
import androidx.annotation.CallSuper
import com.digitall.eid.R
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.models.common.StartDestination
import com.digitall.eid.ui.BaseViewModel
import com.digitall.eid.utils.ActivitiesCommonHelper
import org.koin.core.component.inject

class MainViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "BaseActivityViewModelTag"
    }

    private val activitiesCommonHelper: ActivitiesCommonHelper by inject()

    fun getStartDestination(intent: Intent): StartDestination {
        return StartDestination(R.id.startFlowFragment)
    }

    @CallSuper
    override fun onFirstAttach() {
        super.onFirstAttach()
        logDebug("onFirstAttach", TAG)
        activitiesCommonHelper.getFcmToken()
    }

    fun applyLightDarkTheme() {
        activitiesCommonHelper.applyLightDarkTheme()
    }

    fun onResume() {
        inactivityTimer.activityOnResume()
    }

    fun onPause() {
        inactivityTimer.activityOnPause()
    }

    fun onDestroy() {
        inactivityTimer.activityOnDestroy()
    }

    fun dispatchTouchEvent() {
        inactivityTimer.dispatchTouchEvent()
    }

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        if (!findActivityNavController().popBackStack()) {
            closeActivity()
        }
    }

    @CallSuper
    override fun onCleared() {
        super.onCleared()
    }
}