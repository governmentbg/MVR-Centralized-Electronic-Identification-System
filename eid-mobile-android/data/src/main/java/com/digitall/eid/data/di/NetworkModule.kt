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

import com.digitall.eid.data.models.network.authentication.response.AuthenticationResponse
import com.digitall.eid.data.models.network.base.ErrorApiResponse
import com.digitall.eid.data.network.utils.ArrayConverterFactory
import com.digitall.eid.data.network.utils.AuthenticationResponseDeserializer
import com.digitall.eid.data.network.utils.ErrorApiResponseDeserializer
import com.digitall.eid.data.network.utils.NullOrEmptyConverterFactory
import com.digitall.eid.domain.utils.LogUtil.logNetwork
import com.google.gson.Gson
import com.google.gson.GsonBuilder
import okhttp3.logging.HttpLoggingInterceptor
import org.koin.core.qualifier.named
import org.koin.dsl.module
import retrofit2.converter.gson.GsonConverterFactory
import retrofit2.converter.simplexml.*

private const val TAG = "NetworkModuleTag"

const val RETROFIT_BASE = "RETROFIT_BASE"
const val RETROFIT_KEYCLOAK_PG = "RETROFIT_KEYCLOAK_PG"
const val RETROFIT_PAN = "RETROFIT_PAN"
const val RETROFIT_RO = "RETROFIT_RO"
const val RETROFIT_SIGNING = "RETROFIT_SIGNING"
const val RETROFIT_PJS = "RETROFIT_PJS"
const val RETROFIT_MPOZEI = "RETROFIT_MPOZEI"
const val RETROFIT_ISCEIGW = "RETROFIT_ISCEIGW"
const val RETROFIT_RAEICEI = "RETROFIT_RAEICEI"

const val OKHTTP = "OKHTTP"

const val LOGGING_INTERCEPTOR = "LOGGING_INTERCEPTOR"
const val LOG_TO_FILE_INTERCEPTOR = "LOG_TO_FILE_INTERCEPTOR"

const val TIMEOUT = 60L

val networkModule = module {

    single<GsonConverterFactory> {
        GsonConverterFactory.create(get<Gson>())
    }

    @Suppress("DEPRECATION")
    single<SimpleXmlConverterFactory> {
        SimpleXmlConverterFactory.create()
    }

    single<NullOrEmptyConverterFactory> {
        NullOrEmptyConverterFactory()
    }

    single<Gson> {
        GsonBuilder()
            .registerTypeAdapter(ErrorApiResponse::class.java, ErrorApiResponseDeserializer())
            .registerTypeAdapter(
                AuthenticationResponse::class.java,
                AuthenticationResponseDeserializer()
            )
            .create()
    }

    single<ArrayConverterFactory> {
        ArrayConverterFactory()
    }

    single<HttpLoggingInterceptor>(named(LOGGING_INTERCEPTOR)) {
        HttpLoggingInterceptor().setLevel(HttpLoggingInterceptor.Level.BODY)
    }

    single<HttpLoggingInterceptor>(named(LOG_TO_FILE_INTERCEPTOR)) {
        val logging = HttpLoggingInterceptor {
            logNetwork(it)
        }
        logging.level = HttpLoggingInterceptor.Level.BODY
        logging
    }

}