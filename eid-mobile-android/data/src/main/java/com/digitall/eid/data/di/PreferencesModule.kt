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

import com.digitall.eid.data.repository.common.PreferencesRepositoryImpl
import com.digitall.eid.data.utils.KeystoreHelper
import com.digitall.eid.data.utils.KeystorePreference
import com.digitall.eid.domain.repository.common.PreferencesRepository
import org.koin.android.ext.koin.androidApplication
import org.koin.dsl.module

val preferencesModule = module {

    single<KeystoreHelper> {
        KeystoreHelper()
    }

    single<KeystorePreference> {
        KeystorePreference(
            application = androidApplication()
        )
    }

    single<PreferencesRepository> {
        PreferencesRepositoryImpl()
    }

}