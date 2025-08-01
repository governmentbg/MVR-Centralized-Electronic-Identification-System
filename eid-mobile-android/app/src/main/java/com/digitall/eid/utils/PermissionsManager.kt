/**
 * Simple permission manager that can work with several permission at the time.
 * The manager should be initialized with [initializeManager]
 * and destroyed with [destroyManager].
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 */
package com.digitall.eid.utils

import android.content.Context
import android.content.pm.PackageManager
import androidx.activity.ComponentActivity
import androidx.core.content.ContextCompat
import androidx.fragment.app.Fragment

interface PermissionsManager {

    companion object {
        fun isPermissionGranted(context: Context, permissionId: String): Boolean {
            return ContextCompat.checkSelfPermission(context, permissionId) ==
                    PackageManager.PERMISSION_GRANTED
        }
    }

    /**
     * Initialization of the manager and register permission result callback using [fragment].
     * The callback should be called when the permission is granted and app could work
     * with the feature. All other handling this manager will do itself.
     *
     * The actual callback to manager's holder would be set in [requestPermissions].
     * While it is not set, the results from the system are ignored.
     */
    fun initializeManager(fragment: Fragment)

    /**
     * Initialization of the manager with several permissions and register unified
     * permission result callback using [fragment].
     * The callback should be called when we can work with feature.
     * All other handling this manager will do itself.
     *
     * The actual callback to manager's holder would be set in [requestCombinedPermissions].
     * While it is not set, the results from the system are ignored.
     */
    fun initializeWithCombinedPermissionsManager(fragment: Fragment)

    /**
     * The [permissionsIds] should contain permissions IDs from Manifest.permission.* only.
     *
     * Requests system permissions from [permissionsIds] and waits for
     * the results in callback [onGranted], which works per permission ID.
     *
     * The callback itself could live long after this method scope, basically it just
     * sets in the manager and uses in ActivityResultCallback until it would be
     * changed or destroyed in [destroyManager].
     *
     * This method either calls the callback right away if permission is already granted,
     * shows the explanation dialog if the permission was denied before,
     * or starts system permission dialog and requests the permission.
     */
    fun requestPermissions(
        activity: ComponentActivity,
        permissionsIds: List<String>,
        onGranted: (permissionId: String) -> Unit
    )

    /**
     * This method is similar to the [requestPermissions] but all the permissions
     * in [permissionsIds] are having a common rationale dialog and a one callback call
     * instead of [List.size] callbacks. Useful for location or read/write files permissions
     * for example.
     *
     * Basically, all the permissions in [permissionsIds] requests as one for user,
     * so the first permission in the list have the highest priority and acts as
     * a main permission that we actually request.
     *
     * If the main permission is granted, the [onGranted] would be called immediately.
     * Otherwise, the rationale dialog could be shown for main permission or all permissions
     * in [permissionsIds] will be requested in the system.
     */
    fun requestCombinedPermissions(
        activity: ComponentActivity,
        permissionsIds: List<String>,
        onGranted: (permissionId: String) -> Unit
    )

    /**
     * If we want to ask user about permissions one time we need to use this function.
     * And we should add to sharedPreferences checked permissions state and pass it
     * like [permissionsIdsWithCheckedState] value param. The key in the [permissionsIdsWithCheckedState] map
     * is permission ID, same as in [requestPermissions] list.
     *
     * The other method's mechanic is similar to [requestPermissions].
     */
    fun oneTimePermissionsRequest(
        activity: ComponentActivity,
        permissionsIdsWithCheckedState: Map<String, Boolean>,
        onGranted: (permissionId: String) -> Unit
    )

    /**
     * Checks if the [permissionId] is granted.
     * Similar to [PermissionsManager.Companion.isPermissionGranted] but do not require a context.
     */
    fun isPermissionGranted(permissionId: String): Boolean

    /**
     * Get the permission's readable name from the [permissionId].
     */
    fun getPermissionName(permissionId: String): String

    /**
     * Call this method in view model onCleared method or in any other destruction method
     * in the holder lifecycle.
     */
    fun destroyManager()

}