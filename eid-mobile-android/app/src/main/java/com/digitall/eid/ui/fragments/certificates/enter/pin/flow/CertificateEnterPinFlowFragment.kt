package com.digitall.eid.ui.fragments.certificates.enter.pin.flow

import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentFlowContainerBinding
import com.digitall.eid.models.common.StartDestination
import com.digitall.eid.ui.fragments.base.flow.BaseFlowFragment
import org.koin.androidx.viewmodel.ext.android.viewModel

class CertificateEnterPinFlowFragment :
    BaseFlowFragment<FragmentFlowContainerBinding, CertificateEnterPinFlowViewModel>() {

    companion object {
        private const val TAG = "AuthEnterFlowFragmentTag"
    }

    override fun getViewBinding() = FragmentFlowContainerBinding.inflate(layoutInflater)

    override val viewModel: CertificateEnterPinFlowViewModel by viewModel()

    override fun getFlowGraph() = R.navigation.nav_certificate_enter_pin

    override fun getStartDestination(): StartDestination =
        viewModel.getStartDestination(requireContext())

}