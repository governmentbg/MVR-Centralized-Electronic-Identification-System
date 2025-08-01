/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.auth.flow

import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentFlowContainerBinding
import com.digitall.eid.models.common.StartDestination
import com.digitall.eid.ui.fragments.base.flow.BaseFlowFragment
import org.koin.androidx.viewmodel.ext.android.viewModel

class AuthFlowFragment :
    BaseFlowFragment<FragmentFlowContainerBinding, AuthFlowViewModel>() {

    companion object {
        private const val TAG = "AuthEnterFlowFragmentTag"
    }

    override fun getViewBinding() = FragmentFlowContainerBinding.inflate(layoutInflater)

    override val viewModel: AuthFlowViewModel by viewModel()

    override fun getFlowGraph() = R.navigation.nav_authentication_enter_credentials

    override fun getStartDestination(): StartDestination {
        return viewModel.getStartDestination(requireContext())
    }

}