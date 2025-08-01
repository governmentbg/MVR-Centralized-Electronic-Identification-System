/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.journal.flow

import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentFlowContainerBinding
import com.digitall.eid.models.common.StartDestination
import com.digitall.eid.ui.fragments.base.flow.BaseFlowFragment
import org.koin.androidx.viewmodel.ext.android.viewModel

class JournalFlowFragment :
    BaseFlowFragment<FragmentFlowContainerBinding, JournalFlowViewModel>() {

    companion object {
        private const val TAG = "JournalFlowFragmentTag"
    }

    override fun getViewBinding() = FragmentFlowContainerBinding.inflate(layoutInflater)

    override val viewModel: JournalFlowViewModel by viewModel()

    override fun getFlowGraph() = R.navigation.nav_journals

    override fun getStartDestination(): StartDestination {
        return viewModel.getStartDestination()
    }

}