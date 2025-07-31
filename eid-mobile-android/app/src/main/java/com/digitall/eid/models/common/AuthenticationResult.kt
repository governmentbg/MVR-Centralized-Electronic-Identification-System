package com.digitall.eid.models.common

import com.digitall.eid.domain.models.common.ApplicationCredentials

sealed class AuthenticationResult {
    data class Success(val credentials: ApplicationCredentials): AuthenticationResult()
    data class Failure(val message: StringSource, val requiresFullReAuth: Boolean = false): AuthenticationResult()
    data object FallbackToPin: AuthenticationResult()
    data object FallbackToPassword: AuthenticationResult()
    data object Cancelled: AuthenticationResult()
}