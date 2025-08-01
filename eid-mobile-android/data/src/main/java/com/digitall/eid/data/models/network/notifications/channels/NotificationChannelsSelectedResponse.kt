/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.models.network.notifications.channels

import com.google.gson.annotations.SerializedName

data class NotificationChannelsSelectedResponse(
    @SerializedName("pageIndex") val pageIndex: Int?,
    @SerializedName("totalItems") val totalItems: Int?,
    @SerializedName("data") val data: List<String>?,
) 

data class NotificationChannelTranslationResponse(
    @SerializedName("language") val language: String?,
    @SerializedName("name") val name: String?,
    @SerializedName("description") val description: String?,
)