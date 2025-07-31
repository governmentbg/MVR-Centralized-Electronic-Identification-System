package com.digitall.eid.domain.models.empowerment.common

import android.os.Parcelable
import kotlinx.parcelize.Parcelize

@Parcelize
data class AuthorizedUidModel(
    val uid: String?,
    val uidType: String?,
    val name: String?,
    val issuer: Boolean?,
): Parcelable