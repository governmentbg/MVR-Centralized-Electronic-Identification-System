package com.digitall.eid.ui.fragments.citizen.change.email.flow

import com.digitall.eid.R
import com.digitall.eid.models.common.StartDestination
import com.digitall.eid.ui.fragments.base.flow.BaseFlowViewModel

class ChangeCitizenEmailFlowViewModel: BaseFlowViewModel() {

    companion object {
        private const val TAG = "ChangeCitizenEmailFlowViewModelTag"
    }

    fun getStartDestination(): StartDestination {
        return StartDestination(R.id.changeCitizenEmailFragment)
    }
}