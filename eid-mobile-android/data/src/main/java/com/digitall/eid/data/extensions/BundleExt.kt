/**
 * The backward compatible version of [Bundle.getSerializable] method.
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 */
package com.digitall.eid.data.extensions

import android.os.Build
import android.os.Bundle
import android.os.Parcelable

/**
 * The backward compatible version of [Bundle.getParcelable] method.
 */
inline fun <reified T : Parcelable> Bundle.getParcelableCompat(key: String): T? {
    return if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.TIRAMISU) {
        getParcelable(key, T::class.java)
    } else {
        @Suppress("DEPRECATION")
        getParcelable(key) as? T
    }
}

