/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.models.notifications.channels

data class NotificationChannelsSelectedModel(
    val pageIndex: Int?,
    val totalItems: Int?,
    val data: List<String>?,
)
