package com.digitall.eid.ui.fragments.administrators.flow

import com.digitall.eid.R
import com.digitall.eid.models.common.StartDestination
import com.digitall.eid.ui.fragments.base.flow.BaseFlowViewModel

class AdministratorsFlowViewModel : BaseFlowViewModel() {

    companion object {
        private const val TAG = "AdministratorsFlowViewModelTag"
    }

    fun getStartDestination() = StartDestination(R.id.administratorsFragment)
}