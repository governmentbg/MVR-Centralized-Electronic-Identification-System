/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.repository.common

import com.digitall.eid.domain.models.common.ApplicationEnvironment
import com.digitall.eid.domain.models.common.ApplicationInfo
import com.digitall.eid.domain.models.common.ApplicationLanguage
import com.digitall.eid.domain.models.common.ApplicationPin
import com.digitall.eid.domain.models.common.ApplicationCredentials
import com.digitall.eid.domain.models.firebase.FirebaseToken

interface PreferencesRepository {

    fun saveApplicationInfo(value: ApplicationInfo)

    fun readApplicationInfo(): ApplicationInfo?

    fun readApplicationLanguage(): ApplicationLanguage?

    fun saveApplicationLanguage(language: ApplicationLanguage)

    fun saveFirebaseToken(value: FirebaseToken)

    fun readFirebaseToken(): FirebaseToken?

    fun logoutFromPreferences()

    fun logoutFromPreferencesFully()

    fun saveEnvironment(environment: ApplicationEnvironment)

    fun readEnvironment(): ApplicationEnvironment?

    fun saveCredentials(credentials: ApplicationCredentials)

    fun readCredentials(): ApplicationCredentials?

    fun saveApplicationPin(pin: ApplicationPin)

    fun readApplicationPin(): ApplicationPin?

    fun removeApplicationPin()

}