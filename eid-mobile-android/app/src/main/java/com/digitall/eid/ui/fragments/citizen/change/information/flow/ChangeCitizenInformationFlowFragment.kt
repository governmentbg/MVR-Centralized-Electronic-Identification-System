package com.digitall.eid.ui.fragments.citizen.change.information.flow

import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentFlowContainerBinding
import com.digitall.eid.ui.fragments.base.flow.BaseFlowFragment
import org.koin.androidx.viewmodel.ext.android.viewModel

class ChangeCitizenInformationFlowFragment : BaseFlowFragment<FragmentFlowContainerBinding, ChangeCitizenInformationFlowViewModel>() {

    override fun getViewBinding() = FragmentFlowContainerBinding.inflate(layoutInflater)

    override val viewModel: ChangeCitizenInformationFlowViewModel by viewModel()

    override fun getFlowGraph() = R.navigation.nav_citizen_change_information

    override fun getStartDestination() = viewModel.getStartDestination()

}