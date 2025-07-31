package com.digitall.eid.ui.fragments.citizen.information.flow

import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentFlowContainerBinding
import com.digitall.eid.ui.fragments.base.flow.BaseFlowFragment
import org.koin.androidx.viewmodel.ext.android.viewModel

class CitizenInformationFlowFragment :
    BaseFlowFragment<FragmentFlowContainerBinding, CitizenInformationFlowViewModel>() {

    override fun getViewBinding() = FragmentFlowContainerBinding.inflate(layoutInflater)

    override val viewModel: CitizenInformationFlowViewModel by viewModel()

    override fun getFlowGraph() = R.navigation.nav_citizen_information

    override fun getStartDestination() = viewModel.getStartDestination()

}