/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.mappers.database.notifications.channels

import com.digitall.eid.data.mappers.base.BaseReverseMapper
import com.digitall.eid.data.models.database.notifications.channels.NotificationChannelSelectedEntity

class NotificationChannelSelectedEntityMapper :
    BaseReverseMapper<NotificationChannelSelectedEntity, String>() {

    override fun map(from: NotificationChannelSelectedEntity): String {
        return from.id
    }

    override fun reverse(to: String): NotificationChannelSelectedEntity {
        return NotificationChannelSelectedEntity(
            id = to
        )
    }

}