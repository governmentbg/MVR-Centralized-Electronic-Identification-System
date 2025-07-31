/**
 * This class will provide a nearby active context.
 * Usually, its should be the visible activity context.
 * In the application start this class contains an application context.
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 */
package com.digitall.eid.utils

import android.content.Context
import android.graphics.drawable.Drawable
import androidx.annotation.DrawableRes
import androidx.annotation.StringRes
import androidx.core.content.ContextCompat
import androidx.core.os.ConfigurationCompat
import java.util.Locale

class CurrentContext(private var context: Context) {

    private var currentLocale: Locale = Locale.getDefault()

    /**
     * Attach the current [context].
     * Should be called in activity onCreate().
     */
    fun attachBaseContext(base: Context) {
        this.context = base
        currentLocale = getCurrentLocale()
    }

    fun get() = context

    fun getString(@StringRes resId: Int): String {
        return context.getString(resId)
    }

    fun getString(@StringRes resId: Int, secondaryText: String): String {
        return context.getString(resId, secondaryText)
    }

    fun getString(@StringRes resId: Int, formatArgs: List<Any>): String {
        return  context.getString(resId, *(formatArgs.toTypedArray()))
    }

    fun getDrawable(@DrawableRes resId: Int): Drawable? {
        return ContextCompat.getDrawable(context, resId)
    }

    fun getLocale() = currentLocale

    /**
     * The method returns the correct current locale.
     * The correct locale is the locale from the list of locales on the device
     * that supports by this application. If the correct language is not found,
     * we always choose the default language - English (GB).
     */
    private fun getCurrentLocale(): Locale {
        var correctLocale: Locale? = null
        val locales = ConfigurationCompat.getLocales(context.resources.configuration)
        for (i in 0 until locales.size()) {
            // If the new locale is added this part should be increased.
            if (locales[i] == Locale.forLanguageTag("en_GB")) {
                correctLocale = locales[i]
            }
        }
        return correctLocale ?: Locale.forLanguageTag("en_GB")
    }

}