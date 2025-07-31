package com.digitall.eid.ui.fragments.providers.electronic.administrative.services.flow

import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentFlowContainerBinding
import com.digitall.eid.ui.fragments.base.flow.BaseFlowFragment
import org.koin.androidx.viewmodel.ext.android.viewModel

class ProvidersElectronicAdministrativeServicesFlowFragment :
    BaseFlowFragment<FragmentFlowContainerBinding, ProvidersElectronicAdministrativeServicesFlowViewModel>() {

    companion object {
        private const val TAG = "ProvidersElectronicAdministrativeServicesFlowFragmentTag"
    }

    override fun getViewBinding() = FragmentFlowContainerBinding.inflate(layoutInflater)

    override val viewModel: ProvidersElectronicAdministrativeServicesFlowViewModel by viewModel()

    override fun getFlowGraph() = R.navigation.nav_providers_electronic_administrative_services

    override fun getStartDestination() = viewModel.getStartDestination()

}