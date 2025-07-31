/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.utils

import com.digitall.eid.domain.utils.LogUtil.logError

class AppUncaughtExceptionHandler : Thread.UncaughtExceptionHandler {

    companion object {
        private const val TAG = "AppUncaughtExceptionHandlerTag"
    }

    override fun uncaughtException(thread: Thread, exception: Throwable) {
        logError("${exception.message}", exception, TAG)
    }

}