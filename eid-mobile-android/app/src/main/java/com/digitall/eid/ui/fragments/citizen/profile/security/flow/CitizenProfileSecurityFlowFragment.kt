package com.digitall.eid.ui.fragments.citizen.profile.security.flow

import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentFlowContainerBinding
import com.digitall.eid.ui.fragments.base.flow.BaseFlowFragment
import org.koin.androidx.viewmodel.ext.android.viewModel

class CitizenProfileSecurityFlowFragment :
    BaseFlowFragment<FragmentFlowContainerBinding, CitizenProfileSecurityFlowViewModel>() {

    override fun getViewBinding() = FragmentFlowContainerBinding.inflate(layoutInflater)

    override val viewModel: CitizenProfileSecurityFlowViewModel by viewModel()

    override fun getFlowGraph() = R.navigation.nav_citizen_profile_security

    override fun getStartDestination() = viewModel.getStartDestination()

}