/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.models.network.notifications.notifications

import com.google.gson.annotations.SerializedName

data class NotificationsGetResponse(
    @SerializedName("pageIndex") val pageIndex: Int?,
    @SerializedName("totalItems") val totalItems: Int?,
    @SerializedName("data") val data: List<NotificationDataResponse>?,
) 

data class NotificationDataEventResponse(
    @SerializedName("id") val id: String?,
    @SerializedName("code") val code: String?,
    @SerializedName("modifiedOn") val modifiedOn: String?,
    @SerializedName("modifiedBy") val modifiedBy: String?,
    @SerializedName("isMandatory") val isMandatory: Boolean?,
    @SerializedName("isDeleted") val isDeleted: Boolean?,
    @SerializedName("translations") val translations: List<NotificationDataEventTranslationResponse>?,
)

data class NotificationDataEventTranslationResponse(
    @SerializedName("language") val language: String?,
    @SerializedName("shortDescription") val shortDescription: String?,
    @SerializedName("description") val description: String?,
)

data class NotificationDataResponse(
    @SerializedName("id") val id: String?,
    @SerializedName("name") val name: String?,
    @SerializedName("modifiedOn") val modifiedOn: String?,
    @SerializedName("modifiedBy") val modifiedBy: String?,
    @SerializedName("isApproved") val isApproved: Boolean?,
    @SerializedName("isDeleted") val isDeleted: Boolean?,
    @SerializedName("translations") val translations: List<NotificationDataTranslationResponse>?,
    @SerializedName("events") val events: List<NotificationDataEventResponse>?,
)

data class NotificationDataTranslationResponse(
    @SerializedName("language") val language: String?,
    @SerializedName("name") val name: String?,
)