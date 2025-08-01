/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.utils

import android.annotation.SuppressLint
import android.app.DownloadManager
import android.content.BroadcastReceiver
import android.content.Context
import android.content.Intent
import android.content.IntentFilter
import android.os.Environment
import androidx.core.content.ContextCompat
import androidx.core.net.toUri
import com.digitall.eid.domain.repository.common.PreferencesRepository
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.readOnly
import org.koin.core.component.KoinComponent
import org.koin.core.component.inject

class DownloadHelper : KoinComponent {

    companion object {
        private const val TAG = "DownloadHelperTag"
    }

    private val downloadManager: DownloadManager by inject()
    private val preferences: PreferencesRepository by inject()

    private var downloadId: Long = 0L

    private val _onReadyLiveData = SingleLiveEvent<Unit>()
    val onReadyLiveData = _onReadyLiveData.readOnly()

    private val _onErrorLiveData = SingleLiveEvent<Unit>()
    val onErrorLiveData = _onErrorLiveData.readOnly()

    private val downloadReceiver = object : BroadcastReceiver() {
        override fun onReceive(context: Context?, intent: Intent) {
            onReceive(context)
        }
    }

    @SuppressLint("Range")
    private fun onReceive(context: Context?) {
        if (downloadId != 0L) {
            val cursor =
                downloadManager.query(DownloadManager.Query().setFilterById(downloadId))
            if (cursor.moveToFirst()) {
                when (cursor.getInt(cursor.getColumnIndex(DownloadManager.COLUMN_STATUS))) {
                    DownloadManager.STATUS_SUCCESSFUL -> {
                        logDebug("downloadFile status STATUS_SUCCESSFUL", TAG)
                        _onReadyLiveData.call()
                    }

                    DownloadManager.STATUS_FAILED -> {
                        logError("downloadFile status STATUS_FAILED", TAG)
                        _onErrorLiveData.call()
                    }

                    DownloadManager.STATUS_PENDING -> {
                        logError("downloadFile status STATUS_PENDING", TAG)
                        _onErrorLiveData.call()
                    }

                    DownloadManager.STATUS_PAUSED -> {
                        logError("downloadFile status STATUS_PAUSED", TAG)
                        _onErrorLiveData.call()
                    }
                }
            }
        }
        context?.unregisterReceiver(downloadReceiver)
        downloadId = 0L
    }

    fun downloadFile(
        context: Context,
        url: String,
    ) {
        logDebug("downloadFile url: $url", TAG)
        try {
            val token = preferences.readApplicationInfo()?.accessToken
            val title = "document_${System.currentTimeMillis()}.pdf"
            val request = DownloadManager.Request(url.toUri())
                .setAllowedNetworkTypes(DownloadManager.Request.NETWORK_WIFI or DownloadManager.Request.NETWORK_MOBILE)
                .setTitle(title)
                .setDescription(title)
                .addRequestHeader("Authorization", "Bearer $token")
                .setNotificationVisibility(DownloadManager.Request.VISIBILITY_VISIBLE_NOTIFY_COMPLETED)
                .setAllowedOverMetered(true)
                .setAllowedOverRoaming(false)
                .setDestinationInExternalPublicDir(Environment.DIRECTORY_DOWNLOADS, title)
            downloadId = downloadManager.enqueue(request)
            if (downloadId != 0L) {
                val filter = IntentFilter(DownloadManager.ACTION_DOWNLOAD_COMPLETE)
                ContextCompat.registerReceiver(
                    context,
                    downloadReceiver,
                    filter,
                    ContextCompat.RECEIVER_EXPORTED
                )
            } else {
                _onErrorLiveData.call()
            }
        } catch (e: Exception) {
            logError("downloadFile Exception: ${e.message}", e, TAG)
            _onErrorLiveData.call()
        }
    }
}