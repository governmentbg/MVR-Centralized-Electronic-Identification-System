/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.base.biometric

import androidx.annotation.CallSuper
import com.digitall.eid.databinding.FragmentEnableBiometricBinding
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.ui.fragments.base.BaseFragment
import com.digitall.eid.utils.SupportBiometricManager
import org.koin.android.ext.android.inject

abstract class BaseEnableBiometricFragment<VM : BaseEnableBiometricViewModel> :
    BaseFragment<FragmentEnableBiometricBinding, VM>() {

    companion object {
        private const val TAG = "BaseEnableBiometricFragmentTag"
    }

    override fun getViewBinding() = FragmentEnableBiometricBinding.inflate(layoutInflater)

    private val biometricManager: SupportBiometricManager by inject()

    @CallSuper
    override fun onCreated() {
        logDebug("onCreated", TAG)
        biometricManager.setupBiometricManager(this)
    }

    @CallSuper
    override fun setupControls() {
        binding.btnYes.onClickThrottle {
            logDebug("btnEnable onClickThrottle", TAG)
            viewModel.startBiometric()
        }
        binding.btnNo.onClickThrottle {
            logDebug("btnLater onClickThrottle", TAG)
            viewModel.setupLater()
        }
    }

    @CallSuper
    override fun subscribeToLiveData() {
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

    final override fun onStopped() {
        biometricManager.cancelAuthentication()
    }

}