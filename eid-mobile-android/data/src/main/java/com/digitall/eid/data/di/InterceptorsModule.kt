/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.di

import com.digitall.eid.data.network.utils.ContentTypeInterceptor
import com.digitall.eid.data.network.utils.HeaderInterceptor
import com.digitall.eid.data.network.utils.MockInterceptor
import org.koin.dsl.module

val interceptorsModule = module {

    single<HeaderInterceptor> {
        HeaderInterceptor()
    }

    single<MockInterceptor> {
        MockInterceptor()
    }

    single<ContentTypeInterceptor> {
        ContentTypeInterceptor()
    }

}