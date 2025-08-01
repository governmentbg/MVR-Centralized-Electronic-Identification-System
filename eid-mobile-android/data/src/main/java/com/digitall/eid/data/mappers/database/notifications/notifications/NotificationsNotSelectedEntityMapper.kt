/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.mappers.database.notifications.notifications

import com.digitall.eid.data.mappers.base.BaseReverseMapper
import com.digitall.eid.data.models.database.notifications.notifications.NotificationNotSelectedEntity

class NotificationsNotSelectedEntityMapper :
    BaseReverseMapper<NotificationNotSelectedEntity, String>() {

    companion object {
        private const val TAG = "NotificationsNotSelectedEntityMapperTag"
    }

    override fun map(from: NotificationNotSelectedEntity): String {
        return from.id
    }

    override fun reverse(to: String): NotificationNotSelectedEntity {
        return NotificationNotSelectedEntity(
            id = to
        )
    }


}