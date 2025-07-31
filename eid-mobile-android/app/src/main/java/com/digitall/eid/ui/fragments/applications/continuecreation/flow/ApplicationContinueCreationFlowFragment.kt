/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.applications.continuecreation.flow

import androidx.navigation.fragment.navArgs
import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentFlowContainerBinding
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.models.common.StartDestination
import com.digitall.eid.ui.fragments.base.flow.BaseFlowFragment
import org.koin.androidx.viewmodel.ext.android.viewModel

class ApplicationContinueCreationFlowFragment :
    BaseFlowFragment<FragmentFlowContainerBinding, ApplicationContinueCreationFlowViewModel>() {

    companion object {
        private const val TAG = "ApplicationContinueCreationFlowFragmentTag"
    }

    override fun getViewBinding() = FragmentFlowContainerBinding.inflate(layoutInflater)

    override val viewModel: ApplicationContinueCreationFlowViewModel by viewModel()

    override fun getFlowGraph() = R.navigation.nav_application_continue_creation

    private val args: ApplicationContinueCreationFlowFragmentArgs by navArgs()

    override fun getStartDestination(): StartDestination {
        return try {
            viewModel.getStartDestination(
                applicationId = args.applicationId,
            )
        } catch (e: IllegalStateException) {
            logError("parseArguments Exception: ${e.message}", e, TAG)
            viewModel.getStartDestination(
                applicationId = null,
            )
        }
    }

}