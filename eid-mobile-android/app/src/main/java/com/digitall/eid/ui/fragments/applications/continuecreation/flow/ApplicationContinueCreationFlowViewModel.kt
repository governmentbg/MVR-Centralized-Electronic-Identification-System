/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.applications.continuecreation.flow

import androidx.core.os.bundleOf
import com.digitall.eid.R
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.models.common.StartDestination
import com.digitall.eid.ui.fragments.base.flow.BaseFlowViewModel

class ApplicationContinueCreationFlowViewModel : BaseFlowViewModel() {

    companion object {
        private const val TAG = "ApplicationContinueCreationFlowViewModelTag"
    }

    fun getStartDestination(
        applicationId: String?,
    ): StartDestination {
        logDebug("getStartDestination applicationId: $applicationId", TAG)
        return StartDestination(
            destination = R.id.applicationContinueCreationFragment,
            arguments = bundleOf(
                "applicationId" to applicationId,
            )
        )
    }

}