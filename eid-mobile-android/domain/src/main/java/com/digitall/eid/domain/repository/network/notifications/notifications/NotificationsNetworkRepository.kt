/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.repository.network.notifications.notifications

import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.notifications.notifications.NotificationsNotSelectedModel
import com.digitall.eid.domain.models.notifications.notifications.NotificationsModel
import kotlinx.coroutines.flow.Flow

interface NotificationsNetworkRepository {

    fun getNotifications(
        pageSize: Int?,
        pageIndex: Int?,
        systemName: String?,
        includeDeleted: Boolean?,
    ): Flow<ResultEmittedData<NotificationsModel>>

    fun getNotSelectedNotifications(
        pageSize: Int?,
        pageIndex: Int?,
    ): Flow<ResultEmittedData<NotificationsNotSelectedModel>>

    fun setNotSelectedNotifications(
        notSelectedNotificationsIDs: List<String>
    ): Flow<ResultEmittedData<Unit>>

}