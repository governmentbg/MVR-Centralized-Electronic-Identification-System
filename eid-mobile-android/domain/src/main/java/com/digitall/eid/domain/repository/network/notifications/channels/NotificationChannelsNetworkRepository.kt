/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.repository.network.notifications.channels

import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.notifications.channels.NotificationChannelsModel
import com.digitall.eid.domain.models.notifications.channels.NotificationChannelsSelectedModel
import kotlinx.coroutines.flow.Flow

interface NotificationChannelsNetworkRepository {

    fun getNotificationChannels(
        pageSize: Int?,
        pageIndex: Int?,
        channelName: String?,
    ): Flow<ResultEmittedData<NotificationChannelsModel>>

    fun getSelectedNotificationChannels(
        pageSize: Int?,
        pageIndex: Int?,
    ): Flow<ResultEmittedData<NotificationChannelsSelectedModel>>

    fun setSelectedNotificationChannels(
        selectedChannelsIDs: List<String>
    ): Flow<ResultEmittedData<Unit>>

}