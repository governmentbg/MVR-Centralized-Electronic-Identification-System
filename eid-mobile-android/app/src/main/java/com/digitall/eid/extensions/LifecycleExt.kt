/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.extensions

import androidx.lifecycle.Lifecycle
import androidx.lifecycle.LifecycleOwner
import androidx.lifecycle.lifecycleScope
import androidx.lifecycle.repeatOnLifecycle
import com.digitall.eid.domain.utils.LogUtil.logError
import kotlinx.coroutines.CoroutineExceptionHandler
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.Job
import kotlinx.coroutines.launch

fun LifecycleOwner.launchWhenResumed(block: suspend CoroutineScope.() -> Unit) {
    this.lifecycleScope.launch {
        this@launchWhenResumed.lifecycle.repeatOnLifecycle(Lifecycle.State.RESUMED) {
            block()
        }
    }
}

fun CoroutineScope.launchWithDispatcher(
    block: suspend CoroutineScope.() -> Unit,
): Job {
    val exceptionHandler = CoroutineExceptionHandler { _, throwable ->
        logError(
            "launchWithDispatcher Exception: ${throwable.message}",
            throwable,
            "launchWithDispatcher"
        )
    }
    return launch(Dispatchers.Default + exceptionHandler) {
        block()
    }
}