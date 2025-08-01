/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.models.network.authentication.response

import com.google.gson.annotations.SerializedName

data class AuthenticationResponse(
    val data: Any?
)

data class TokenResponse(
    @SerializedName("access_token") val accessToken: String?,
    @SerializedName("expires_in") val expiresIn: Int?,
    @SerializedName("refresh_expires_in") val refreshExpiresIn: Int?,
    @SerializedName("refresh_token") val refreshToken: String?,
    @SerializedName("token_type") val tokenType: String?,
    @SerializedName("not-before-policy") val notBeforePolicy: Int?,
    @SerializedName("session_state") val sessionState: String?,
    @SerializedName("scope") val scope: String?,
)

data class MFAResponse(
    @SerializedName("isceiUi2FaUrl") val mfaUrl: String?,
    @SerializedName("sessionId") val sessionId: String?,
    @SerializedName("ttl") val ttl: Int?
)

