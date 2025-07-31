/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.applications.confirm.flow

import androidx.navigation.fragment.navArgs
import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentFlowContainerBinding
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.models.common.StartDestination
import com.digitall.eid.ui.fragments.base.flow.BaseFlowFragment
import org.koin.androidx.viewmodel.ext.android.viewModel

class ApplicationConfirmFlowFragment :
    BaseFlowFragment<FragmentFlowContainerBinding, ApplicationConfirmFlowViewModel>() {

    companion object {
        private const val TAG = "ApplicationConfirmFlowFragmentTag"
    }

    override fun getViewBinding() = FragmentFlowContainerBinding.inflate(layoutInflater)

    override val viewModel: ApplicationConfirmFlowViewModel by viewModel()
    private val args: ApplicationConfirmFlowFragmentArgs by navArgs()

    override fun getFlowGraph() = R.navigation.nav_application_confirm

    override fun getStartDestination(): StartDestination {
        return try {
            viewModel.getStartDestination(args.qrCode)
        } catch (e: IllegalStateException) {
            logError("parseArguments Exception: ${e.message}", e, TAG)
            viewModel.getStartDestination(null)
        }
    }

}