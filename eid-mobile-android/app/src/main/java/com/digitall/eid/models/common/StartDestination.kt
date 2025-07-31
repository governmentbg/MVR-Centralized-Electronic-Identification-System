/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.models.common

import android.os.Bundle
import android.os.Parcelable
import androidx.annotation.IdRes
import kotlinx.parcelize.Parcelize

@Parcelize
data class StartDestination(
    @param:IdRes val destination: Int,

    /**
     * Use this variable to pass arguments to the start destination of
     * the [destination] graph.
     */
    val arguments: Bundle? = null
) : Parcelable