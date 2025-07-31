/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.models.empowerment.common

import android.os.Parcelable
import kotlinx.parcelize.Parcelize

@Parcelize
data class EmpowermentUidModel(
    val uid: String?,
    val uidType: String?,
    val name: String?,
): Parcelable
