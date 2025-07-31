/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.di

import com.digitall.eid.data.network.utils.NullOrEmptyConverterFactory
import com.digitall.eid.domain.ENVIRONMENT
import okhttp3.OkHttpClient
import org.koin.core.qualifier.named
import org.koin.dsl.module
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory
import retrofit2.converter.simplexml.*

@Suppress("DEPRECATION")
val retrofitModule = module {

    single<Retrofit>(named(RETROFIT_BASE)) {
        Retrofit.Builder()
            .baseUrl(ENVIRONMENT.urlBase)
            .client(get<OkHttpClient>(named(OKHTTP)))
//            .addConverterFactory(get<ArrayConverterFactory>())
            .addConverterFactory(get<NullOrEmptyConverterFactory>())
            .addConverterFactory(get<GsonConverterFactory>())
            .addConverterFactory(get<SimpleXmlConverterFactory>())
            .build()
    }

    single<Retrofit>(named(RETROFIT_KEYCLOAK_PG)) {
        Retrofit.Builder()
            .baseUrl(ENVIRONMENT.urlKeycloakPg)
            .client(get<OkHttpClient>(named(OKHTTP)))
//            .addConverterFactory(get<ArrayConverterFactory>())
            .addConverterFactory(get<NullOrEmptyConverterFactory>())
            .addConverterFactory(get<GsonConverterFactory>())
            .addConverterFactory(get<SimpleXmlConverterFactory>())
            .build()
    }

    single<Retrofit>(named(RETROFIT_PAN)) {
        Retrofit.Builder()
            .baseUrl(ENVIRONMENT.urlPan)
            .client(get<OkHttpClient>(named(OKHTTP)))
//            .addConverterFactory(get<ArrayConverterFactory>())
            .addConverterFactory(get<NullOrEmptyConverterFactory>())
            .addConverterFactory(get<GsonConverterFactory>())
            .addConverterFactory(get<SimpleXmlConverterFactory>())
            .build()
    }

    single<Retrofit>(named(RETROFIT_RO)) {
        Retrofit.Builder()
            .baseUrl(ENVIRONMENT.urlRo)
            .client(get<OkHttpClient>(named(OKHTTP)))
//            .addConverterFactory(get<ArrayConverterFactory>())
            .addConverterFactory(get<NullOrEmptyConverterFactory>())
            .addConverterFactory(get<GsonConverterFactory>())
            .addConverterFactory(get<SimpleXmlConverterFactory>())
            .build()
    }
    single<Retrofit>(named(RETROFIT_SIGNING)) {
        Retrofit.Builder()
            .baseUrl(ENVIRONMENT.urlSigning)
            .client(get<OkHttpClient>(named(OKHTTP)))
//            .addConverterFactory(get<ArrayConverterFactory>())
            .addConverterFactory(get<NullOrEmptyConverterFactory>())
            .addConverterFactory(get<GsonConverterFactory>())
            .addConverterFactory(get<SimpleXmlConverterFactory>())
            .build()
    }
    single<Retrofit>(named(RETROFIT_PJS)) {
        Retrofit.Builder()
            .baseUrl(ENVIRONMENT.urlPjs)
            .client(get<OkHttpClient>(named(OKHTTP)))
//            .addConverterFactory(get<ArrayConverterFactory>())
            .addConverterFactory(get<NullOrEmptyConverterFactory>())
            .addConverterFactory(get<GsonConverterFactory>())
            .addConverterFactory(get<SimpleXmlConverterFactory>())
            .build()
    }
    single<Retrofit>(named(RETROFIT_MPOZEI)) {
        Retrofit.Builder()
            .baseUrl(ENVIRONMENT.urlMpozei)
            .client(get<OkHttpClient>(named(OKHTTP)))
//            .addConverterFactory(get<ArrayConverterFactory>())
            .addConverterFactory(get<NullOrEmptyConverterFactory>())
            .addConverterFactory(get<GsonConverterFactory>())
            .addConverterFactory(get<SimpleXmlConverterFactory>())
            .build()
    }

    single<Retrofit>(named(RETROFIT_ISCEIGW)) {
        Retrofit.Builder()
            .baseUrl(ENVIRONMENT.urlIsceigw)
            .client(get<OkHttpClient>(named(OKHTTP)))
//            .addConverterFactory(get<ArrayConverterFactory>())
            .addConverterFactory(get<NullOrEmptyConverterFactory>())
            .addConverterFactory(get<GsonConverterFactory>())
            .addConverterFactory(get<SimpleXmlConverterFactory>())
            .build()
    }

    single<Retrofit>(named(RETROFIT_RAEICEI)) {
        Retrofit.Builder()
            .baseUrl(ENVIRONMENT.urlRaeicei)
            .client(get<OkHttpClient>(named(OKHTTP)))
//            .addConverterFactory(get<ArrayConverterFactory>())
            .addConverterFactory(get<NullOrEmptyConverterFactory>())
            .addConverterFactory(get<GsonConverterFactory>())
            .addConverterFactory(get<SimpleXmlConverterFactory>())
            .build()
    }

}