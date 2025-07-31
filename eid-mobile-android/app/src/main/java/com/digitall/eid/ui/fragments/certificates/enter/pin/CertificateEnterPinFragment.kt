package com.digitall.eid.ui.fragments.certificates.enter.pin

import androidx.lifecycle.lifecycleScope
import com.digitall.eid.databinding.FragmentCertificateEnterPinBinding
import com.digitall.eid.extensions.hideKeyboard
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.ui.fragments.base.BaseFragment
import kotlinx.coroutines.flow.onEach
import org.koin.androidx.viewmodel.ext.android.viewModel

class CertificateEnterPinFragment :
    BaseFragment<FragmentCertificateEnterPinBinding, CertificateEnterPinViewModel>() {

    companion object {
        private const val TAG = "CertificateEnterPinFragmentTag"
    }

    override fun getViewBinding() = FragmentCertificateEnterPinBinding.inflate(layoutInflater)

    override val viewModel: CertificateEnterPinViewModel by viewModel()

    override fun setupControls() {
        binding.toolbar.navigationClickListener = {
            viewModel.onBackPressed()
        }
        binding.pinView.onPinEntered = { pin ->
            hideKeyboard()
            viewModel.onPinEntered(pin = pin)
        }
        binding.pinView.onPinCleared = {
            viewModel.onPinCleared()
        }
        binding.btnLogin.onClickThrottle {
            viewModel.onLoginButtonClicked()
        }
    }

    override fun subscribeToLiveData() {
        viewModel.enableLoginStateLiveData.onEach { flag ->
            binding.btnLogin.isEnabled = flag
        }.launchInScope(lifecycleScope)
    }
}