package com.digitall.eid.domain.models.notifications.channels

data class NotificationsChannelsModel(
    val channels: List<NotificationChannelModel>? = null,
    val enabledChannels: List<String>? = null,
) {
    val isValid: Boolean
        get() = channels.isNullOrEmpty().not()
}
