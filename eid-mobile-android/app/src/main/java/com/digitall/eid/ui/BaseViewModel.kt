/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 */
package com.digitall.eid.ui

import android.os.Bundle
import android.os.Looper
import androidx.annotation.CallSuper
import androidx.annotation.DrawableRes
import androidx.annotation.IdRes
import androidx.annotation.VisibleForTesting
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import androidx.navigation.NavController
import androidx.navigation.NavDirections
import com.digitall.eid.NavActivityDirections
import com.digitall.eid.R
import com.digitall.eid.domain.APPLICATION_LANGUAGE
import com.digitall.eid.domain.models.base.ErrorType
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onLoading
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.models.common.ApplicationLanguage
import com.digitall.eid.domain.models.common.BiometricStatus
import com.digitall.eid.domain.repository.common.PreferencesRepository
import com.digitall.eid.domain.usecase.logout.LogoutUseCase
import com.digitall.eid.domain.utils.CryptographyHelper
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.extensions.launchWithDispatcher
import com.digitall.eid.extensions.navigateNewRoot
import com.digitall.eid.extensions.navigateTo
import com.digitall.eid.extensions.popBackStackToFragment
import com.digitall.eid.extensions.readOnly
import com.digitall.eid.extensions.setValueOnMainThread
import com.digitall.eid.models.common.AlertDialogResult
import com.digitall.eid.models.common.BannerMessage
import com.digitall.eid.models.common.DialogMessage
import com.digitall.eid.models.common.ErrorState
import com.digitall.eid.models.common.FullscreenLoadingState
import com.digitall.eid.models.common.LoadingState
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.common.UiState
import com.digitall.eid.models.list.CommonDatePickerUi
import com.digitall.eid.models.list.CommonDialogWithSearchMultiselectUi
import com.digitall.eid.models.list.CommonDialogWithSearchUi
import com.digitall.eid.models.list.CommonSpinnerUi
import com.digitall.eid.models.main.MainTabsEnum
import com.digitall.eid.utils.AppEventsHelper
import com.digitall.eid.utils.AppFirebaseMessagingServiceHelper
import com.digitall.eid.utils.InactivityTimer
import com.digitall.eid.utils.LocalizationManager
import com.digitall.eid.utils.SingleLiveEvent
import com.google.firebase.messaging.RemoteMessage
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.onEach
import kotlinx.coroutines.launch
import org.koin.core.component.KoinComponent
import org.koin.core.component.inject
import java.lang.ref.WeakReference
import javax.crypto.Cipher

abstract class BaseViewModel : ViewModel(), KoinComponent {

    companion object {
        private const val TAG = "BaseViewModelTag"
    }

    protected open val isAuthorizationActive: Boolean = true

    protected val inactivityTimer: InactivityTimer by inject()
    protected val preferences: PreferencesRepository by inject()

    private val appFirebaseMessagingServiceHelper: AppFirebaseMessagingServiceHelper by inject()
    private val appEventsHelper: AppEventsHelper by inject()

    private val logoutUseCase: LogoutUseCase by inject()
    private val cryptographyHelper: CryptographyHelper by inject()
    private val localizationManager: LocalizationManager by inject()

    private var flowNavControllerRef: WeakReference<NavController>? = null
    private var activityNavControllerRef: WeakReference<NavController>? = null
    private var tabNavControllerRef: WeakReference<NavController>? = null

    private var isAlreadyInitialized = false

    private val _closeActivityLiveData = SingleLiveEvent<Unit>()
    val closeActivityLiveData = _closeActivityLiveData.readOnly()

    private val _backPressedFailedLiveData = SingleLiveEvent<Unit>()
    val backPressedFailedLiveData = _backPressedFailedLiveData.readOnly()

    private val _showBannerMessageLiveData = SingleLiveEvent<BannerMessage>()
    val showBannerMessageLiveData = _showBannerMessageLiveData.readOnly()

    private val _showDialogMessageLiveData = SingleLiveEvent<DialogMessage>()
    val showDialogMessageLiveData = _showDialogMessageLiveData.readOnly()

    private val _showLoadingDialogLiveData = SingleLiveEvent<FullscreenLoadingState>()
    val showLoadingDialogLiveData = _showLoadingDialogLiveData.readOnly()

    private val _uiState = MutableLiveData<UiState>()
    val uiState = _uiState.readOnly()

    private val _loadingState = MutableLiveData<LoadingState>()
    val loadingState = _loadingState.readOnly()

    private val _errorState = MutableLiveData<ErrorState>()
    val errorState = _errorState.readOnly()

