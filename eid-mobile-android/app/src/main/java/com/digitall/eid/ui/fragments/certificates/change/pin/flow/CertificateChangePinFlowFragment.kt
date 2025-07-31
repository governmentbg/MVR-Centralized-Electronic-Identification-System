package com.digitall.eid.ui.fragments.certificates.change.pin.flow

import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentFlowContainerBinding
import com.digitall.eid.ui.fragments.base.flow.BaseFlowFragment
import org.koin.androidx.viewmodel.ext.android.viewModel

class CertificateChangePinFlowFragment  :
    BaseFlowFragment<FragmentFlowContainerBinding, CertificateChangePinFlowViewModel>() {

    companion object {
        private const val TAG = "CardChangePinFlowFragmentTag"
    }

    override fun getViewBinding() = FragmentFlowContainerBinding.inflate(layoutInflater)

    override val viewModel: CertificateChangePinFlowViewModel by viewModel()

    override fun getFlowGraph() = R.navigation.nav_card_change_pin

    override fun getStartDestination() = viewModel.getStartDestination()
}