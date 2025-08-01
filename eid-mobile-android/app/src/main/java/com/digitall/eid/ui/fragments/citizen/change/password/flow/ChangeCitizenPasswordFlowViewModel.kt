package com.digitall.eid.ui.fragments.citizen.change.password.flow

import com.digitall.eid.R
import com.digitall.eid.models.common.StartDestination
import com.digitall.eid.ui.fragments.base.flow.BaseFlowViewModel

class ChangeCitizenPasswordFlowViewModel: BaseFlowViewModel() {

    companion object {
        private const val TAG = "ChangeCitizenPasswordFlowViewModelTag"
    }

    fun getStartDestination(): StartDestination {
        return StartDestination(R.id.changeChangePasswordFragment)
    }
}