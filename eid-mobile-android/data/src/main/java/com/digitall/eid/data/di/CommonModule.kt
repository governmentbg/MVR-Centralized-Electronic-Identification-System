/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.di

import com.digitall.eid.data.utils.CryptographyHelperImpl
import com.digitall.eid.domain.utils.CryptographyHelper
import org.koin.android.ext.koin.androidApplication
import org.koin.dsl.module

val commonModule = module {

    single<CryptographyHelper> {
        CryptographyHelperImpl(
            context = androidApplication(),
        )
    }

}