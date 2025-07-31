/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.models.notifications.notifications

import com.digitall.eid.domain.models.common.SelectionState

data class NotificationsModel(
    val pageIndex: Int?,
    val totalItems: Int?,
    val data: List<NotificationModel>?,
)

data class NotificationModel(
    val id: String,
    val name: String?,
    val isOpened: Boolean,
    val modifiedOn: String?,
    val modifiedBy: String?,
    val isApproved: Boolean?,
    val isDeleted: Boolean?,
    val englishName: String?,
    val bulgarianName: String?,
    var selectionState: SelectionState?,
    val events: List<NotificationEventModel>?,
)