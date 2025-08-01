/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.extensions

private const val TAG = "TextExtTag"

inline fun String?.notEmpty(
    crossinline text: (String) -> Unit,
) {
    if (!this.isNullOrEmpty()) {
        text(this)
    }
}

