/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.di

import com.digitall.eid.data.database.AppDatabase
import com.digitall.eid.data.database.dao.PermissionsDao
import com.digitall.eid.data.database.dao.notifications.channels.NotificationChannelsDao
import com.digitall.eid.data.database.dao.notifications.notifications.NotificationsDao
import org.koin.dsl.module

val daoModule = module {

    factory<PermissionsDao> {
        get<AppDatabase>().getPermissionsDao()
    }

    factory<NotificationChannelsDao> {
        get<AppDatabase>().getNotificationChannelsDao()
    }

    factory<NotificationsDao> {
        get<AppDatabase>().getNotificationsDao()
    }

}