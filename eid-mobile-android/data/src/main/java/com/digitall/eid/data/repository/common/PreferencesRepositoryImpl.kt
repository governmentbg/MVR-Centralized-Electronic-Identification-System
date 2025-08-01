/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.repository.common

import com.digitall.eid.data.BuildConfig.PROPERTY_KEY_APP_CREDENTIALS
import com.digitall.eid.data.BuildConfig.PROPERTY_KEY_APP_ENVIRONMENT
import com.digitall.eid.data.BuildConfig.PROPERTY_KEY_APP_LANGUAGE
import com.digitall.eid.data.BuildConfig.PROPERTY_KEY_APP_PIN
import com.digitall.eid.data.BuildConfig.PROPERTY_KEY_FIREBASE_TOKEN
import com.digitall.eid.data.BuildConfig.PROPERTY_KEY_PIN_CODE
import com.digitall.eid.data.utils.KeystorePreference
import com.digitall.eid.domain.models.common.ApplicationEnvironment
import com.digitall.eid.domain.models.common.ApplicationInfo
import com.digitall.eid.domain.models.common.ApplicationLanguage
import com.digitall.eid.domain.models.common.ApplicationPin
import com.digitall.eid.domain.models.common.ApplicationCredentials
import com.digitall.eid.domain.models.firebase.FirebaseToken
import com.digitall.eid.domain.repository.common.PreferencesRepository
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.google.gson.Gson
import org.koin.core.component.KoinComponent
import org.koin.core.component.inject

internal class PreferencesRepositoryImpl : PreferencesRepository, KoinComponent {

    companion object {
        private const val TAG = "PreferencesRepositoryTag"
    }

    private val preferences: KeystorePreference by inject()

    override fun saveApplicationInfo(value: ApplicationInfo) {
        logDebug("savePinCode value: $value", TAG)
        saveObject(value, PROPERTY_KEY_PIN_CODE)
    }

    override fun readApplicationInfo(): ApplicationInfo? {
        val value = readObject(ApplicationInfo::class.java, PROPERTY_KEY_PIN_CODE)
        logDebug("readPinCode value: $value", TAG)
        return value
    }

    override fun readApplicationLanguage(): ApplicationLanguage? {
        val value = readObject(ApplicationLanguage::class.java, PROPERTY_KEY_APP_LANGUAGE)
        logDebug("readApplicationLanguage value: $value", TAG)
        return value
    }

    override fun saveApplicationLanguage(language: ApplicationLanguage) {
        logDebug("saveApplicationLanguage value: $language", TAG)
        saveObject(language, PROPERTY_KEY_APP_LANGUAGE)
    }

    override fun saveFirebaseToken(value: FirebaseToken) {
        logDebug("saveFirebaseToken value: $value", TAG)
        saveObject(value, PROPERTY_KEY_FIREBASE_TOKEN)
    }

    override fun readFirebaseToken(): FirebaseToken? {
        val value = readObject(FirebaseToken::class.java, PROPERTY_KEY_FIREBASE_TOKEN)
        logDebug("readFirebaseToken value: $value", TAG)
        return value
    }

    override fun saveEnvironment(environment: ApplicationEnvironment) {
        logDebug("saveEnvironment value: $environment", TAG)
        saveObject(environment, PROPERTY_KEY_APP_ENVIRONMENT)
    }

    override fun readEnvironment(): ApplicationEnvironment? {
        val value = readObject(ApplicationEnvironment::class.java, PROPERTY_KEY_APP_ENVIRONMENT)
        logDebug("readEnvironment value: $value", TAG)
        return value
    }

    override fun saveCredentials(credentials: ApplicationCredentials) {
        logDebug("saveCredentials value: $credentials", TAG)
        saveObject(credentials, PROPERTY_KEY_APP_CREDENTIALS)
    }

    override fun readCredentials(): ApplicationCredentials? {
        val value = readObject(ApplicationCredentials::class.java, PROPERTY_KEY_APP_CREDENTIALS)
        logDebug("readCredentials value: $value", TAG)
        return value
    }

    override fun saveApplicationPin(pin: ApplicationPin) {
        logDebug("saveApplicationPin value: $pin", TAG)
        saveObject(pin, PROPERTY_KEY_APP_PIN)
    }

    override fun readApplicationPin(): ApplicationPin? {
        val value = readObject(ApplicationPin::class.java, PROPERTY_KEY_APP_PIN)
        logDebug("readApplicationPin value: $value", TAG)
        return value
    }

    override fun removeApplicationPin() {
        logDebug("removeApplicationPin", TAG)
        preferences.remove(PROPERTY_KEY_APP_PIN)
    }

    override fun logoutFromPreferences() {
        logDebug("logoutFromPreferences", TAG)
        preferences.remove(PROPERTY_KEY_PIN_CODE)
    }

    override fun logoutFromPreferencesFully() {
        logoutFromPreferences()
        logDebug("logoutFromPreferencesFully", TAG)
        preferences.remove(PROPERTY_KEY_FIREBASE_TOKEN)
    }

    private fun saveObject(dataObject: Any, key: String) {
        val objectJson = Gson().toJson(dataObject)
        preferences.save(key = key, value = objectJson)
    }

    private fun <T> readObject(baseClass: Class<T>, key: String): T? {
        val dataObject = preferences.get(key)
        return try {
            Gson().fromJson(dataObject, baseClass)
        } catch (e: Exception) {
            logError("readObject Exception: ${e.message}", e, TAG)
            null
        }
    }

}