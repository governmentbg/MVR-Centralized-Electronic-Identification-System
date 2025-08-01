/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.applications.show.flow

import androidx.navigation.fragment.navArgs
import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentFlowContainerBinding
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.models.common.StartDestination
import com.digitall.eid.ui.fragments.base.flow.BaseFlowFragment
import org.koin.androidx.viewmodel.ext.android.viewModel

class ApplicationsFlowFragment :
    BaseFlowFragment<FragmentFlowContainerBinding, ApplicationsFlowViewModel>() {

    companion object {
        private const val TAG = "ApplicationsFlowFragmentTag"
    }

    override fun getViewBinding() = FragmentFlowContainerBinding.inflate(layoutInflater)
    override val viewModel: ApplicationsFlowViewModel by viewModel()
    private val args: ApplicationsFlowFragmentArgs by navArgs()

    override fun getFlowGraph() = R.navigation.nav_applications

    override fun getStartDestination(): StartDestination {
        return try {
            viewModel.getStartDestination(
                applicationId = args.applicationId,
                certificateId = args.certificateId,
            )
        } catch (e: IllegalStateException) {
            logError("parseArguments Exception: ${e.message}", e, TAG)
            viewModel.getStartDestination(
                applicationId = null,
                certificateId = null,
            )
        }
    }

}