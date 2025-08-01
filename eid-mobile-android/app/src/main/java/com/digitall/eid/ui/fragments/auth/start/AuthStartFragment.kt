/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.auth.start

import androidx.core.view.isVisible
import androidx.lifecycle.lifecycleScope
import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentAuthStartBinding
import com.digitall.eid.domain.ENVIRONMENT
import com.digitall.eid.domain.extensions.getEnumValue
import com.digitall.eid.domain.models.common.ApplicationCredentials
import com.digitall.eid.domain.models.common.ApplicationEnvironment
import com.digitall.eid.domain.models.common.ApplicationLanguage
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.extensions.performClickAfterDelay
import com.digitall.eid.extensions.setTextSource
import com.digitall.eid.extensions.showSpinner
import com.digitall.eid.models.auth.start.AuthStartElementsEnumUi
import com.digitall.eid.models.common.AlertDialogResult
import com.digitall.eid.models.common.ApplicationEnvironmentUi
import com.digitall.eid.models.common.AuthenticationResult
import com.digitall.eid.models.common.AuthenticationType
import com.digitall.eid.models.common.BannerMessage
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonSpinnerMenuItemUi
import com.digitall.eid.models.list.CommonSpinnerUi
import com.digitall.eid.ui.fragments.base.BaseFragment
import com.digitall.eid.ui.fragments.pin.citizen.profile.enter.EnterPinCitizenProfileBottomSheetFragment
import kotlinx.coroutines.flow.onEach
import org.koin.androidx.viewmodel.ext.android.viewModel
import kotlin.system.exitProcess

