/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.models.common

import android.os.Parcelable
import kotlinx.parcelize.Parcelize

@Parcelize
sealed class LoadingState : Parcelable {

    data class Loading(
        val message: String?,
        val translucent: Boolean
    ) : LoadingState()

    data object Ready : LoadingState()

}