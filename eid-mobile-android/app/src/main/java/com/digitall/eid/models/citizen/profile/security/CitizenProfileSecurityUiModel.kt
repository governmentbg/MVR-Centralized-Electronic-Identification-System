package com.digitall.eid.models.citizen.profile.security

data class CitizenProfileSecurityUiModel(
    val isTwoFactorEnabled: Boolean = false,
    val isPinEnabled: Boolean = false,
    val isBiometricEnabled: Boolean = false,
    val isBiometricAvailable: Boolean = false,
)
