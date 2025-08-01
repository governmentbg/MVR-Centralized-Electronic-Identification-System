package com.digitall.eid.models.common

import android.os.Parcelable
import kotlinx.parcelize.Parcelize

@Parcelize
sealed class FullscreenLoadingState : Parcelable {

    data class Loading(
        val message: StringSource?,
    ) : FullscreenLoadingState()

    data object Ready : FullscreenLoadingState()

}