/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.network.notifications

import com.digitall.eid.data.models.network.base.EmptyResponse
import com.digitall.eid.data.models.network.notifications.channels.NotificationChannelsGetResponse
import com.digitall.eid.data.models.network.notifications.channels.NotificationChannelsSelectedResponse
import com.digitall.eid.data.models.network.notifications.notifications.NotificationsNotSelectedResponse
import com.digitall.eid.data.models.network.notifications.notifications.NotificationsGetResponse
import retrofit2.Response
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.POST
import retrofit2.http.Query

interface NotificationsApi {

    @GET("pan/api/v1/NotificationChannels")
    suspend fun getNotificationChannels(
        @Query("pageSize") pageSize: Int?,
        @Query("pageIndex") pageIndex: Int?,
        @Query("channelName") channelName: String?,
    ): Response<NotificationChannelsGetResponse>

    @GET("pan/api/v1/NotificationChannels/selected")
    suspend fun getSelectedNotificationChannels(): Response<NotificationChannelsSelectedResponse>

    @POST("pan/api/v1/NotificationChannels/selection")
    suspend fun setSelectedNotificationChannels(
        @Body request: List<String>,
    ): Response<EmptyResponse>

    @GET("pan/api/v1/Notifications/deactivated")
    suspend fun getNotSelectedNotifications(
        @Query("pageSize") pageSize: Int?,
        @Query("pageIndex") pageIndex: Int?,
    ): Response<NotificationsNotSelectedResponse>

    @GET("pan/api/v1/Notifications")
    suspend fun getNotifications(
        @Query("pageSize") pageSize: Int?,
        @Query("pageIndex") pageIndex: Int?,
        @Query("systemName") systemName: String?,
        @Query("includeDeleted") includeDeleted: Boolean?,
    ): Response<NotificationsGetResponse>

    @POST("pan/api/v1/Notifications/deactivate")
    suspend fun setNotSelectedNotifications(
        @Body request: List<String>,
    ): Response<EmptyResponse>

}