    private val _appLanguageStateLiveData = SingleLiveEvent<ApplicationLanguage?>()
    val appLanguageStateLiveData = _appLanguageStateLiveData.readOnly()

    val newFirebaseMessageLiveData = appFirebaseMessagingServiceHelper.newFirebaseMessageLiveData

    val newTokenEventLiveData = appFirebaseMessagingServiceHelper.newTokenEventLiveData

    open var mainTabsEnum: MainTabsEnum? = null

    abstract fun onBackPressed()

    private fun isMainThread(): Boolean {
        return Thread.currentThread() == Looper.getMainLooper().thread
    }

    fun showLoader(
        message: String? = null,
        translucent: Boolean = false,
    ) {
        _loadingState.setValueOnMainThread(
            LoadingState.Loading(
                message = message,
                translucent = translucent,
            )
        )
    }

    fun hideLoader(delay: Long? = null) {
        if (delay == null || delay == 0L) {
            _loadingState.setValueOnMainThread(LoadingState.Ready)
        } else {
            viewModelScope.launchWithDispatcher {
                delay(delay)
                _loadingState.setValueOnMainThread(LoadingState.Ready)
            }
        }
    }

    fun showFullscreenLoader(message: StringSource?) {
        _showLoadingDialogLiveData.setValueOnMainThread(
            FullscreenLoadingState.Loading(
                message = message,
            )
        )
    }

    fun hideFullscreenLoader(delay: Long? = null) {
        if (delay == null || delay == 0L) {
            _showLoadingDialogLiveData.setValueOnMainThread(FullscreenLoadingState.Ready)
        } else {
            viewModelScope.launchWithDispatcher {
                delay(delay)
                _showLoadingDialogLiveData.setValueOnMainThread(FullscreenLoadingState.Ready)
            }
        }
    }

    fun showEmptyState() {
        logDebug("showEmptyState", TAG)
        _uiState.setValueOnMainThread(UiState.Empty)
    }

    fun showReadyState() {
        logDebug("showReadyState", TAG)
        _uiState.setValueOnMainThread(UiState.Ready)
    }

    fun showErrorState(
        title: StringSource,
        iconRes: Int? = null,
        description: StringSource,
        showIcon: Boolean? = null,
        showTitle: Boolean? = null,
        showDescription: Boolean? = null,
        showActionOneButton: Boolean? = null,
        showActionTwoButton: Boolean? = null,
        actionOneButtonText: StringSource? = null,
        actionTwoButtonText: StringSource? = null,
    ) {
        logDebug("showErrorState", TAG)
        _errorState.setValueOnMainThread(
            ErrorState.Error(
                title = title,
                iconRes = iconRes,
                showIcon = showIcon,
                showTitle = showTitle,
                description = description,
                showDescription = showDescription,
                showActionOneButton = showActionOneButton,
                showActionTwoButton = showActionTwoButton,
                actionOneButtonText = actionOneButtonText,
                actionTwoButtonText = actionTwoButtonText,
            )
        )
    }

    fun hideErrorState() {
        logDebug("hideErrorState", TAG)
        _errorState.setValueOnMainThread(ErrorState.Ready)
    }

    // Should be called only by Base
    fun onViewCreated() {
        logDebug("attachView isAlreadyInitialized: $isAlreadyInitialized", TAG)
        _loadingState.value = LoadingState.Ready
        _errorState.value = ErrorState.Ready
        onCreated()
        inactivityTimer.setTimerCoroutineScope(viewModelScope)
        if (isAlreadyInitialized) return
        isAlreadyInitialized = true
        localizationManager.tryApplyInitialApplicationLanguage()
        onFirstAttach()
    }

    /**
     * You should override this instead of init.
     * This method will be called after [BaseFragment::initViews] and
     * [BaseFlowFragment::onViewCreated] and also after [BaseActivity::onCreate]
     */
    protected open fun onFirstAttach() {
        // Override when needed
    }

    open fun onCreated() {
        // Override when needed
    }

    open fun onResumed() {
        // Override when needed
    }

    open fun onPaused() {
        // Override when needed
    }

    open fun onStopped() {
        // Override when needed
    }

    open fun onDestroyed() {
        // Override when needed
    }

    open fun onDetached() {
        // Override when needed, viewModelScope will not work
    }

    open fun onAlertDialogResult() {
        // Override when needed
    }

    open fun onAlertDialogResult(result: AlertDialogResult) {
        // Override when needed
    }

    open fun onHiddenChanged(hidden: Boolean) {
        // Override when needed
    }

