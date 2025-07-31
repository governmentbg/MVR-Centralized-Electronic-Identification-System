package com.digitall.eid.ui.fragments.centers.certification.services.flow

import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentFlowContainerBinding
import com.digitall.eid.ui.fragments.base.flow.BaseFlowFragment
import org.koin.androidx.viewmodel.ext.android.viewModel

class CentersCertificationServicesFlowFragment : BaseFlowFragment<FragmentFlowContainerBinding, CentersCertificationServicesFlowViewModel>() {

    companion object {
        private const val TAG = "CentersCertificationServicesFlowFragmentTag"
    }

    override fun getViewBinding() = FragmentFlowContainerBinding.inflate(layoutInflater)

    override val viewModel: CentersCertificationServicesFlowViewModel by viewModel()

    override fun getFlowGraph() = R.navigation.nav_centers_certification_services

    override fun getStartDestination() = viewModel.getStartDestination()

}