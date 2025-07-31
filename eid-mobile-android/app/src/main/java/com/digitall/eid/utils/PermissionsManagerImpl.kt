/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.utils

import androidx.activity.ComponentActivity
import androidx.activity.result.ActivityResultLauncher
import androidx.activity.result.contract.ActivityResultContracts
import androidx.core.app.ActivityCompat
import androidx.fragment.app.Fragment
import androidx.navigation.NavController
import com.digitall.eid.NavActivityDirections
import com.digitall.eid.extensions.findActivityNavController
import com.digitall.eid.mappers.common.PermissionNamePmMapper
import org.koin.core.component.KoinComponent
import java.lang.ref.WeakReference
import org.koin.core.component.inject

class PermissionsManagerImpl : PermissionsManager, KoinComponent {

    private val currentContext: CurrentContext by inject()
    private val permissionNamePmMapper: PermissionNamePmMapper by inject()

    private var permissionsIds = mutableListOf<String>()

    private var activityNavController: WeakReference<NavController>? = null

    private var permissionGrantedCallback: ((permissionId: String) -> Unit)? = null

    private var permissionRequestLauncher: ActivityResultLauncher<Array<String>>? = null

    override fun initializeManager(fragment: Fragment) {
        initializeManagerInternal(fragment, ::handleMultiplePermissionsRequestResult)
    }

    override fun initializeWithCombinedPermissionsManager(fragment: Fragment) {
        initializeManagerInternal(fragment, ::handleCombinedPermissionsRequestResult)
    }

    private fun initializeManagerInternal(
        fragment: Fragment,
        onPermissionsRequestResult: (Map<String, Boolean>) -> Unit
    ) {
        this.activityNavController = WeakReference(fragment.findActivityNavController())
        if (permissionRequestLauncher == null) {
            this.permissionRequestLauncher = fragment.registerForActivityResult(
                ActivityResultContracts.RequestMultiplePermissions(),
                onPermissionsRequestResult::invoke,
            )
        }
    }

    private fun handleMultiplePermissionsRequestResult(permissions: Map<String, Boolean>) {
        permissions.keys.forEach { permissionId ->
            if (permissionsIds.contains(permissionId)) {
                if (permissions[permissionId] == true) {
                    permissionGrantedCallback?.invoke(permissionId)
                } else {
                    showPermissionExplanationBottomSheet(permissionId)
                }
            }
        }
    }

    private fun handleCombinedPermissionsRequestResult(permissions: Map<String, Boolean>) {
        val isPermissionGranted = permissions.keys.any { permissionId ->
            permissionsIds.contains(permissionId) && permissions[permissionId] == true
        }
        if (isPermissionGranted) {
            permissionGrantedCallback?.invoke(permissionsIds.first())
        } else {
            showPermissionExplanationBottomSheet(permissionsIds.first())
        }
    }

    override fun requestPermissions(
        activity: ComponentActivity,
        permissionsIds: List<String>,
        onGranted: (permissionId: String) -> Unit
    ) {
        if (permissionsIds.isEmpty()) {
            throw IllegalArgumentException("permissionsIds should not be empty")
        }

        this.permissionsIds.clear()
        this.permissionsIds.addAll(permissionsIds)
        this.permissionGrantedCallback = onGranted
        requestPermissionsInternal(activity, permissionsIds)
    }

    /**
     * Request permissions list without saving the [permissionsIds] to private variable.
     * The [permissionsIds] arguments could have different permissions list than
     * the private variable. BUT the [permissionsIds] private variable should always contain all
     * permissions from the argument list, otherwise [permissionGrantedCallback] won't be called.
     */
    private fun requestPermissionsInternal(
        activity: ComponentActivity,
        permissionsIds: List<String>
    ) {
        if (permissionsIds.isEmpty()) return

        val requestPermissionsIds = permissionsIds.toMutableList()
        permissionsIds.forEach { permissionId ->
            val needToRequest = when {
                isPermissionGranted(permissionId) -> {
                    permissionGrantedCallback?.invoke(permissionId)
                    false
                }

                ActivityCompat.shouldShowRequestPermissionRationale(activity, permissionId) -> {
                    showPermissionExplanationBottomSheet(permissionId)
                    false
                }

                else -> true
            }
            if (!needToRequest) requestPermissionsIds.remove(permissionId)
        }

        // Request only permissions that are not yet granted and
        // ones that don't require a rationale dialog
        if (requestPermissionsIds.isNotEmpty()) {
            permissionRequestLauncher?.launch(requestPermissionsIds.toTypedArray())
        }
    }

    override fun requestCombinedPermissions(
        activity: ComponentActivity,
        permissionsIds: List<String>,
        onGranted: (permissionId: String) -> Unit
    ) {
        if (permissionsIds.isEmpty()) {
            throw IllegalArgumentException("permissionsIds should not be empty")
        }

        this.permissionsIds.clear()
        this.permissionsIds.addAll(permissionsIds)
        this.permissionGrantedCallback = onGranted
        val mainPermission = permissionsIds.first()
        when {
            isPermissionGranted(mainPermission) -> permissionGrantedCallback?.invoke(mainPermission)
            ActivityCompat.shouldShowRequestPermissionRationale(activity, mainPermission) -> {
                showPermissionExplanationBottomSheet(mainPermission)
            }

            else -> permissionRequestLauncher?.launch(permissionsIds.toTypedArray())
        }
    }

    override fun oneTimePermissionsRequest(
        activity: ComponentActivity,
        permissionsIdsWithCheckedState: Map<String, Boolean>,
        onGranted: (permissionId: String) -> Unit
    ) {
        if (permissionsIdsWithCheckedState.isEmpty()) {
            throw IllegalArgumentException("permissionsIdsAndChecked should not be empty")
        }

        val requestPermissionsIds = permissionsIdsWithCheckedState.keys.toMutableList()
        this.permissionsIds.clear()
        this.permissionsIds.addAll(requestPermissionsIds)
        this.permissionGrantedCallback = onGranted
        permissionsIdsWithCheckedState.keys.forEach { permissionId ->
            val isAlreadyChecked = permissionsIdsWithCheckedState[permissionId] ?: false
            if (isAlreadyChecked || isPermissionGranted(permissionId)) {
                requestPermissionsIds.remove(permissionId)
            }
        }
        if (requestPermissionsIds.isNotEmpty()) {
            requestPermissionsInternal(activity, requestPermissionsIds)
        }
    }

    override fun isPermissionGranted(permissionId: String): Boolean {
        return PermissionsManager.isPermissionGranted(currentContext.get(), permissionId)
    }

    override fun getPermissionName(permissionId: String): String {
        return permissionNamePmMapper.map(permissionId)
    }

    override fun destroyManager() {
        activityNavController?.clear()
        activityNavController = null
        permissionGrantedCallback = null
        permissionRequestLauncher?.unregister()
        permissionRequestLauncher = null
    }

    private fun showPermissionExplanationBottomSheet(permissionId: String) {
        activityNavController?.get()?.navigate(
            NavActivityDirections.toPermissionBottomSheetFragment(permissionId = permissionId)
        )
    }
}