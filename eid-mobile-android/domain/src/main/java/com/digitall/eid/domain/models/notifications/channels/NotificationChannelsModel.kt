/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.models.notifications.channels

data class NotificationChannelsModel(
    val pageIndex: Int?,
    val totalItems: Int?,
    val data: List<NotificationChannelModel>?,
)

data class NotificationChannelModel(
    val id: String,
    val name: String?,
    val price: String?,
    val infoUrl: String?,
    val isEnabled: Boolean?,
    val translations: List<NotificationChannelTranslationModel>?,
)

data class NotificationChannelTranslationModel(
    val language: String?,
    val name: String?,
    val description: String?,
)
