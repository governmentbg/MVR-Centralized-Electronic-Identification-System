package com.digitall.eid.domain.models.common

import android.os.Parcelable
import kotlinx.parcelize.Parcelize

@Parcelize
data class ApplicationCredentials(
    val username: String,
    val password: String
): Parcelable
