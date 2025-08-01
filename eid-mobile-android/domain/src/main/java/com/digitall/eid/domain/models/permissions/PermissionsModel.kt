/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.models.permissions

import android.os.Parcelable
import kotlinx.parcelize.Parcelize

@Parcelize
data class PermissionsModel (
    val id: String,
) : Parcelable