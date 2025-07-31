package com.digitall.eid.ui.fragments.faq.flow

import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentFlowContainerBinding
import com.digitall.eid.ui.fragments.base.flow.BaseFlowFragment
import org.koin.androidx.viewmodel.ext.android.viewModel

class FaqFlowFragment :
    BaseFlowFragment<FragmentFlowContainerBinding, FaqFlowViewModel>() {

    companion object {
        private const val TAG = "FaqFlowFragmentTag"
    }

    override fun getViewBinding() = FragmentFlowContainerBinding.inflate(layoutInflater)

    override val viewModel: FaqFlowViewModel by viewModel()

    override fun getFlowGraph() = R.navigation.nav_faq

    override fun getStartDestination() = viewModel.getStartDestination()

}