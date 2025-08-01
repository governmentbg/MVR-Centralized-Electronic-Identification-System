/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.models.common

import com.digitall.eid.domain.models.base.TypeEnum

enum class BiometricStatus(override val type: String): TypeEnum {
    // There the first app start and user didn't choose auth method yet
    UNSPECIFIED("unspecified"),
    // Uses when the biometric was active, but the fingerprint has not been
    // recognized too many times, so we need to change status to BIOMETRIC in next success
    // pass code entering
    BIOMETRIC_BLOCKED("biometric_blocked"),
    // User chose a fingerprint or face recognition
    BIOMETRIC("biometric"),
    // User prefer to use password
    DENIED("denied")
}