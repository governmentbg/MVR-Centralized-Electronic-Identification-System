/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.auth.start

import androidx.annotation.IdRes
import androidx.fragment.app.FragmentActivity
import androidx.lifecycle.viewModelScope
import com.digitall.eid.NavActivityDirections
import com.digitall.eid.R
import com.digitall.eid.data.di.apiModule
import com.digitall.eid.data.di.networkRepositoryModule
import com.digitall.eid.data.di.retrofitModule
import com.digitall.eid.domain.APPLICATION_LANGUAGE
import com.digitall.eid.domain.DELAY_500
import com.digitall.eid.domain.EID_MOBILE_CERTIFICATE
import com.digitall.eid.domain.EID_MOBILE_CERTIFICATE_KEYS
import com.digitall.eid.domain.ENVIRONMENT
import com.digitall.eid.domain.LOCALIZATIONS
import com.digitall.eid.domain.di.useCaseModule
import com.digitall.eid.domain.extensions.readOnly
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onLoading
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.models.common.ApplicationCredentials
import com.digitall.eid.domain.models.common.ApplicationEnvironment
import com.digitall.eid.domain.usecase.assets.localization.GetAssetsLocalizationsUseCase
import com.digitall.eid.domain.utils.CryptographyHelper
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.extensions.launchWithDispatcher
import com.digitall.eid.extensions.readOnly
import com.digitall.eid.extensions.setValueOnMainThread
import com.digitall.eid.models.common.DialogMessage
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.ui.fragments.base.BaseFragment.Companion.DIALOG_EXIT
import com.digitall.eid.ui.fragments.base.auth.BaseAuthViewModel
import com.digitall.eid.utils.SingleLiveEvent
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.onEach
import kotlinx.coroutines.launch
import org.koin.core.component.inject
import org.koin.core.context.loadKoinModules
import org.koin.core.context.unloadKoinModules

class AuthStartViewModel : BaseAuthViewModel() {

    companion object {
        private const val TAG = "AuthStartViewModelTag"
        private val NETWORK_MODULES =
            listOf(apiModule, retrofitModule, networkRepositoryModule, useCaseModule)
    }

    override val isAuthorizationActive: Boolean = false

    private val cryptographyHelper: CryptographyHelper by inject()
    private val getAssetsLocalizationsUseCase: GetAssetsLocalizationsUseCase
        get() = getKoin().get()

    private val _enableLoginWithMobileEIDStateLiveData = MutableStateFlow(true)
    val enableLoginWithMobileEIDStateLiveData = _enableLoginWithMobileEIDStateLiveData.readOnly()

    private val _appEnvironmentStateLiveData = SingleLiveEvent<ApplicationEnvironment>()
    val appEnvironmentStateLiveData = _appEnvironmentStateLiveData.readOnly()

    val authenticationResultEvent = authenticationManager.authenticationResultEvent
    val authenticationTypeEvent = authenticationManager.authenticationTypeEvent

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        showMessage(
            DialogMessage(
                messageID = DIALOG_EXIT,
                message = StringSource(R.string.exit_application_message),
                title = StringSource(R.string.information),
                positiveButtonText = StringSource(R.string.yes),
                negativeButtonText = StringSource(R.string.no),
            )
        )
    }

    override fun onFirstAttach() {
        initialApplicationEnvironment()
        fetchAssets()
    }

    override fun changeLanguage() {
        super.changeLanguage()
        fetchAssets()
    }

    fun toAuthWithPassword() =
        navigateInFlow(AuthStartFragmentDirections.toAuthEnterEmailPasswordFragment())

    fun toCardEnterPin() = navigateInFlow(AuthStartFragmentDirections.toCardEnterPinFlowFragment())

    fun toRegistration() = navigateInFlow(AuthStartFragmentDirections.toRegistrationFlowFragment())

    fun toCertificateEnterPin() =
        navigateInFlow(AuthStartFragmentDirections.toCertificateEnterPinFlowFragment())

    fun logoutFromPreferences() = preferences.logoutFromPreferences()

    fun checkExistenceMobileEID() {
        val hasKeys = cryptographyHelper.hasAlias(alias = EID_MOBILE_CERTIFICATE_KEYS)
        val hasCertificate = cryptographyHelper.hasAlias(alias = EID_MOBILE_CERTIFICATE)
        val hasCertificatePin = preferences.readApplicationInfo()?.certificatePin.isNullOrEmpty()

        if (hasKeys.not() || hasCertificate.not() || hasCertificatePin.not()) {
            cryptographyHelper.deleteCertificateWithChainFromKeyStore(alias = EID_MOBILE_CERTIFICATE)
            cryptographyHelper.deletePrivateKey(alias = EID_MOBILE_CERTIFICATE_KEYS)

            viewModelScope.launchWithDispatcher {
                _enableLoginWithMobileEIDStateLiveData.emit(false)
            }
        }
    }

    fun checkInitialAuthenticationState(activity: FragmentActivity) {
        authenticationManager.initialize(activity = activity)
        authenticationManager.getInitialAuthenticationState()
    }

    private fun initialApplicationEnvironment() {
        checkEnvironment()
        loadKoinModules(NETWORK_MODULES)
    }

    fun setNewApplicationEnvironment(environment: ApplicationEnvironment) {
        unloadKoinModules(NETWORK_MODULES)
        ENVIRONMENT = environment
        preferences.saveEnvironment(environment = environment)
        loadKoinModules(NETWORK_MODULES)
        fetchAssets()
        _appEnvironmentStateLiveData.setValueOnMainThread(ENVIRONMENT)
    }

    fun checkEnvironment() {
        ENVIRONMENT = preferences.readEnvironment() ?: ApplicationEnvironment.DIGITALL_DEV
        _appEnvironmentStateLiveData.setValueOnMainThread(ENVIRONMENT)
    }

    fun login(credentials: ApplicationCredentials) =
        login(username = credentials.username, password = credentials.password)

    fun clearApplicationPin() = authenticationManager.clearApplicationPin()

    fun disableBiometrics() = authenticationManager.setBiometricsEnabledForUnlock(false)

    fun requestBiometricUnlock() = authenticationManager.requestBiometricUnlock()

    private fun fetchAssets() {
        val language = APPLICATION_LANGUAGE.type
        getAssetsLocalizationsUseCase.invoke(language = language).onEach { result ->
            result.onLoading {
                showLoader()
            }.onSuccess { model, _, _ ->
                LOCALIZATIONS = model
                delay(DELAY_500)
                hideLoader()
            }.onFailure { _, _, _, _, _ ->
                delay(DELAY_500)
                hideLoader()
            }
        }.launchInScope(viewModelScope)
    }

    override fun navigateNext(@IdRes tabId: Int) {
        logDebug("navigateNext", TAG)
        navigateNewRootInActivity(
            NavActivityDirections.toMainTabsFlowFragment(tabId = tabId)
        )
        viewModelScope.launch {
            delay(DELAY_500)
            hideLoader()
        }
    }

    override fun toMfaFragment(sessionId: String, ttl: Int, username: String, password: String) {
        logDebug("toMfaFragment", TAG)
        navigateInFlow(
            AuthStartFragmentDirections.toMfaFragment(
                sessionId = sessionId,
                email = username,
                password = password,
                ttl = ttl
            )
        )
    }
}