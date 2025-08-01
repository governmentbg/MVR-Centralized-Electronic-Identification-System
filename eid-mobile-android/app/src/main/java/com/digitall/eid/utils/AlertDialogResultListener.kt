/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.utils

import com.digitall.eid.models.common.AlertDialogResult

fun interface AlertDialogResultListener {

    fun onAlertDialogResultReady(result: AlertDialogResult)

}