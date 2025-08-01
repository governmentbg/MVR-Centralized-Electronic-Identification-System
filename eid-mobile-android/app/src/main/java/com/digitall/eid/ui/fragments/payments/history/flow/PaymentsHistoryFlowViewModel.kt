package com.digitall.eid.ui.fragments.payments.history.flow

import com.digitall.eid.R
import com.digitall.eid.models.common.StartDestination
import com.digitall.eid.ui.fragments.base.flow.BaseFlowViewModel

class PaymentsHistoryFlowViewModel : BaseFlowViewModel() {

    companion object {
        private const val TAG = "PaymentHistoryFlowViewModelTag"
    }

    fun getStartDestination() = StartDestination(R.id.paymentsHistoryFragment)
}