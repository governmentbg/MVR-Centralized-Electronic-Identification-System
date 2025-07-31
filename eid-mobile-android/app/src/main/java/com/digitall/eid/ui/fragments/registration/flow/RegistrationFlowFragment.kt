package com.digitall.eid.ui.fragments.registration.flow

import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentFlowContainerBinding
import com.digitall.eid.models.common.StartDestination
import com.digitall.eid.ui.fragments.base.flow.BaseFlowFragment
import org.koin.androidx.viewmodel.ext.android.viewModel

class RegistrationFlowFragment: BaseFlowFragment<FragmentFlowContainerBinding, RegistrationFlowViewModel>() {

    companion object {
        private const val TAG = "RegistrationFlowFragmentTag"
    }

    override fun getViewBinding() = FragmentFlowContainerBinding.inflate(layoutInflater)

    override val viewModel: RegistrationFlowViewModel by viewModel()

    override fun getFlowGraph() = R.navigation.nav_registration

    override fun getStartDestination(): StartDestination {
        return viewModel.getStartDestination(requireContext())
    }
}