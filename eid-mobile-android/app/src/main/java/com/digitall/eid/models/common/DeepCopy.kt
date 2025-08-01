/**
 * Simple interface to provide consistent method to markers and their descendants
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 */
package com.digitall.eid.models.common

fun interface DeepCopy<T> {
    fun deepCopy(): T
}