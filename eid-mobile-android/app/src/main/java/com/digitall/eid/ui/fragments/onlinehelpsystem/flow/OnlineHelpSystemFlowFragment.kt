package com.digitall.eid.ui.fragments.onlinehelpsystem.flow

import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentFlowContainerBinding
import com.digitall.eid.ui.fragments.base.flow.BaseFlowFragment
import org.koin.androidx.viewmodel.ext.android.viewModel

class OnlineHelpSystemFlowFragment :
    BaseFlowFragment<FragmentFlowContainerBinding, OnlineHelpSystemFlowViewModel>() {

    companion object {
        private const val TAG = "FaqFlowFragmentTag"
    }

    override fun getViewBinding() = FragmentFlowContainerBinding.inflate(layoutInflater)

    override val viewModel: OnlineHelpSystemFlowViewModel by viewModel()

    override fun getFlowGraph() = R.navigation.nav_online_help_system

    override fun getStartDestination() = viewModel.getStartDestination()

}