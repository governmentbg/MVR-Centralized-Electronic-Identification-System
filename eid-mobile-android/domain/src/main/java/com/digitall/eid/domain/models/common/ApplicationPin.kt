package com.digitall.eid.domain.models.common

import android.os.Parcelable
import kotlinx.parcelize.Parcelize

@Parcelize
data class ApplicationPin(
    val hash: String,
    val salt: String,
): Parcelable
