package com.digitall.eid.ui.fragments.citizen.change.password.flow

import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentFlowContainerBinding
import com.digitall.eid.models.common.StartDestination
import com.digitall.eid.ui.fragments.base.flow.BaseFlowFragment
import org.koin.androidx.viewmodel.ext.android.viewModel

class ChangeCitizenPasswordFlowFragment : BaseFlowFragment<FragmentFlowContainerBinding, ChangeCitizenPasswordFlowViewModel>() {

    override fun getViewBinding() = FragmentFlowContainerBinding.inflate(layoutInflater)

    override val viewModel: ChangeCitizenPasswordFlowViewModel by viewModel()

    override fun getFlowGraph() = R.navigation.nav_citizen_change_password

    override fun getStartDestination(): StartDestination {
        return viewModel.getStartDestination()
    }
}