    open fun citizenAssociateEID() {
        // Override when needed
    }


    fun bindFlowNavController(navController: NavController) {
        flowNavControllerRef = WeakReference(navController)
    }

    fun unbindFlowNavController() {
        flowNavControllerRef?.clear()
        flowNavControllerRef = null
    }

    fun bindTabNavController(navController: NavController) {
        logDebug("bindTabNavController", TAG)
        tabNavControllerRef = WeakReference(navController)
    }

    fun unbindTabNavController() {
        logDebug("unbindTabNavController", TAG)
        tabNavControllerRef?.clear()
        tabNavControllerRef = null
    }

    fun bindActivityNavController(navController: NavController) {
        activityNavControllerRef = WeakReference(navController)
    }

    fun unbindActivityNavController() {
        activityNavControllerRef?.clear()
        activityNavControllerRef = null
    }

    protected fun findFlowNavController(): NavController {
        return flowNavControllerRef?.get()
            ?: throw IllegalArgumentException("Flow Navigation controller is not set!")
    }

    protected fun findActivityNavController(): NavController {
        return activityNavControllerRef?.get()
            ?: throw IllegalArgumentException("Activity Navigation controller is not set!")
    }

    protected fun findTabNavController(): NavController {
        return tabNavControllerRef?.get()
            ?: throw IllegalArgumentException("Tab Navigation controller is not set!")
    }

    @CallSuper
    open fun consumeException(exception: Throwable, isSilent: Boolean = false) {
        logDebug(exception.toString(), TAG)
    }

    protected fun showMessage(message: BannerMessage) {
        logDebug("showMessage BannerMessage", TAG)
        _showBannerMessageLiveData.setValueOnMainThread(message)
    }

    protected fun showBannerMessage(
        messageId: String? = null,
        title: StringSource? = null,
        message: StringSource,
        @DrawableRes icon: Int? = R.drawable.ic_error,
        state: BannerMessage.State = BannerMessage.State.ERROR,
        gravity: BannerMessage.Gravity = BannerMessage.Gravity.START,
    ) {
        logDebug("showBannerMessage", TAG)
        _showBannerMessageLiveData.setValueOnMainThread(
            BannerMessage(
                icon = icon,
                title = title,
                state = state,
                message = message,
                gravity = gravity,
                messageID = messageId,
            )
        )
    }

    protected fun showMessage(message: DialogMessage) {
        logDebug("showMessage DialogMessage", TAG)
        _showDialogMessageLiveData.setValueOnMainThread(message)
    }

    protected fun showDialogMessage(
        messageId: String? = null,
        title: StringSource? = null,
        message: StringSource,
        state: DialogMessage.State = DialogMessage.State.ERROR,
        gravity: DialogMessage.Gravity = DialogMessage.Gravity.START,
        positiveButtonText: StringSource? = null,
        negativeButtonText: StringSource? = null,
        @DrawableRes icon: Int? = R.drawable.ic_error,
    ) {
        logDebug("showDialogMessage", TAG)
        _showDialogMessageLiveData.setValueOnMainThread(
            DialogMessage(
                icon = icon,
                title = title,
                state = state,
                message = message,
                gravity = gravity,
                messageID = messageId,
                positiveButtonText = positiveButtonText,
                negativeButtonText = negativeButtonText,
            )
        )
    }

    protected fun popBackStack() {
        try {
            if (isMainThread()) {
                if (!findFlowNavController().popBackStack()) {
                    _backPressedFailedLiveData.call()
                }
            } else {
                viewModelScope.launch(Dispatchers.Main) {
                    try {
                        if (!findFlowNavController().popBackStack()) {
                            _backPressedFailedLiveData.call()
                        }
                    } catch (e: Exception) {
                        logError("bobBackStack Exception: ${e.message}", e, "NavController")
                    }
                }
            }
        } catch (e: Exception) {
            logError("bobBackStack Exception: ${e.message}", e, "NavController")
        }
    }

    protected fun navigateUp() {
        try {
            if (isMainThread()) {
                if (!findFlowNavController().navigateUp()) {
                    _backPressedFailedLiveData.call()
                }
            } else {
                viewModelScope.launch(Dispatchers.Main) {
                    try {
                        if (!findFlowNavController().navigateUp()) {
                            _backPressedFailedLiveData.call()
                        }
                    } catch (e: Exception) {
                        logError("bobBackStack Exception: ${e.message}", e, "NavController")
                    }
                }
            }
        } catch (e: Exception) {
            logError("bobBackStack Exception: ${e.message}", e, "NavController")
        }
    }

