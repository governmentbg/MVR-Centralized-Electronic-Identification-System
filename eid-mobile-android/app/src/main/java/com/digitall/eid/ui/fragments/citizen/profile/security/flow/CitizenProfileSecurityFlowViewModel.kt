package com.digitall.eid.ui.fragments.citizen.profile.security.flow

import com.digitall.eid.R
import com.digitall.eid.models.common.StartDestination
import com.digitall.eid.ui.fragments.base.flow.BaseFlowViewModel

class CitizenProfileSecurityFlowViewModel: BaseFlowViewModel() {

    companion object {
        private const val TAG = "CitizenProfileSecurityFlowViewModelTag"
    }

    fun getStartDestination() = StartDestination(R.id.citizenProfileSecurityFragment)
}