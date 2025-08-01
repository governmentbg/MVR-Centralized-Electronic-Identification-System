/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.utils

import kotlinx.coroutines.Dispatchers
import kotlin.coroutines.CoroutineContext

open class CoroutineContextProvider {

    open val main: CoroutineContext by lazy { Dispatchers.Main }

    open val io: CoroutineContext by lazy { Dispatchers.IO }

    open val default: CoroutineContext by lazy { Dispatchers.Default }

}