    // activity

    protected fun navigateInActivity(directions: NavDirections) {
        findActivityNavController().navigateTo(
            directions = directions,
            viewModelScope = viewModelScope,
        )
    }

    protected fun navigateInActivity(@IdRes fragment: Int) {
        findActivityNavController().navigateTo(
            fragment = fragment,
            viewModelScope = viewModelScope,
        )
    }

    fun navigateInActivity(
        @IdRes fragment: Int,
        bundle: Bundle,
    ) {
        findActivityNavController().navigateTo(
            fragment = fragment,
            bundle = bundle,
            viewModelScope = viewModelScope,
        )
    }

    // activity

    protected fun navigateInTab(directions: NavDirections) {
        findTabNavController().navigateTo(
            directions = directions,
            viewModelScope = viewModelScope,
        )
    }

    protected fun navigateInTab(@IdRes fragment: Int) {
        findTabNavController().navigateTo(
            fragment = fragment,
            viewModelScope = viewModelScope,
        )
    }

    // activity

    protected fun navigateInFlow(directions: NavDirections) {
        findFlowNavController().navigateTo(
            directions = directions,
            viewModelScope = viewModelScope,
        )
    }

    protected fun navigateInFlow(@IdRes fragment: Int) {
        findFlowNavController().navigateTo(
            fragment = fragment,
            viewModelScope = viewModelScope,
        )
    }

    fun navigateInFlow(
        @IdRes fragment: Int,
        bundle: Bundle,
    ) {
        findFlowNavController().navigateTo(
            fragment = fragment,
            bundle = bundle,
            viewModelScope = viewModelScope,
        )
    }

    protected fun navigateNewRootInActivity(directions: NavDirections) {
        findActivityNavController().navigateNewRoot(
            directions = directions,
            viewModelScope = viewModelScope,
        )
    }

    protected fun navigateNewRootInFlow(directions: NavDirections) {
        findFlowNavController().navigateNewRoot(
            directions = directions,
            viewModelScope = viewModelScope,
        )
    }

    protected fun popBackStackToFragment(@IdRes fragment: Int) {
        logDebug("popBackStackToFragment", TAG)
        findFlowNavController().popBackStackToFragment(
            fragment = fragment,
            viewModelScope = viewModelScope,
        )
    }

    protected fun popBackStackToFragmentInTab(@IdRes fragment: Int) {
        logDebug("popBackStackToFragment", TAG)
        findTabNavController().popBackStackToFragment(
            fragment = fragment,
            viewModelScope = viewModelScope,
        )
    }

    protected fun backToTab() {
        logDebug("popBackStackToTab", TAG)
        mainTabsEnum?.let { mainTabsEnum ->
            findTabNavController().popBackStackToFragment(
                fragment = mainTabsEnum.fragmentID,
                viewModelScope = viewModelScope,
            )
        } ?: run {
            logError("popBackStackToTab mainTabsEnum not found", TAG)
            showMessage(BannerMessage.error(StringSource("Tab not found, internal error")))
        }
    }

    protected fun navigateNewRootInActivity(
        @IdRes fragment: Int,
        bundle: Bundle? = null
    ) {
        findActivityNavController().navigateNewRoot(
            fragment = fragment,
            bundle = bundle,
            viewModelScope = viewModelScope,
        )
    }

    protected fun navigateNewRootInFlow(
        @IdRes fragment: Int,
        bundle: Bundle? = null
    ) {
        findFlowNavController().navigateNewRoot(
            fragment = fragment,
            bundle = bundle,
            viewModelScope = viewModelScope,
        )
    }

    protected fun closeActivity() {
        _closeActivityLiveData.callOnMainThread()
    }

    @CallSuper
    @VisibleForTesting(otherwise = VisibleForTesting.PROTECTED)
    public override fun onCleared() {
        logDebug("onCleared", TAG)
        unbindFlowNavController()
        unbindActivityNavController()
        unbindTabNavController()
//        disposables.clear()
        super.onCleared()
    }

