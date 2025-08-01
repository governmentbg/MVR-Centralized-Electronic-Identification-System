/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.models.common

import android.os.Parcelable
import kotlinx.parcelize.Parcelize

@Parcelize
sealed class ErrorState : Parcelable {

    data object Ready : ErrorState()

    data class Error(
        val title: StringSource,
        val description: StringSource,
        val iconRes: Int?,
        val showIcon: Boolean?,
        val showTitle: Boolean?,
        val showDescription: Boolean?,
        val showActionOneButton: Boolean?,
        val showActionTwoButton: Boolean?,
        val actionOneButtonText: StringSource?,
        val actionTwoButtonText: StringSource?,
    ) : ErrorState()

}