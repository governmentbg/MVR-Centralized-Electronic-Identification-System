package com.digitall.eid.ui.fragments.base.auth

import androidx.annotation.IdRes
import androidx.lifecycle.viewModelScope
import com.digitall.eid.R
import com.digitall.eid.domain.models.authentication.AuthenticationType
import com.digitall.eid.domain.models.base.ErrorType
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onLoading
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.models.common.ApplicationCredentials
import com.digitall.eid.domain.usecase.authentication.AuthenticationWithBasicProfileUseCase
import com.digitall.eid.domain.usecase.verify.login.VerifyLoginUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.models.common.BannerMessage
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.ui.BaseViewModel
import com.digitall.eid.utils.AuthenticationManager
import kotlinx.coroutines.flow.onEach
import org.koin.core.component.inject

abstract class BaseAuthViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "BaseAuthViewModelTag"
    }

    private val authenticationWithBasicProfileUseCase: AuthenticationWithBasicProfileUseCase by inject()
    private val verifyLoginUseCase: VerifyLoginUseCase by inject()

    protected val authenticationManager: AuthenticationManager by inject()

    abstract fun toMfaFragment(sessionId: String, ttl: Int, username: String, password: String)

    abstract fun navigateNext(@IdRes tabId: Int)

    protected fun login(username: String, password: String) {
        logDebug("authenticate email: $username password: $password", TAG)
        authenticationWithBasicProfileUseCase.invoke(
            email = username,
            password = password,
        ).onEach { result ->
            result.onLoading {
                logDebug("onNextClicked onLoading", TAG)
                showLoader()
            }.onSuccess { model, _, _ ->
                logDebug("onNextClicked onSuccess", TAG)
                authenticationManager.saveCredentials(
                    credentials = ApplicationCredentials(
                        username = username,
                        password = password
                    )
                )
                when (model) {
                    is AuthenticationType.Token -> verifyLogin()
                    is AuthenticationType.Mfa -> {
                        hideLoader()
                        toMfaFragment(
                            sessionId = model.sessionId ?: return@onSuccess,
                            username = username,
                            password = password,
                            ttl = model.ttl ?: return@onSuccess
                        )
                    }
                }

            }.onFailure { _, _, message, responseCode, errorType ->
                logError("onNextClicked onFailure", message, TAG)
                hideLoader()
                when (errorType) {
                    ErrorType.NO_INTERNET_CONNECTION -> showErrorState(
                        title = StringSource(R.string.error_network_not_available),
                        description = StringSource(R.string.error_network_not_available_description),
                    )

                    ErrorType.AUTHORIZATION -> showMessage(BannerMessage.error(StringSource(R.string.login_wrong_credentials_error)))

                    else -> showMessage(
                        BannerMessage.error(message?.let {
                            StringSource(
                                "$it (%s)",
                                formatArgs = listOf((responseCode ?: 0).toString())
                            )
                        } ?: run {
                            StringSource(
                                R.string.error_api_general,
                                formatArgs = listOf((responseCode ?: 0).toString())
                            )
                        }
                        )
                    )
                }
            }
        }.launchInScope(viewModelScope)
    }

    private fun verifyLogin() {
        logDebug("verifyLogin", TAG)
        verifyLoginUseCase.invoke().onEach { result ->
            result.onLoading {
                logDebug("onNextClicked onLoading", TAG)
            }.onSuccess { model, _, _ ->
                logDebug("onNextClicked onSuccess", TAG)
                val tabId = when (model.tabToOpen) {
                    0 -> R.id.nav_main_tab_home
                    else -> R.id.nav_main_tab_requests
                }

                navigateNext(tabId = tabId)
            }.onFailure { _, _, message, responseCode, errorType ->
                logError(
                    "onNextClicked onFailure", message, TAG
                )
                hideLoader()
                when (errorType) {
                    ErrorType.NO_INTERNET_CONNECTION -> showErrorState(
                        title = StringSource(R.string.error_network_not_available),
                        description = StringSource(R.string.error_network_not_available_description),
                    )

                    else -> {
                        val bannerMessage = message?.let {
                            StringSource(
                                "$it (%s)",
                                formatArgs = listOf(responseCode.toString())
                            )
                        } ?: run {
                            StringSource(
                                R.string.error_api_general,
                                formatArgs = listOf(responseCode.toString())
                            )
                        }

                        showMessage(BannerMessage.error(bannerMessage))
                    }
                }
            }
        }.launchInScope(viewModelScope)
    }

}