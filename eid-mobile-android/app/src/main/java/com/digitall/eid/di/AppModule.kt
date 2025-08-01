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
package com.digitall.eid.di

import android.app.DownloadManager
import android.app.NotificationManager
import android.content.ClipboardManager
import android.os.Handler
import android.os.Looper
import android.view.inputmethod.InputMethodManager
import androidx.core.content.ContextCompat
import com.digitall.eid.data.utils.CoroutineContextProvider
import com.digitall.eid.utils.ActivitiesCommonHelper
import com.digitall.eid.utils.AppEventsHelper
import com.digitall.eid.utils.AppFirebaseMessagingServiceHelper
import com.digitall.eid.utils.AuthenticationManager
import com.digitall.eid.utils.CurrentContext
import com.digitall.eid.utils.DownloadHelper
import com.digitall.eid.utils.InactivityTimer
import com.digitall.eid.utils.InactivityTimerImpl
import com.digitall.eid.utils.LocalizationManager
import com.digitall.eid.utils.LoginTimer
import com.digitall.eid.utils.LoginTimerImpl
import com.digitall.eid.utils.NotificationHelper
import com.digitall.eid.utils.PermissionsManager
import com.digitall.eid.utils.PermissionsManagerImpl
import com.digitall.eid.utils.RecyclerViewAdapterDataObserver
import com.digitall.eid.utils.ScreenshotsDetector
import com.digitall.eid.utils.ScreenshotsDetectorImpl
import com.digitall.eid.utils.SocialNetworksHelper
import com.digitall.eid.utils.SupportBiometricManager
import com.digitall.eid.utils.SupportBiometricManagerImpl
import org.koin.android.ext.koin.androidApplication
import org.koin.android.ext.koin.androidContext
import org.koin.dsl.module

val appModule = module {

    single<ClipboardManager> {
        ContextCompat.getSystemService(
            androidContext(),
            ClipboardManager::class.java
        ) as ClipboardManager
    }

    single<DownloadManager> {
        ContextCompat.getSystemService(
            androidContext(),
            DownloadManager::class.java
        ) as DownloadManager
    }

    single<InputMethodManager> {
        ContextCompat.getSystemService(
            androidContext(),
            InputMethodManager::class.java
        ) as InputMethodManager
    }

    single<NotificationManager> {
        ContextCompat.getSystemService(
            androidContext(),
            NotificationManager::class.java
        ) as NotificationManager
    }

    single<CoroutineContextProvider> {
        CoroutineContextProvider()
    }

    single<ActivitiesCommonHelper> {
        ActivitiesCommonHelper()
    }

    single<NotificationHelper> {
        NotificationHelper()
    }

    single<AppFirebaseMessagingServiceHelper> {
        AppFirebaseMessagingServiceHelper()
    }

    single<AppEventsHelper> {
        AppEventsHelper()
    }

    single<DownloadHelper> {
        DownloadHelper()
    }

    single<ScreenshotsDetector> {
        ScreenshotsDetectorImpl()
    }

    single<Handler> {
        Handler(Looper.getMainLooper())
    }

    single<SocialNetworksHelper> {
        SocialNetworksHelper()
    }

    single<PermissionsManager> {
        PermissionsManagerImpl()
    }

    single<SupportBiometricManager> {
        SupportBiometricManagerImpl()
    }

    single<RecyclerViewAdapterDataObserver> {
        RecyclerViewAdapterDataObserver()
    }

    single<CurrentContext> {
        CurrentContext(
            context = androidContext()
        )
    }

    single<LocalizationManager> {
        LocalizationManager()
    }

    single<LoginTimer> {
        LoginTimerImpl()
    }

    single<InactivityTimer> {
        InactivityTimerImpl()
    }

    single<AuthenticationManager> {
        AuthenticationManager(
            application = androidApplication()
        )
    }

}