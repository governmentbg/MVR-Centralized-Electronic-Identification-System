/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.error.biometric

import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.ui.fragments.base.BaseBottomSheetFragment
import org.koin.androidx.viewmodel.ext.android.viewModel
import com.digitall.eid.databinding.BottomSheetBiometricErrorBinding
import com.digitall.eid.extensions.onClickThrottle

class BiometricErrorBottomSheetFragment :
    BaseBottomSheetFragment<BottomSheetBiometricErrorBinding, BiometricErrorBottomSheetViewModel>() {

    companion object {
        private const val TAG = "PermissionBottomSheetFragmentTag"
    }

    override fun getViewBinding() = BottomSheetBiometricErrorBinding.inflate(layoutInflater)

    override val viewModel: BiometricErrorBottomSheetViewModel by viewModel()

    override fun setupControls() {
        binding.btnClose.onClickThrottle {
            logDebug("btnNext onClickThrottle", TAG)
            dismiss()
        }
    }

}