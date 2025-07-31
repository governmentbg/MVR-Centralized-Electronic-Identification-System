package com.digitall.eid.ui.fragments.payments.history.flow

import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentFlowContainerBinding
import com.digitall.eid.ui.fragments.base.flow.BaseFlowFragment
import org.koin.androidx.viewmodel.ext.android.viewModel

class PaymentsHistoryFlowFragment :
    BaseFlowFragment<FragmentFlowContainerBinding, PaymentsHistoryFlowViewModel>() {

    companion object {
        private const val TAG = "PaymentHistoryFlowFragmentTag"
    }

    override fun getViewBinding() = FragmentFlowContainerBinding.inflate(layoutInflater)

    override val viewModel: PaymentsHistoryFlowViewModel by viewModel()

    override fun getFlowGraph() = R.navigation.nav_payments_history

    override fun getStartDestination() = viewModel.getStartDestination()
}