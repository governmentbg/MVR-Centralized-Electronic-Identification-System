/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.database

import androidx.room.Database
import androidx.room.RoomDatabase
import androidx.room.TypeConverters
import com.digitall.eid.data.database.converters.BigDecimalToStringConverter
import com.digitall.eid.data.database.converters.DateToLongConverter
import com.digitall.eid.data.database.converters.StringListConverter
import com.digitall.eid.data.database.dao.notifications.channels.NotificationChannelsDao
import com.digitall.eid.data.database.dao.notifications.notifications.NotificationsDao
import com.digitall.eid.data.database.dao.PermissionsDao
import com.digitall.eid.data.models.database.notifications.channels.NotificationChannelEntity
import com.digitall.eid.data.models.database.notifications.notifications.NotificationEntity
import com.digitall.eid.data.models.database.notifications.notifications.NotificationEventEntity
import com.digitall.eid.data.models.database.PermissionsEntity
import com.digitall.eid.data.models.database.notifications.channels.NotificationChannelSelectedEntity
import com.digitall.eid.data.models.database.notifications.notifications.NotificationNotSelectedEntity

@Database(
    entities = [
        PermissionsEntity::class,
        NotificationEntity::class,
        NotificationEventEntity::class,
        NotificationChannelEntity::class,
        NotificationNotSelectedEntity::class,
        NotificationChannelSelectedEntity::class,
    ],
    version = 1,
    exportSchema = false
)

@TypeConverters(
    value = [
        BigDecimalToStringConverter::class,
        DateToLongConverter::class,
        StringListConverter::class,
    ]
)

abstract class AppDatabase : RoomDatabase() {

    abstract fun getPermissionsDao(): PermissionsDao

    abstract fun getNotificationChannelsDao(): NotificationChannelsDao

    abstract fun getNotificationsDao(): NotificationsDao

}