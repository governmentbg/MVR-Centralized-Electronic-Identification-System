/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.models.network.notifications.channels

import com.google.gson.annotations.SerializedName

data class NotificationChannelsGetResponse(
    @SerializedName("pageIndex") val pageIndex: Int?,
    @SerializedName("totalItems") val totalItems: Int?,
    @SerializedName("data") val data: List<NotificationChannelResponse>?,
) 

data class NotificationChannelResponse(
    @SerializedName("id") val id: String,
    @SerializedName("name") val name: String?,
    @SerializedName("price") val price: String?,
    @SerializedName("infoUrl") val infoUrl: String?,
    @SerializedName("translations") val translations: List<NotificationChannelTranslationResponse>?,
)