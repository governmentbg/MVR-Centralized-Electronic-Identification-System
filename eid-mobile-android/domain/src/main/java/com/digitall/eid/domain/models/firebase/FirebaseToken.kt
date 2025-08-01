package com.digitall.eid.domain.models.firebase

import android.os.Parcelable
import kotlinx.parcelize.Parcelize

@Parcelize
data class FirebaseToken(val token: String): Parcelable
