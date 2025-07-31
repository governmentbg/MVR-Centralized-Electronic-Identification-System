/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.extensions

import android.os.Handler
import android.os.Looper
import androidx.annotation.MainThread
import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData

fun <T> MutableLiveData<T>.readOnly(): LiveData<T> = this

@MainThread
fun MutableLiveData<Unit>.call() {
    this.value = Unit
}

fun <T> MutableLiveData<T>.setValueOnMainThread(value: T?) {
    if (Looper.myLooper() == Looper.getMainLooper()) {
        this.value = value
    } else {
        Handler(Looper.getMainLooper()).post {
            this.value = value
        }
    }
}