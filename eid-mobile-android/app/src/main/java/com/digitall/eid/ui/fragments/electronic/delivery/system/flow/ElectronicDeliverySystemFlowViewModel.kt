package com.digitall.eid.ui.fragments.electronic.delivery.system.flow

import com.digitall.eid.R
import com.digitall.eid.models.common.StartDestination
import com.digitall.eid.ui.fragments.base.flow.BaseFlowViewModel

class ElectronicDeliverySystemFlowViewModel : BaseFlowViewModel() {

    companion object {
        private const val TAG = "ElectronicDeliverySystemFlowViewModelTag"
    }

    fun getStartDestination() = StartDestination(R.id.electronicDeliverySystemFragment)
}