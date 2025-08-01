package com.digitall.eid.ui.fragments.citizen.change.information.flow

import com.digitall.eid.R
import com.digitall.eid.models.common.StartDestination
import com.digitall.eid.ui.fragments.base.flow.BaseFlowViewModel

class ChangeCitizenInformationFlowViewModel : BaseFlowViewModel() {

    companion object {
        private const val TAG = "ChangeCitizenPhoneFlowViewModelTag"
    }

    fun getStartDestination() = StartDestination(R.id.changeCitizenPhoneFragment)

}