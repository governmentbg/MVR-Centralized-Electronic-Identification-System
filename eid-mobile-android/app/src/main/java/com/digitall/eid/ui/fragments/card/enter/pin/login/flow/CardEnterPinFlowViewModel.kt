package com.digitall.eid.ui.fragments.card.enter.pin.login.flow

import com.digitall.eid.R
import com.digitall.eid.models.common.StartDestination
import com.digitall.eid.ui.fragments.base.flow.BaseFlowViewModel

class CardEnterPinFlowViewModel : BaseFlowViewModel() {

    companion object {
        private const val TAG = "CardEnterPinFlowViewModelTag"
    }

    fun getStartDestination() = StartDestination(R.id.cardEnterPinFragment)

}