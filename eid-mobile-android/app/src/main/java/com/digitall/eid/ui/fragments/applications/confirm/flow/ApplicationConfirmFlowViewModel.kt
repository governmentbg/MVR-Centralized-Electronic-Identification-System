/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.applications.confirm.flow

import androidx.core.os.bundleOf
import com.digitall.eid.R
import com.digitall.eid.models.common.StartDestination
import com.digitall.eid.ui.fragments.base.flow.BaseFlowViewModel

class ApplicationConfirmFlowViewModel : BaseFlowViewModel() {

    companion object {
        private const val TAG = "ApplicationConfirmFlowViewModelTag"
    }

    fun getStartDestination(qrCode: String?): StartDestination {
        return StartDestination(
            destination = R.id.applicationConfirmIntroFragment,
            arguments = bundleOf(
                "qrCode" to qrCode
            )
        )
    }

}