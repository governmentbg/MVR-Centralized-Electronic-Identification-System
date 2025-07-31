package com.digitall.eid.ui.fragments.onlinehelpsystem.flow

import com.digitall.eid.R
import com.digitall.eid.models.common.StartDestination
import com.digitall.eid.ui.fragments.base.flow.BaseFlowViewModel

class OnlineHelpSystemFlowViewModel : BaseFlowViewModel() {

    companion object {
        private const val TAG = "OnlineHelpSystemFlowViewModelTag"
    }

    fun getStartDestination() = StartDestination(R.id.onlineHelpSystemFragment)
}