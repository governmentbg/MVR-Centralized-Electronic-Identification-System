/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.extensions

import android.util.Base64
import com.digitall.eid.domain.utils.LogUtil.logError
import java.security.Key

private const val TAG = "TextExtTag"

fun String.toBase64(): String {
    return try {
        Base64.encodeToString(toByteArray(charset("UTF-8")), Base64.NO_WRAP)
    } catch (e: Exception) {
        logError("toBase64 Exception: ${e.message}", e, TAG)
        ""
    }
}

fun Key.toBase64(): String {
    return try {
        Base64.encodeToString(encoded, Base64.NO_WRAP)
    } catch (e: Exception) {
        logError("toBase64 Exception: ${e.message}", e, TAG)
        ""
    }
}

fun ByteArray.toBase64(): String {
    return try {
        Base64.encodeToString(this, Base64.NO_WRAP)
    } catch (e: Exception) {
        logError("toBase64 Exception: ${e.message}", e, TAG)
        ""
    }
}

