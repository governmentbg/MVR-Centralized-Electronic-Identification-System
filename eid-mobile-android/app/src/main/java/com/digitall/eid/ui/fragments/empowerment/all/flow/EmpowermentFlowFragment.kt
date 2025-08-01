/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.empowerment.all.flow

import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentFlowContainerBinding
import com.digitall.eid.models.common.StartDestination
import com.digitall.eid.ui.fragments.base.flow.BaseFlowFragment
import org.koin.androidx.viewmodel.ext.android.viewModel

class EmpowermentFlowFragment :
    BaseFlowFragment<FragmentFlowContainerBinding, EmpowermentFlowViewModel>() {

    companion object {
        private const val TAG = "EmpowermentFlowFragmentTag"
    }

    override fun getViewBinding() = FragmentFlowContainerBinding.inflate(layoutInflater)

    override val viewModel: EmpowermentFlowViewModel by viewModel()

    override fun getFlowGraph() = R.navigation.nav_empowerments

    override fun getStartDestination(): StartDestination {
        return viewModel.getStartDestination()
    }

}