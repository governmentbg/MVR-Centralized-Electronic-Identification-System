package com.digitall.eid.ui.fragments.journal.from.me.flow

import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentFlowContainerBinding
import com.digitall.eid.models.common.StartDestination
import com.digitall.eid.ui.fragments.base.flow.BaseFlowFragment
import org.koin.androidx.viewmodel.ext.android.viewModel

class JournalFromMeFlowFragment :
    BaseFlowFragment<FragmentFlowContainerBinding, JournalFromMeFlowViewModel>() {

    companion object {
        private const val TAG = "JournalFromMeFlowFragmentTag"
    }

    override fun getViewBinding() = FragmentFlowContainerBinding.inflate(layoutInflater)

    override val viewModel: JournalFromMeFlowViewModel by viewModel()

    override fun getFlowGraph() = R.navigation.nav_journals_from_me

    override fun getStartDestination(): StartDestination {
        return viewModel.getStartDestination()
    }

}