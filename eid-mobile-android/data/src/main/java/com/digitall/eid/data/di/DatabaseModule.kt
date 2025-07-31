/**
 * single<Class>(named("name")){Class()} -> for creating a specific instance in module
 * single<Class1>{Class1(get<Class2>(named("name")))} -> for creating a specific instance in module
 * val nameOfVariable: Class by inject(named("name")) -> for creating a specific instance in class
 * get<Class>{parametersOf("param")} -> parameter passing in module
 * single<Class>{(param: String)->Class(param)} -> parameter passing in module
 * val name: Class by inject(parameters={parametersOf("param")}) -> parameter passing in class
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 */
package com.digitall.eid.data.di

import androidx.room.Room
import com.digitall.eid.data.BuildConfig.DATABASE_NAME
import com.digitall.eid.data.BuildConfig.PROPERTY_USER_PASSWORD
import com.digitall.eid.data.database.AppDatabase
import net.sqlcipher.database.SQLiteDatabase
import net.sqlcipher.database.SupportFactory
import org.koin.android.ext.koin.androidContext
import org.koin.dsl.module

val databaseModule = module {

    single<AppDatabase> {
        try {
            val passPhrase = getKoin().getProperty(PROPERTY_USER_PASSWORD, "")
            val passPhraseBytes: ByteArray = SQLiteDatabase.getBytes(passPhrase.toCharArray())
            val factory = SupportFactory(passPhraseBytes)
            Room.databaseBuilder(androidContext(), AppDatabase::class.java, DATABASE_NAME)
                .openHelperFactory(factory)
                .fallbackToDestructiveMigration(false)
                .build().also {
                    // Check if DB create correctly. If no - will be exception.
                    it.openHelper.readableDatabase.isDatabaseIntegrityOk
                }
        } catch (e: Exception) {
            Room.databaseBuilder(androidContext(), AppDatabase::class.java, DATABASE_NAME)
                .fallbackToDestructiveMigration(false)
                .build()
        }
    }

}