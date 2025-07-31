/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid

import android.app.Application
import com.digitall.eid.data.BuildConfig.ANDROID_KEYSTORE
import com.digitall.eid.data.BuildConfig.BIOMETRIC_KEY_ALIAS
import com.digitall.eid.data.BuildConfig.CLIENT_ID
import com.digitall.eid.data.BuildConfig.CLIENT_ID_MPOZEI
import com.digitall.eid.data.BuildConfig.CLIENT_SECRET_AUTH
import com.digitall.eid.data.BuildConfig.CLIENT_SECRET_MPOZEI
import com.digitall.eid.data.BuildConfig.DATABASE_NAME
import com.digitall.eid.data.BuildConfig.ENCRYPTED_SHARED_PREFERENCES_NAME
import com.digitall.eid.data.BuildConfig.EVROTRUST_APPLICATION_NUMBER
import com.digitall.eid.data.BuildConfig.GRANT_TYPE_AUTH
import com.digitall.eid.data.BuildConfig.MASTER_PREFERENCES_KEY_ALIAS
import com.digitall.eid.data.BuildConfig.MASTER_SYMMETRIC_KEY_ALIAS
import com.digitall.eid.data.BuildConfig.PROPERTY_KEY_PIN_CODE
import com.digitall.eid.data.BuildConfig.PROPERTY_USER_PASSWORD
import com.digitall.eid.data.di.dataModules
import com.digitall.eid.di.appModules
import com.digitall.eid.domain.di.domainModules
import leakcanary.LeakCanary
import org.bouncycastle.jce.provider.BouncyCastleProvider
import org.koin.android.ext.koin.androidContext
import org.koin.android.ext.koin.androidLogger
import org.koin.core.context.startKoin
import org.koin.core.logger.Level
import java.security.Security

class App : Application() {

    companion object {
        private const val TAG = "AppTag"
    }

    init {
        Security.removeProvider(BouncyCastleProvider.PROVIDER_NAME)
        Security.addProvider(BouncyCastleProvider())
    }

    override fun onCreate() {
        super.onCreate()
        checkLocalProperties()
        setupKoin()
    }

    private fun setupKoin() {
        startKoin {
            androidContext(this@App)
            if (BuildConfig.DEBUG) {
                androidLogger(Level.ERROR)
            }
            allowOverride(override = true)
            modules(appModules, domainModules, dataModules)
        }
        configureLeakCanary()
    }

    private fun configureLeakCanary(isEnable: Boolean = false) {
        LeakCanary.config = LeakCanary.config.copy(dumpHeap = isEnable)
        LeakCanary.showLeakDisplayActivityLauncherIcon(isEnable)
    }

    private fun checkLocalProperties() {
        if (CLIENT_SECRET_AUTH.isEmpty() ||
            CLIENT_SECRET_MPOZEI.isEmpty() ||
            GRANT_TYPE_AUTH.isEmpty() ||
            CLIENT_ID.isEmpty() ||
            CLIENT_ID_MPOZEI.isEmpty() ||
            DATABASE_NAME.isEmpty() ||
            ENCRYPTED_SHARED_PREFERENCES_NAME.isEmpty() ||
            PROPERTY_USER_PASSWORD.isEmpty() ||
            EVROTRUST_APPLICATION_NUMBER.isEmpty() ||
            PROPERTY_KEY_PIN_CODE.isEmpty() ||
            ANDROID_KEYSTORE.isEmpty() ||
            MASTER_SYMMETRIC_KEY_ALIAS.isEmpty() ||
            BIOMETRIC_KEY_ALIAS.isEmpty() ||
            MASTER_PREFERENCES_KEY_ALIAS.isEmpty()
        ) {
            throw IllegalArgumentException("Some local properties argument is empty")
        }

    }

}