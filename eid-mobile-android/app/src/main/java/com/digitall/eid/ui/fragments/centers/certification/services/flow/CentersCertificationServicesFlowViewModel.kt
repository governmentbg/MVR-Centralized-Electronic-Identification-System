package com.digitall.eid.ui.fragments.centers.certification.services.flow

import com.digitall.eid.R
import com.digitall.eid.models.common.StartDestination
import com.digitall.eid.ui.fragments.base.flow.BaseFlowViewModel

class CentersCertificationServicesFlowViewModel : BaseFlowViewModel() {

    companion object {
        private const val TAG = "CentersCertificationServicesFlowViewModelTag"
    }

    fun getStartDestination() = StartDestination(R.id.centersCertificationServicesFragment)
}