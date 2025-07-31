/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.utils

import android.Manifest
import android.app.NotificationChannel
import android.app.NotificationManager
import android.app.PendingIntent
import android.content.DialogInterface
import android.content.Intent
import android.content.pm.PackageManager
import android.media.AudioAttributes
import android.media.RingtoneManager
import android.os.Build
import android.os.Bundle
import android.os.Looper
import androidx.annotation.StringRes
import androidx.appcompat.app.AlertDialog
import androidx.core.app.ActivityCompat
import androidx.core.app.NotificationCompat
import androidx.core.app.NotificationManagerCompat
import com.digitall.eid.R
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.ui.activity.MainActivity
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import org.koin.core.component.KoinComponent
import org.koin.core.component.inject

class NotificationHelper : KoinComponent {

    companion object {
        private const val TAG = "NotificationHelperTag"
        private const val NOTIFICATION_CHANNEL_ID = "eid_notification_channel"
        private const val DEFAULT_NOTIFICATION_ID = 123
        private const val PENDING_INTENT_REQUEST_CODE = 123
    }

    private val currentContext: CurrentContext by inject()
    private val notificationManager: NotificationManager by inject()

    init {
        logDebug(TAG, "init")
        createNotificationChannel()
    }

    fun showNotificationOnMainThread(
        title: StringSource,
        content: StringSource,
        bundle: Bundle? = null,
        id: Int = DEFAULT_NOTIFICATION_ID,
        @StringRes buttonNameRes: Int? = null,
        requestCode: Int = PENDING_INTENT_REQUEST_CODE,
    ) {
        if (Looper.myLooper() == Looper.getMainLooper()) {
            showNotification(
                id = id,
                title = title,
                content = content,
                requestCode = requestCode,
                buttonNameRes = buttonNameRes,
            )
        } else {
            CoroutineScope(Dispatchers.Main).launch {
                showNotification(
                    id = id,
                    title = title,
                    content = content,
                    requestCode = requestCode,
                    buttonNameRes = buttonNameRes,
                )
            }
        }
    }

    private fun createNotificationChannel() {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
            logDebug(TAG, "createNotificationChannel")
            val name = currentContext.getString(R.string.channel_name)
            val descriptionText = currentContext.getString(R.string.channel_description)
            val importance = NotificationManager.IMPORTANCE_HIGH
            val sound = RingtoneManager.getDefaultUri(RingtoneManager.TYPE_NOTIFICATION)
            val attributes = AudioAttributes.Builder()
                .setUsage(AudioAttributes.USAGE_NOTIFICATION)
                .build()
            val channel = NotificationChannel(NOTIFICATION_CHANNEL_ID, name, importance)
            channel.description = descriptionText
            channel.setSound(sound, attributes)
            notificationManager.createNotificationChannel(channel)
        }
    }

    fun hideAllNotifications() {
        NotificationManagerCompat.from(currentContext.get()).cancelAll()
    }

    private fun showNotification(
        title: StringSource,
        content: StringSource,
        @StringRes buttonNameRes: Int? = null,
        id: Int = DEFAULT_NOTIFICATION_ID,
        requestCode: Int = PENDING_INTENT_REQUEST_CODE,
    ) {
        val titleString = title.getString(currentContext)
        val contentString = content.getString(currentContext)
        logDebug(TAG, "showNotification titleString: $titleString contentString: $contentString")
        val intent = Intent(
            currentContext.get(),
            MainActivity::class.java
        ).addFlags(Intent.FLAG_ACTIVITY_NEW_TASK)
        val pendingIntent = PendingIntent.getActivity(
            currentContext.get(),
            requestCode,
            intent,
            PendingIntent.FLAG_IMMUTABLE
        )
        val sound = RingtoneManager.getDefaultUri(RingtoneManager.TYPE_NOTIFICATION)
        val builder = if (buttonNameRes == null) {
            NotificationCompat.Builder(currentContext.get(), NOTIFICATION_CHANNEL_ID)
                .setSmallIcon(R.drawable.img_logo)
                .setContentTitle(titleString)
                .setContentText(contentString)
                .setSound(sound)
                .setPriority(NotificationCompat.PRIORITY_DEFAULT)
                .setAutoCancel(true)
                .setContentIntent(pendingIntent)
        } else {
            NotificationCompat.Builder(currentContext.get(), NOTIFICATION_CHANNEL_ID)
                .setSmallIcon(R.drawable.img_logo)
                .setContentTitle(titleString)
                .setContentText(contentString)
                .setSound(sound)
                .setPriority(NotificationCompat.PRIORITY_DEFAULT)
                .setAutoCancel(true)
                .setContentIntent(pendingIntent)
                .addAction(
                    R.drawable.img_logo,
                    currentContext.getString(buttonNameRes),
                    pendingIntent
                )
        }
        if ((ActivityCompat.checkSelfPermission(
                currentContext.get(),
                Manifest.permission.POST_NOTIFICATIONS
            ) != PackageManager.PERMISSION_GRANTED) || NotificationManagerCompat.from(currentContext.get())
                .areNotificationsEnabled().not()
        ) {
            return
        }
        
        NotificationManagerCompat.from(currentContext.get())
            .notify(id, builder.build())
    }

    private fun showAppNotificationsDialog(content: String) {
        logDebug(TAG, "showAppNotificationsDialog content: $content")
        val notificationsDialog = AlertDialog.Builder(currentContext.get())
            .setTitle(currentContext.getString(R.string.warning_label))
            .setMessage(currentContext.getString(R.string.notifications_window_label, content))
            .setPositiveButton(currentContext.getString(R.string.settings_label)) { dialog: DialogInterface, _: Int ->
                val intent = Intent()
                intent.setAction("android.settings.APP_NOTIFICATION_SETTINGS")
                intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK)
                intent.putExtra(
                    "android.provider.extra.APP_PACKAGE",
                    currentContext.get().packageName
                )
                currentContext.get().startActivity(intent)
                dialog.dismiss()
            }
            .setNegativeButton(currentContext.getString(R.string.cancel)) { _: DialogInterface?, _: Int ->
                logDebug(TAG, "showAppNotificationsDialog setNegativeButton")
            }
            .setOnCancelListener { _: DialogInterface? ->
                logDebug(TAG, "showAppNotificationsDialog setOnCancelListener")
            }
        try {
            notificationsDialog.create()
            notificationsDialog.show()
        } catch (e: Exception) {
            logError("showAppNotificationsDialog Exception: ${e.message}", e, TAG)
        }
    }

}