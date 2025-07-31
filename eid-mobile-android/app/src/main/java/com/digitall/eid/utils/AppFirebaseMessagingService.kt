/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.utils

import com.google.firebase.messaging.FirebaseMessagingService
import com.google.firebase.messaging.RemoteMessage
import org.koin.android.ext.android.inject
import org.koin.core.component.KoinComponent

class AppFirebaseMessagingService : FirebaseMessagingService(), KoinComponent {

    companion object {
        private const val TAG = "AppFirebaseMessagingServiceTag"
    }

    private val firebaseMessagingServiceHelper: AppFirebaseMessagingServiceHelper by inject()

    override fun onNewToken(token: String) {
        firebaseMessagingServiceHelper.onNewToken(token)
    }

    override fun onMessageReceived(message: RemoteMessage) {
        super.onMessageReceived(message)
        firebaseMessagingServiceHelper.onMessageReceived(message)
    }

}