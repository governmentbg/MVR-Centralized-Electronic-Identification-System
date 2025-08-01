/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.applications.continuecreation.continuecreation

import androidx.navigation.fragment.navArgs
import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentWithLoaderBinding
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.ui.fragments.base.BaseFragment
import org.koin.androidx.viewmodel.ext.android.viewModel

class ApplicationContinueCreationFragment :
    BaseFragment<FragmentWithLoaderBinding, ApplicationContinueCreationViewModel>() {

    companion object {
        private const val TAG = "ApplicationContinueCreationFragmentTag"
    }

    override fun getViewBinding() = FragmentWithLoaderBinding.inflate(layoutInflater)

    override val viewModel: ApplicationContinueCreationViewModel by viewModel()

    private val args: ApplicationContinueCreationFragmentArgs by navArgs()

    override fun parseArguments() {
        try {
            viewModel.setupModel(args.applicationId)
        } catch (e: IllegalStateException) {
            logError("parseArguments Exception: ${e.message}", e, TAG)
            viewModel.setupModel(null)
        }
    }

    override fun setupView() {
        binding.toolbar.setTitleText(StringSource(R.string.create_application_screen_title))
    }

    override fun setupControls() {
        binding.toolbar.navigationClickListener = {
            viewModel.onBackPressed()
        }
        binding.errorView.actionOneClickListener = {
            viewModel.refreshScreen()
        }
        binding.errorView.actionTwoClickListener = {
            viewModel.refreshScreen()
        }
        binding.emptyStateView.reloadClickListener = {
            viewModel.refreshScreen()
        }
    }

}