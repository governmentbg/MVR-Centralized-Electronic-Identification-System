/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.models.common

import android.os.Parcelable
import kotlinx.parcelize.Parcelize

@Parcelize
sealed class DownloadProgress : Parcelable {

    data class Loading(
        val message: String? = null,
    ) : DownloadProgress()

    data object Ready : DownloadProgress()

}