class AuthStartFragment :
    BaseFragment<FragmentAuthStartBinding, AuthStartViewModel>(),
    EnterPinCitizenProfileBottomSheetFragment.Listener {

    companion object {
        private const val TAG = "AuthStartFragmentTag"
    }

    override fun getViewBinding() = FragmentAuthStartBinding.inflate(layoutInflater)

    override val viewModel: AuthStartViewModel by viewModel()

    private var enterPinCitizenProfileBottomSheet: EnterPinCitizenProfileBottomSheetFragment? = null

    override fun onCreated() {
        viewModel.checkEnvironment()
        viewModel.logoutFromPreferences()
        viewModel.checkExistenceMobileEID()
        viewModel.checkCurrentApplicationLanguage()
        activity?.let {
            viewModel.checkInitialAuthenticationState(activity = it)
        }
    }

    override fun setupControls() {
        super.setupControls()
        binding.btnEnterPassword.onClickThrottle {
            viewModel.toAuthWithPassword()
        }
        binding.btnEnterEIDDocument.onClickThrottle {
            viewModel.toCardEnterPin()
        }
        binding.btnRegister.onClickThrottle {
            viewModel.toRegistration()
        }
        binding.btnEnterMobileEID.onClickThrottle {
            viewModel.toCertificateEnterPin()
        }
        binding.btnLanguage.onClickThrottle {
            viewModel.changeLanguage()
        }
        binding.btnEnvironment.onClickThrottle {
            binding.btnEnvironment.showSpinner(
                model = CommonSpinnerUi(
                    elementEnum = AuthStartElementsEnumUi.ENVIRONMENT_SPINNER,
                    title = AuthStartElementsEnumUi.ENVIRONMENT_SPINNER.title,
                    selectedValue = null,
                    list = ApplicationEnvironment.entries.map { environment ->
                        val environmentUi = getEnumValue<ApplicationEnvironmentUi>(environment.type)
                            ?: ApplicationEnvironmentUi.DIGITALL_DEV
                        CommonSpinnerMenuItemUi(
                            text = environmentUi.title,
                            isSelected = environment == ENVIRONMENT,
                            serverValue = environmentUi.type
                        )
                    }
                ),
                clickListener = { model ->
                    val environment = getEnumValue<ApplicationEnvironment>(
                        model.serverValue ?: return@showSpinner
                    ) ?: ApplicationEnvironment.DIGITALL_DEV
                    viewModel.setNewApplicationEnvironment(environment = environment)
                }
            )
        }
    }

    override fun subscribeToLiveData() {
        viewModel.enableLoginWithMobileEIDStateLiveData.onEach { flag ->
            binding.btnEnterMobileEID.isVisible = flag
        }.launchInScope(lifecycleScope)

        viewModel.appLanguageStateLiveData.observe(viewLifecycleOwner) { language ->
            when (language) {
                ApplicationLanguage.EN -> {
                    binding.btnLanguage.setCompoundDrawablesRelativeWithIntrinsicBounds(
                        R.drawable.ic_bg_language,
                        0,
                        0,
                        0
                    )
                    binding.btnLanguage.setTextSource(StringSource(R.string.bulgarian))
                }

                ApplicationLanguage.BG -> {
                    binding.btnLanguage.setCompoundDrawablesRelativeWithIntrinsicBounds(
                        R.drawable.ic_en_language,
                        0,
                        0,
                        0
                    )
                    binding.btnLanguage.setTextSource(StringSource(R.string.english))
                }

                null -> {}
            }
        }

        viewModel.appEnvironmentStateLiveData.observe(viewLifecycleOwner) { environment ->
            val environmentUi = getEnumValue<ApplicationEnvironmentUi>(environment.type)
                ?: ApplicationEnvironmentUi.DIGITALL_DEV
            binding.btnEnvironment.setTextSource(environmentUi.title)
        }

        viewModel.authenticationResultEvent.observe(viewLifecycleOwner) { result ->
            when (result) {
                is AuthenticationResult.FallbackToPassword -> {
                    setPasswordButton()
                    binding.btnEnterPassword.performClickAfterDelay()
                }
                is AuthenticationResult.FallbackToPin -> {
                    setPinButton()
                    binding.btnEnterPassword.performClickAfterDelay()
                }
                is AuthenticationResult.Failure -> {
                    showMessage(BannerMessage.error(message = result.message))
                    setPasswordButton()
                }

                is AuthenticationResult.Success -> viewModel.login(credentials = result.credentials)

                is AuthenticationResult.Cancelled -> {}

                else -> setPasswordButton()
            }
        }

        viewModel.authenticationTypeEvent.observe(viewLifecycleOwner) { type ->
            when (type) {
                AuthenticationType.PIN -> setPinButton()
                AuthenticationType.BIOMETRICS -> setBiometricsButton()
                else -> setPasswordButton()
            }
        }
    }

    override fun onAlertDialogResult(result: AlertDialogResult) {
        if (result.messageId == DIALOG_EXIT && result.isPositive) {
            exitProcess(0)
        }
    }

    override fun operationCompleted(credentials: ApplicationCredentials) {
        enterPinCitizenProfileBottomSheet?.dismiss().also {
            enterPinCitizenProfileBottomSheet = null
        }
        viewModel.login(credentials = credentials)
    }

    override fun operationFailed() {
        enterPinCitizenProfileBottomSheet?.dismiss().also {
            enterPinCitizenProfileBottomSheet = null
        }
        setPasswordButton()
        viewModel.clearApplicationPin()
        viewModel.disableBiometrics()
        binding.btnEnterPassword.performClickAfterDelay()
    }

    private fun setPasswordButton() {
        binding.btnEnterPassword.setTextSource(StringSource(R.string.landing_login_password))
        binding.btnEnterPassword.onClickThrottle {
            viewModel.toAuthWithPassword()
        }
    }

    private fun setPinButton() {
        binding.btnEnterPassword.setTextSource(StringSource(R.string.landing_login_pin))
        binding.btnEnterPassword.onClickThrottle {
            showEnterPinCitizenProfile()
        }
    }

    private fun setBiometricsButton() {
        binding.btnEnterPassword.setTextSource(StringSource(R.string.landing_login_biometric))
        binding.btnEnterPassword.onClickThrottle {
            viewModel.requestBiometricUnlock()
        }
    }

    private fun showEnterPinCitizenProfile() {
        enterPinCitizenProfileBottomSheet = EnterPinCitizenProfileBottomSheetFragment.newInstance(listener = this)
            .also { bottomSheet ->
                bottomSheet.show(
                    parentFragmentManager,
                    "EnterPinCitizenProfileBottomSheetFragmentTag"
                )
            }
    }
}