    fun logout() {
        showMessage(BannerMessage.error(StringSource("User not setup, please go through the registration process")))
        logoutUseCase.invoke().onEach { result ->
            result.onLoading {
                logDebug("onLogoutClicked onLoading", TAG)
                showLoader()
            }.onSuccess { _, _, _ ->
                logDebug("onLogoutClicked onSuccess", TAG)
                hideLoader()
                hideErrorState()
                toLoginFragment()
            }.onFailure { _, title, message, responseCode, errorType ->
                logError("onLogoutClicked onFailure", message, TAG)
                hideLoader()
                when (errorType) {
                    ErrorType.NO_INTERNET_CONNECTION -> showErrorState(
                        title = StringSource(R.string.error_network_not_available),
                        description = StringSource(R.string.error_network_not_available_description),
                    )

                    ErrorType.AUTHORIZATION -> toLoginFragment()

                    else -> showErrorState(
                        title = StringSource(R.string.information),
                        description = message?.let {
                            StringSource(
                                "$it (%s)",
                                formatArgs = listOf((responseCode ?: 0).toString())
                            )
                        } ?: StringSource(
                            R.string.error_api_general,
                            formatArgs = listOf((responseCode ?: 0).toString())
                        ),
                    )
                }
            }
        }.launchInScope(viewModelScope)
    }

    fun enableBiometric(encryptedPin: String) {
        logDebug("setupNow encryptedPin: $encryptedPin", TAG)
        val pinCode = preferences.readApplicationInfo()
        if (pinCode == null) {
            logout()
            return
        }
        preferences.saveApplicationInfo(
            pinCode.copy(
                applicationPin = encryptedPin,
                biometricStatus = BiometricStatus.BIOMETRIC,
            )
        )
    }

    fun disableBiometric() {
        logDebug("disableBiometric", TAG)
        val pinCode = preferences.readApplicationInfo()
        if (pinCode == null) {
            logout()
            return
        }
        preferences.saveApplicationInfo(
            pinCode.copy(
                applicationPin = null,
                biometricStatus = BiometricStatus.DENIED,
            )
        )
    }

    protected fun getBiometricCipherForDecryption(): Cipher? {
        logDebug("getBiometricCipherForDecryption", TAG)
        val pinCode = preferences.readApplicationInfo()
        if (pinCode == null) {
            logError("getBiometricCipherForDecryption pinCode == null", TAG)
            showMessage(BannerMessage.error(StringSource(R.string.error_pin_code_not_setup)))
            return null
        }
        return try {
            val vector =
                cryptographyHelper.getInitializationVectorFromString(pinCode.applicationPin!!)
            cryptographyHelper.getBiometricCipherForDecryption(vector)
        } catch (e: Exception) {
            logError("getBiometricCipherForDecryption Exception: ${e.message}", e, TAG)
            null
        }
    }

    fun showDialogWithSearch(model: CommonDialogWithSearchUi) {
        logDebug("onDialogClicked", TAG)
        findActivityNavController().navigateTo(
            directions = NavActivityDirections.toCommonBottomSheetWithSearchFragment(
                model = model,
            ),
            viewModelScope = viewModelScope,
        )
    }

    fun showDialogWithSearchMultiselect(model: CommonDialogWithSearchMultiselectUi) {
        findActivityNavController().navigateTo(
            directions = NavActivityDirections.toCommonBottomSheetWithSearchMultiselectFragment(
                model = model,
            ),
            viewModelScope = viewModelScope,
        )
    }

    open fun onSpinnerSelected(model: CommonSpinnerUi) {
        // Override when needed
    }

    open fun onDatePickerChanged(
        model: CommonDatePickerUi,
    ) {
        // Override when needed
    }

    open fun onDialogElementSelected(model: CommonDialogWithSearchUi) {
        // Override when needed
    }

    open fun onDialogMultiselectSelected(model: CommonDialogWithSearchMultiselectUi) {
        // Override when needed
    }

    fun fragmentOnPause() {
        logDebug("fragmentOnPause", TAG)

    }

    fun fragmentOnResume() {
        logDebug("fragmentOnResume isAuthorizationActive: $isAuthorizationActive", TAG)
        inactivityTimer.fragmentOnResume(isAuthorizationActive)
    }

    fun toLoginFragment() {
        logDebug("onActivityTimerExpired", TAG)
        hideFullscreenLoader()
        navigateNewRootInActivity(
            NavActivityDirections.toAuthFlowFragment()
        )
    }

    fun checkCurrentApplicationLanguage() {
        val applicationLanguage = APPLICATION_LANGUAGE
        _appLanguageStateLiveData.setValueOnMainThread(applicationLanguage)
    }

    open fun changeLanguage() {
        localizationManager.changeLanguage()
    }

    fun onNewFirebaseMessage(message: RemoteMessage) {
        viewModelScope.launch(Dispatchers.IO) {
            appEventsHelper.onNewFirebaseMessage(message)
        }
    }

    fun checkIfUserHasAssociatedEID(): Boolean = preferences.readApplicationInfo()?.userModel?.eidEntityId != null

}