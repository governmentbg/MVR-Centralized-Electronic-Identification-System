package com.digitall.eid.utils

import com.digitall.eid.domain.models.firebase.FirebaseToken
import com.digitall.eid.domain.repository.common.PreferencesRepository
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.readOnly
import com.digitall.eid.extensions.setValueOnMainThread
import com.google.firebase.messaging.RemoteMessage
import org.koin.core.component.KoinComponent
import org.koin.core.component.inject

class AppFirebaseMessagingServiceHelper: KoinComponent {

    companion object {
        private const val TAG = "AppFirebaseMessagingServiceHelperTag"
    }

    private val preferences: PreferencesRepository by inject()

    private val _newFirebaseMessageLiveData = SingleLiveEvent<RemoteMessage>()
    val newFirebaseMessageLiveData = _newFirebaseMessageLiveData.readOnly()

    private val _newTokenEventLiveData = SingleLiveEvent<Unit>()
    val newTokenEventLiveData = _newTokenEventLiveData.readOnly()


    fun onNewToken(token: String) {
        if (token.isEmpty()) {
            logError("onNewToken token.isNullOrEmpty()", TAG)
            return
        }
        logDebug("onNewToken token: $token", TAG)
        val tokenModel =  FirebaseToken(token = token)
        preferences.saveFirebaseToken(
            value = tokenModel
        )
        _newTokenEventLiveData.callOnMainThread()
    }

    fun onMessageReceived(message: RemoteMessage) {
        logDebug(
            "onMessageReceived title: ${message.notification?.title} body:${message.notification?.body}",
            TAG
        )
        if (message.notification == null) {
            logError("onMessageReceived message.notification == null", TAG)
            return
        }
        if (message.notification?.title.isNullOrEmpty()) {
            logError("onMessageReceived message.notification?.title.isNullOrEmpty()", TAG)
            return
        }
        if (message.notification?.body.isNullOrEmpty()) {
            logError("onMessageReceived message.notification?.body.isNullOrEmpty()", TAG)
            return
        }
        if (message.data.isEmpty()) {
            logError("onMessageReceived message.data.isEmpty()", TAG)
        }
        _newFirebaseMessageLiveData.setValueOnMainThread(message)
    }
}