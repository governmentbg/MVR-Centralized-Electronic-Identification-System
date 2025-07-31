package com.digitall.eid.ui.fragments.citizen.information.flow

import com.digitall.eid.R
import com.digitall.eid.models.common.StartDestination
import com.digitall.eid.ui.fragments.base.flow.BaseFlowViewModel

class CitizenInformationFlowViewModel: BaseFlowViewModel() {

    companion object {
        private const val TAG = "CitizenInformationFlowViewModelTag"
    }

    fun getStartDestination() = StartDestination(R.id.citizenInformationFragment)

}