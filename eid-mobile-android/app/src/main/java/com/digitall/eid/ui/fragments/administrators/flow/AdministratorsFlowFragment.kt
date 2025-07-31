package com.digitall.eid.ui.fragments.administrators.flow

import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentFlowContainerBinding
import com.digitall.eid.ui.fragments.base.flow.BaseFlowFragment
import org.koin.androidx.viewmodel.ext.android.viewModel

class AdministratorsFlowFragment :
    BaseFlowFragment<FragmentFlowContainerBinding, AdministratorsFlowViewModel>() {

    companion object {
        private const val TAG = "AdministratorsFlowFragmentTag"
    }

    override fun getViewBinding() = FragmentFlowContainerBinding.inflate(layoutInflater)

    override val viewModel: AdministratorsFlowViewModel by viewModel()

    override fun getFlowGraph() = R.navigation.nav_administrators

    override fun getStartDestination() = viewModel.getStartDestination()

}