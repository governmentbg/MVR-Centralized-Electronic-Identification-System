/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.applications.continuecreation.pin

import androidx.lifecycle.viewModelScope
import com.digitall.eid.R
import com.digitall.eid.domain.EID_MOBILE_CERTIFICATE
import com.digitall.eid.domain.models.base.ErrorType
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onLoading
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.usecase.applications.create.ApplicationCreateConfirmWithEIDUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.extensions.readOnly
import com.digitall.eid.extensions.setValueOnMainThread
import com.digitall.eid.models.applications.create.ApplicationCreatePinModel
import com.digitall.eid.models.common.DialogMessage
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.main.MainTabsEnum
import com.digitall.eid.ui.fragments.base.pin.BaseCreatePinViewModel
import com.digitall.eid.utils.SingleLiveEvent
import kotlinx.coroutines.flow.onEach
import org.koin.core.component.inject

class ApplicationContinueCreationCreatePinViewModel : BaseCreatePinViewModel() {

    companion object {
        private const val TAG = "ApplicationContinueCreationCreatePinViewModelTag"
    }

    private val applicationCreateConfirmWithEIDUseCase: ApplicationCreateConfirmWithEIDUseCase by inject()

    private val _creationCompletedEventLiveData = SingleLiveEvent<Unit>()
    val creationCompletedEventLiveData = _creationCompletedEventLiveData.readOnly()

    private var pinModel: ApplicationCreatePinModel? = null

    override var mainTabsEnum: MainTabsEnum? = MainTabsEnum.TAB_EIM

    fun setupModel(model: ApplicationCreatePinModel?) {
        logDebug("setupModel", TAG)
        if (model == null) {
            showErrorState(
                title = StringSource(R.string.error_internal_error_short),
                description = StringSource("Required element is empty")
            )
            return
        }
        pinModel = model
    }

    fun onCreateClicked() {
        logDebug("onCreateClicked", TAG)
        applicationCreateConfirmWithEIDUseCase.invoke(
            alias = EID_MOBILE_CERTIFICATE,
            certificate = pinModel?.certificate ?: return,
            applicationId = pinModel?.applicationId ?: return,
            certificateChain = pinModel?.certificateChain ?: return,
        ).onEach { result ->
            result.onLoading {
                logDebug("applicationCreateConfirmWithEIDUseCase onLoading", TAG)
                showLoader(message = it)
            }.onSuccess { _, _, _ ->
                logDebug("applicationCreateConfirmWithEIDUseCase onSuccess", TAG)
                val applicationInfo = preferences.readApplicationInfo()
                if (applicationInfo == null) {
                    logError("applicationCreateConfirmWithEIDUseCase applicationInfo == null", TAG)
                    return@onEach
                }
                preferences.saveApplicationInfo(
                    applicationInfo.copy(
                        certificatePin = pin,
                    )
                )
                hideLoader()
                showMessage(
                    DialogMessage(
                        message = StringSource(R.string.create_application_success_confirm_message),
                        title = StringSource(R.string.information),
                        positiveButtonText = StringSource(R.string.ok),
                    )
                )
            }.onFailure { _, title, message, responseCode, errorType ->
                logError("applicationCreateConfirmWithEIDUseCase onFailure", message, TAG)
                hideLoader()
                when (errorType) {
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

    override fun onAlertDialogResult() {
        logDebug("onAlertDialogResult", TAG)
        _creationCompletedEventLiveData.setValueOnMainThread(Unit)
        popBackStackToFragmentInTab(R.id.applicationsFlowFragment)
    }

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        popBackStackToFragmentInTab(R.id.applicationsFlowFragment)
    }

}