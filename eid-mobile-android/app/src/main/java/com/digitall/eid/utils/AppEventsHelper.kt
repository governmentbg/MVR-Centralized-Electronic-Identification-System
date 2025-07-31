package com.digitall.eid.utils

import android.os.Bundle
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.models.common.StringSource
import com.google.firebase.messaging.RemoteMessage
import org.koin.core.component.KoinComponent
import org.koin.core.component.inject

class AppEventsHelper: KoinComponent {

    companion object {
        private const val TAG = "AppEventsHelperTag"
    }

    private val notificationHelper: NotificationHelper by inject()

    fun onNewFirebaseMessage(message: RemoteMessage) {
        logDebug("onNewFirebaseMessage", TAG)
        when {
            message.data.isEmpty() -> notificationHelper.showNotificationOnMainThread(
                title = StringSource(message.notification?.title),
                content = StringSource(message.notification?.body),
            )

            else -> {
                val bundle = Bundle().apply {
                    message.data.forEach { (key, value) ->
                        putString(key, value)
                    }
                }
                notificationHelper.showNotificationOnMainThread(
                    bundle = bundle,
                    title = StringSource(message.notification?.title),
                    content = StringSource(message.notification?.body),
                )
            }
        }

    }
}