/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.scanner.flow

import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentFlowContainerBinding
import com.digitall.eid.models.common.StartDestination
import com.digitall.eid.ui.fragments.base.flow.BaseFlowFragment
import org.koin.androidx.viewmodel.ext.android.viewModel

class ScanCodeFlowFragment :
    BaseFlowFragment<FragmentFlowContainerBinding, ScanCodeFlowViewModel>() {

    override fun getViewBinding() = FragmentFlowContainerBinding.inflate(layoutInflater)

    override val viewModel: ScanCodeFlowViewModel by viewModel()

    override fun getFlowGraph() = R.navigation.nav_scan_code

    override fun getStartDestination(): StartDestination {
        return viewModel.getStartDestination()
    }

}