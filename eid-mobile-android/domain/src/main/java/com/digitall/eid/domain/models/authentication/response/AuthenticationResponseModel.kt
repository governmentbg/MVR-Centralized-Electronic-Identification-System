/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.models.authentication.response

import android.os.Parcelable
import kotlinx.parcelize.Parcelize
import kotlinx.parcelize.RawValue

@Parcelize
data class AuthenticationResponseModel(
    val data: @RawValue Any?,
) : Parcelable

@Parcelize
data class TokenResponseModel(
    val scope: String?,
    val expiresIn: Int?,
    val tokenType: String?,
    val accessToken: String?,
    val refreshToken: String?,
    val sessionState: String?,
    val notBeforePolicy: Int?,
    val refreshExpiresIn: Int?,
) : Parcelable

@Parcelize
data class MFAResponseModel(
    val mfaUrl: String?,
    val sessionId: String?,
    val ttl: Int?
) : Parcelable
