/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.start.flow

import com.digitall.eid.R
import com.digitall.eid.models.common.StartDestination
import com.digitall.eid.ui.fragments.base.flow.BaseFlowViewModel

class StartFlowViewModel : BaseFlowViewModel() {

    fun getStartDestination(): StartDestination {
        return StartDestination(R.id.splashFragment)
    }

}