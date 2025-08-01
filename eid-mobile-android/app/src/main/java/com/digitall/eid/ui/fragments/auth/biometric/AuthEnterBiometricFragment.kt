/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.auth.biometric

import com.digitall.eid.databinding.FragmentAuthEnterBiometricBinding
import com.digitall.eid.domain.models.common.BiometricStatus
import com.digitall.eid.domain.repository.common.PreferencesRepository
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.models.common.AlertDialogResult
import com.digitall.eid.ui.fragments.base.BaseFragment
import com.digitall.eid.utils.SupportBiometricManager
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.viewModel
import javax.crypto.Cipher
import kotlin.system.exitProcess

class AuthEnterBiometricFragment :
    BaseFragment<FragmentAuthEnterBiometricBinding, AuthEnterBiometricViewModel>() {

    companion object {
        private const val TAG = "EnterBiometricFragmentTag"
    }

    override fun getViewBinding() = FragmentAuthEnterBiometricBinding.inflate(layoutInflater)

    override val viewModel: AuthEnterBiometricViewModel by viewModel()
    private val biometricManager: SupportBiometricManager by inject()
    private val preferences: PreferencesRepository by inject()

    override fun onCreated() {
        logDebug("onCreated", TAG)
        val isBiometricAvailable = biometricManager.hasBiometrics() &&
                preferences.readApplicationInfo()?.biometricStatus == BiometricStatus.BIOMETRIC
        if (isBiometricAvailable) {
            biometricManager.setupBiometricManager(this)
        } else {
            viewModel.toEnterPinFragment(true)
        }
    }

    override fun setupControls() {
        binding.tvLanguage.onClickThrottle {
            logDebug("tvLanguage onClickThrottle", TAG)
            viewModel.onLanguageCLicked()
        }
        binding.btnUsePin.onClickThrottle {
            logDebug("btnUsePin onClickThrottle", TAG)
            viewModel.toEnterPinFragment(false)
        }
        binding.icFingerprint.onClickThrottle {
            logDebug("icFingerprint onClickThrottle", TAG)
            viewModel.startBiometricAuth()
        }
    }

    override fun subscribeToLiveData() {
        viewModel.startBiometricAuthLiveData.observe(viewLifecycleOwner) {
            handleStartBiometricAuth(it)
        }
        biometricManager.onBiometricErrorLiveData.observe(viewLifecycleOwner) {
            viewModel.onBiometricError()
        }
        biometricManager.onBiometricTooManyAttemptsLiveData.observe(viewLifecycleOwner) {
            viewModel.onBiometricTooManyAttempts()
        }
        biometricManager.onBiometricSuccessLiveData.observe(viewLifecycleOwner) {
            viewModel.onBiometricSuccess(it)
        }
    }

    private fun handleStartBiometricAuth(cipher: Cipher) {
        logDebug("handleStartBiometricAuth", TAG)
        try {
            biometricManager.authenticate(cipher)
        } catch (e: Exception) {
            logError("handleStartBiometricAuth exception: ${e.message}", e, TAG)
            viewModel.onBiometricException()
        }
    }

    override fun onStopped() {
        biometricManager.cancelAuthentication()
    }

    override fun onAlertDialogResult(result: AlertDialogResult) {
        if (result.messageId == DIALOG_EXIT && result.isPositive) {
            exitProcess(0)
        }
    }

}