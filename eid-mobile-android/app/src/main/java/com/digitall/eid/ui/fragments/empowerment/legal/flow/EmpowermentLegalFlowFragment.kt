package com.digitall.eid.ui.fragments.empowerment.legal.flow

import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentFlowContainerBinding
import com.digitall.eid.models.common.StartDestination
import com.digitall.eid.ui.fragments.base.flow.BaseFlowFragment
import org.koin.androidx.viewmodel.ext.android.viewModel

class EmpowermentLegalFlowFragment :
    BaseFlowFragment<FragmentFlowContainerBinding, EmpowermentLegalFlowViewModel>() {

    companion object {
        private const val TAG = "EmpowermentLegalFlowFragmentTag"
    }

    override fun getViewBinding() = FragmentFlowContainerBinding.inflate(layoutInflater)

    override val viewModel: EmpowermentLegalFlowViewModel by viewModel()

    override fun getFlowGraph() = R.navigation.nav_empowerments_legal

    override fun getStartDestination(): StartDestination {
        return viewModel.getStartDestination()
    }

}