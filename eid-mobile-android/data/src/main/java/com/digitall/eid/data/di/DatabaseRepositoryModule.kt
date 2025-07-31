/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.di

import com.digitall.eid.data.repository.database.notifications.notifications.NotificationsDatabaseRepositoryImpl
import com.digitall.eid.domain.repository.database.notifications.notifications.NotificationsDatabaseRepository
import org.koin.dsl.module

val databaseRepositoryModule = module {

    single<NotificationsDatabaseRepository> {
        NotificationsDatabaseRepositoryImpl()
    }

}