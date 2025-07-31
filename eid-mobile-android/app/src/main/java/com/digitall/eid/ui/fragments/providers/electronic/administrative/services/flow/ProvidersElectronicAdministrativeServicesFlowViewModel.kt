package com.digitall.eid.ui.fragments.providers.electronic.administrative.services.flow

import com.digitall.eid.R
import com.digitall.eid.models.common.StartDestination
import com.digitall.eid.ui.fragments.base.flow.BaseFlowViewModel

class ProvidersElectronicAdministrativeServicesFlowViewModel : BaseFlowViewModel() {

    companion object {
        private const val TAG = "ProvidersElectronicAdministrativeServicesFlowViewModelTag"
    }

    fun getStartDestination() = StartDestination(R.id.providersElectronicAdministrativeServicesFragment)
}