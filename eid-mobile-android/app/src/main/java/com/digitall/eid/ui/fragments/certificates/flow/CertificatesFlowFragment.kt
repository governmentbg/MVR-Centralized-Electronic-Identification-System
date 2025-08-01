/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.certificates.flow

import androidx.navigation.fragment.navArgs
import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentFlowContainerBinding
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.models.common.StartDestination
import com.digitall.eid.ui.fragments.base.flow.BaseFlowFragment
import org.koin.androidx.viewmodel.ext.android.viewModel

class CertificatesFlowFragment :
    BaseFlowFragment<FragmentFlowContainerBinding, CertificatesFlowViewModel>() {

    companion object {
        private const val TAG = "CertificatesFlowFragmentTag"
    }

    override fun getViewBinding() = FragmentFlowContainerBinding.inflate(layoutInflater)
    override val viewModel: CertificatesFlowViewModel by viewModel()
    private val args: CertificatesFlowFragmentArgs by navArgs()

    override fun getFlowGraph() = R.navigation.nav_certificates

    override fun getStartDestination(): StartDestination {
        return try {
            viewModel.getStartDestination(
                certificateId = args.certificateId,
                applicationId = args.applicationId,
            )
        } catch (e: IllegalStateException) {
            logError("parseArguments Exception: ${e.message}", e, TAG)
            viewModel.getStartDestination(
                certificateId = null,
                applicationId = null,
            )
        }
    }

}