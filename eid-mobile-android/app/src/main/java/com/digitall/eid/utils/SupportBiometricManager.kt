/**
 * Support class to interact with biometric sensors.
 * Should be injected in fragment and setup in fragment initialization step.
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 */
package com.digitall.eid.utils

import androidx.fragment.app.Fragment
import androidx.lifecycle.LiveData
import javax.crypto.Cipher

interface SupportBiometricManager {

    val onBiometricErrorLiveData: LiveData<Unit>

    val onBiometricTooManyAttemptsLiveData: LiveData<Unit>

    val onBiometricSuccessLiveData: LiveData<Cipher?>

    fun hasBiometrics(): Boolean

    fun setupBiometricManager(fragment: Fragment)

    fun authenticate(cipher: Cipher?)

    fun cancelAuthentication()
}