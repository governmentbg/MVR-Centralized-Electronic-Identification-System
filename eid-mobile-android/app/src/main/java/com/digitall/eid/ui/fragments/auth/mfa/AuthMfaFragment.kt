package com.digitall.eid.ui.fragments.auth.mfa

import androidx.navigation.fragment.navArgs
import com.digitall.eid.databinding.FragmentAuthMfaBinding
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.extensions.setTextChangeListener
import com.digitall.eid.models.common.AlertDialogResult
import com.digitall.eid.ui.fragments.base.BaseFragment
import org.koin.androidx.viewmodel.ext.android.viewModel
import java.util.Locale
import kotlin.system.exitProcess
import kotlin.time.Duration.Companion.seconds

class AuthMfaFragment :
    BaseFragment<FragmentAuthMfaBinding, AuthMfaViewModel>() {

    companion object {
        private const val TAG = "AuthMfaFragmentTag"
    }

    override fun getViewBinding() = FragmentAuthMfaBinding.inflate(layoutInflater)

    override val viewModel: AuthMfaViewModel by viewModel()

    private val args: AuthMfaFragmentArgs by navArgs()

    override fun setupControls() {
        binding.etOtpCode.setTextChangeListener {
            viewModel.onOtpCodeChanged(it.trim())
        }
        binding.toolbar.navigationClickListener = {
            viewModel.onBackPressed()
        }
        binding.errorView.actionOneClickListener = {
            viewModel.restartLastAction()
        }
        binding.errorView.actionTwoClickListener = {
            viewModel.restartLastAction()
        }
        binding.btnAuthenticate.onClickThrottle {
            viewModel.onAuthenticateClicked()
        }
    }

    override fun onResumed() {
        viewModel.startTimer()
    }

    override fun parseArguments() {
        try {
            viewModel.setupModel(
                sessionId = args.sessionId,
                email = args.email,
                password = args.password,
                ttl = args.ttl
            )
        } catch (e: IllegalStateException) {
            logError("parseArguments Exception: ${e.message}", e, TAG)
        }
    }


    override fun subscribeToLiveData() {
        viewModel.enableAuthenticationButtonLiveData.observe(viewLifecycleOwner) {
            binding.btnAuthenticate.isEnabled = it
        }
        viewModel.countDownTimeLeftLiveData.observe(viewLifecycleOwner) { timeLeft ->
            when {
                timeLeft > 0 -> timeLeft.seconds.toComponents { _, minutes, seconds, _ ->
                    binding.tvCountDown.text =
                        String.format(Locale.getDefault(), "%02d:%02d", minutes, seconds)
                }

                else -> viewModel.onBackPressed()
            }
        }
    }

    override fun onAlertDialogResult(result: AlertDialogResult) {
        if (result.messageId == DIALOG_EXIT && result.isPositive) {
            exitProcess(0)
        }
    }
}