package com.digitall.eid.ui.fragments.electronic.delivery.system.flow

import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentFlowContainerBinding
import com.digitall.eid.ui.fragments.base.flow.BaseFlowFragment
import org.koin.androidx.viewmodel.ext.android.viewModel

class ElectronicDeliverySystemFlowFragment : BaseFlowFragment<FragmentFlowContainerBinding, ElectronicDeliverySystemFlowViewModel>() {

    companion object {
        private const val TAG = "ElectronicDeliverySystemFlowFragmentTag"
    }

    override fun getViewBinding() = FragmentFlowContainerBinding.inflate(layoutInflater)

    override val viewModel: ElectronicDeliverySystemFlowViewModel by viewModel()

    override fun getFlowGraph() = R.navigation.nav_electronic_delivery_system

    override fun getStartDestination() = viewModel.getStartDestination()

}