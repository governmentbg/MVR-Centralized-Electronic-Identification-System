package com.digitall.eid.ui.fragments.termsandconditions.flow

import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentFlowContainerBinding
import com.digitall.eid.ui.fragments.base.flow.BaseFlowFragment
import org.koin.androidx.viewmodel.ext.android.viewModel

class TermsAndConditionsFlowFragment :
    BaseFlowFragment<FragmentFlowContainerBinding, TermsAndConditionsFlowViewModel>() {

    companion object {
        private const val TAG = "FaqFlowFragmentTag"
    }

    override fun getViewBinding() = FragmentFlowContainerBinding.inflate(layoutInflater)

    override val viewModel: TermsAndConditionsFlowViewModel by viewModel()

    override fun getFlowGraph() = R.navigation.nav_terms_and_conditions

    override fun getStartDestination() = viewModel.getStartDestination()

}