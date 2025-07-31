/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.extensions

import android.content.ActivityNotFoundException
import android.content.Context
import android.content.Intent
import android.net.Uri
import androidx.core.net.toUri

fun openAnotherApp(context: Context, packageName: String) {
    val intent = context.packageManager.getLaunchIntentForPackage(packageName)?.apply {
        flags = Intent.FLAG_ACTIVITY_NEW_TASK
    }
    if (intent != null) {
        context.startActivity(intent)
    } else {
        openPlayStoreAppPage(context, packageName)
    }
}

fun openPlayStoreAppPage(context: Context, packageName: String? = null) {
    val appPackageName = if (packageName.isNullOrEmpty()) context.packageName else packageName
    try {
        tryToOpenApp(context, appPackageName)
    } catch (e: ActivityNotFoundException) {
        goToMarket(context, appPackageName)
    }
}

@Throws(ActivityNotFoundException::class)
private fun tryToOpenApp(context: Context, pkg: String) {
    val intent = Intent(
        Intent.ACTION_VIEW,
        "market://details?id=$pkg".toUri()
    )
    context.startActivity(intent)
}

@Throws(ActivityNotFoundException::class)
private fun goToMarket(context: Context, pkg: String) {
    val intent = Intent(
        Intent.ACTION_VIEW,
        "https://play.google.com/store/apps/details?id=$pkg".toUri()
    )
    context.startActivity(intent)
}