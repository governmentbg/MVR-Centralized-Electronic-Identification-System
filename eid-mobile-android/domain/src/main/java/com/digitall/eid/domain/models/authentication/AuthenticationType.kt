package com.digitall.eid.domain.models.authentication

import android.os.Parcelable
import kotlinx.parcelize.Parcelize

@Parcelize
sealed class AuthenticationType : Parcelable {
    data object Token : AuthenticationType()

    data class Mfa(
        val sessionId: String?,
        val ttl: Int?
    ) : AuthenticationType()
}