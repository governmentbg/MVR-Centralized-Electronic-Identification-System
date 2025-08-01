package com.digitall.eid.domain.models.signing

import android.os.Parcelable
import kotlinx.parcelize.Parcelize

@Parcelize
data class SigningCheckUserStatusRequestModel(
    val uid: String?
): Parcelable
