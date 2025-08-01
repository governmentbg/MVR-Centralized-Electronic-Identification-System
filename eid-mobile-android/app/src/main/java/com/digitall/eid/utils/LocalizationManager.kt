/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.utils

import androidx.appcompat.app.AppCompatDelegate
import androidx.core.os.LocaleListCompat
import com.digitall.eid.domain.APPLICATION_LANGUAGE
import com.digitall.eid.domain.models.common.ApplicationLanguage
import com.digitall.eid.domain.repository.common.PreferencesRepository
import org.koin.core.component.KoinComponent
import org.koin.core.component.inject

class LocalizationManager : KoinComponent {

    companion object {
        private const val TAG = "LocalizationManagerTag"
    }

    private val preferences: PreferencesRepository by inject()

    fun tryApplyInitialApplicationLanguage() {
        AppCompatDelegate.getApplicationLocales()[0]?.language?.let { language ->
            APPLICATION_LANGUAGE = when {
                language.startsWith(ApplicationLanguage.BG.type) -> ApplicationLanguage.BG
                language.startsWith(ApplicationLanguage.EN.type) -> ApplicationLanguage.EN
                else -> ApplicationLanguage.BG
            }

        } ?: run {
            APPLICATION_LANGUAGE = preferences.readApplicationLanguage() ?: ApplicationLanguage.EN
        }
        preferences.saveApplicationLanguage(language = APPLICATION_LANGUAGE)
        AppCompatDelegate.setApplicationLocales(
            LocaleListCompat.forLanguageTags(
                APPLICATION_LANGUAGE.type
            )
        )
    }

    fun changeLanguage() {
        val newApplicationLanguageLocale = when (APPLICATION_LANGUAGE) {
            ApplicationLanguage.BG -> {
                APPLICATION_LANGUAGE = ApplicationLanguage.EN
                LocaleListCompat.forLanguageTags(ApplicationLanguage.EN.type)
            }

            ApplicationLanguage.EN -> {
                APPLICATION_LANGUAGE = ApplicationLanguage.BG
                LocaleListCompat.forLanguageTags(ApplicationLanguage.BG.type)
            }
        }

        preferences.saveApplicationLanguage(language = APPLICATION_LANGUAGE)
        AppCompatDelegate.setApplicationLocales(newApplicationLanguageLocale)
    }
}