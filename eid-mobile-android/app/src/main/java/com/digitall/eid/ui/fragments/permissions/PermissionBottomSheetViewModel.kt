/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.permissions

import androidx.core.os.bundleOf
import androidx.fragment.app.setFragmentResult
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.mappers.common.PermissionNamePmMapper
import com.digitall.eid.ui.BaseViewModel
import com.digitall.eid.ui.fragments.permissions.PermissionBottomSheetFragment.Companion.IS_PERMISSION_GRANTED_BUNDLE_KEY
import com.digitall.eid.ui.fragments.permissions.PermissionBottomSheetFragment.Companion.PERMISSION_REQUEST_KEY
import com.digitall.eid.utils.PermissionsManager.Companion.isPermissionGranted
import org.koin.core.component.inject

class PermissionBottomSheetViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "PermissionBottomSheetViewModelTag"
    }

    private val permissionNamePmMapper: PermissionNamePmMapper by inject()

    private lateinit var permissionId: String

    fun setupPermissionFromArgs(permission: String) {
        logDebug("setupPermissionFromArgs permission: $permission", TAG)
        this.permissionId = permission
    }

    fun getPermissionName(): String {
        logDebug("getPermissionName", TAG)
        return permissionNamePmMapper.map(permissionId)
    }

    fun dismissIfPermissionGranted(fragment: PermissionBottomSheetFragment) {
        logDebug("dismissIfPermissionGranted", TAG)
        if (isPermissionGranted(fragment.requireContext(), permissionId)) {
            fragment.setFragmentResult(
                PERMISSION_REQUEST_KEY,
                bundleOf(IS_PERMISSION_GRANTED_BUNDLE_KEY to true)
            )
            fragment.dismiss()
        }
    }

    fun handleDismissAction(fragment: PermissionBottomSheetFragment) {
        logDebug("handleDismissAction", TAG)
        if (isPermissionGranted(fragment.requireContext(), permissionId).not()) {
            fragment.setFragmentResult(
                PERMISSION_REQUEST_KEY,
                bundleOf(IS_PERMISSION_GRANTED_BUNDLE_KEY to false)
            )
        }
    }

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        popBackStack()
    }

}