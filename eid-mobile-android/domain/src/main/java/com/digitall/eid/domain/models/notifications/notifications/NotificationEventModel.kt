/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.models.notifications.notifications

data class NotificationEventModel(
    val id: String,
    val code: String?,
    val parentId: String?,
    val modifiedOn: String?,
    val modifiedBy: String?,
    val isDeleted: Boolean?,
    var isSelected: Boolean?,
    val englishName: String?,
    val isMandatory: Boolean?,
    val bulgarianName: String?,
)
