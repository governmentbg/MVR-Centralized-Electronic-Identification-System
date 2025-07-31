/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.permissions

import android.content.DialogInterface
import androidx.navigation.fragment.navArgs
import com.digitall.eid.R
import com.digitall.eid.databinding.BottomSheetPermissionBinding
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.extensions.openApplicationSettings
import com.digitall.eid.ui.fragments.base.BaseBottomSheetFragment
import org.koin.androidx.viewmodel.ext.android.viewModel

class PermissionBottomSheetFragment :
    BaseBottomSheetFragment<BottomSheetPermissionBinding, PermissionBottomSheetViewModel>() {

    companion object {
        private const val TAG = "PermissionBottomSheetFragmentTag"
        const val PERMISSION_REQUEST_KEY = "PERMISSION_BOTTOM_SHEET_REQUEST_KEY"
        const val IS_PERMISSION_GRANTED_BUNDLE_KEY = "IS_PERMISSION_GRANTED_BUNDLE_KEY"
    }

    override fun getViewBinding() = BottomSheetPermissionBinding.inflate(layoutInflater)

    override val viewModel: PermissionBottomSheetViewModel by viewModel()
    private val args: PermissionBottomSheetFragmentArgs by navArgs()

    private fun parseArguments() {
        try {
            viewModel.setupPermissionFromArgs(args.permissionId)
        } catch (e: IllegalStateException) {
            logError("parseArguments Exception: ${e.message}", e, TAG)
        }
    }

    override fun setupView() {
        binding.tvDescription.text = getString(
            R.string.permissions_modal_description,
            viewModel.getPermissionName().lowercase()
        )
        binding.tvStep1.text = getString(
            R.string.permissions_modal_step_1,
            viewModel.getPermissionName()
        )
    }

    override fun setupControls() {
        parseArguments()
        binding.btnNext.onClickThrottle {
            logDebug("btnNext onClickThrottle", TAG)
            requireContext().openApplicationSettings(this)
        }
    }

    override fun onResumed() {
        viewModel.dismissIfPermissionGranted(this)
    }

    override fun onDismissed(dialog: DialogInterface) {
        viewModel.handleDismissAction(this)
    }

}