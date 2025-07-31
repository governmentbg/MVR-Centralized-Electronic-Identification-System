/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.models.common

import android.os.Parcelable
import androidx.annotation.DrawableRes
import kotlinx.parcelize.Parcelize

sealed interface ImageSource : Parcelable {

    @Parcelize
    data class Url(
        val url: String?,
        @param:DrawableRes val placeholder: Int,
        @param:DrawableRes val error: Int,
    ) : ImageSource

    @Parcelize
    data class Res(
        @param:DrawableRes val res: Int,
    ) : ImageSource

}