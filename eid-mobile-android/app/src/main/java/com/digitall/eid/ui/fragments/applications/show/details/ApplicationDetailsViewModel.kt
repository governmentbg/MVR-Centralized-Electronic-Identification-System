/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.applications.show.details

import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.viewModelScope
import com.digitall.eid.R
import com.digitall.eid.domain.APPLICATION_LANGUAGE
import com.digitall.eid.domain.extensions.getEnumValue
import com.digitall.eid.domain.extensions.readOnly
import com.digitall.eid.domain.models.applications.all.ApplicationCompletionStatusEnum
import com.digitall.eid.domain.models.applications.all.ApplicationFullDetailsModel
import com.digitall.eid.domain.models.base.ErrorType
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onLoading
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.usecase.applications.all.CompleteApplicationUseCase
import com.digitall.eid.domain.usecase.applications.all.GetApplicationDetailsUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.isFragmentInBackStack
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.extensions.readOnly
import com.digitall.eid.extensions.setValueOnMainThread
import com.digitall.eid.mappers.applications.show.details.ApplicationDetailsUiMapper
import com.digitall.eid.models.applications.details.ApplicationDetailsAdapterMarker
import com.digitall.eid.models.applications.details.ApplicationDetailsElementsEnumUi
import com.digitall.eid.models.applications.details.ApplicationDetailsTypeEnum
import com.digitall.eid.models.common.AlertDialogResult
import com.digitall.eid.models.common.DialogMessage
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonSimpleTextUi
import com.digitall.eid.models.main.MainTabsEnum
import com.digitall.eid.ui.BaseViewModel
import com.digitall.eid.ui.fragments.main.tabs.eim.MainTabEIMFragmentDirections
import com.digitall.eid.utils.SingleLiveEvent
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.onEach
import org.koin.core.component.inject

class ApplicationDetailsViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "ApplicationDetailsViewModelTag"
    }

    private val getApplicationDetailsUseCase: GetApplicationDetailsUseCase by inject()
    private val applicationDetailsUiMapper: ApplicationDetailsUiMapper by inject()
    private val completeApplicationUseCase: CompleteApplicationUseCase by inject()

    override var mainTabsEnum: MainTabsEnum? = MainTabsEnum.TAB_EIM

    private val _adapterListLiveData =
        MutableStateFlow<List<ApplicationDetailsAdapterMarker>>(emptyList())
    val adapterListLiveData = _adapterListLiveData.readOnly()

    private val _openPaymentEvent = SingleLiveEvent<String?>()
    val openPaymentEvent = _openPaymentEvent.readOnly()

    private val _certificateStatusChangeLiveData = MutableLiveData(false)
    val certificateStatusChangeLiveData = _certificateStatusChangeLiveData.readOnly()

    private var applicationId: String? = null
    private var certificateId: String? = null
    private var applicationDetails: ApplicationFullDetailsModel? = null

    private var isCertificateStatusChanged = false
        set(value) {
            _certificateStatusChangeLiveData.setValueOnMainThread(value)
            field = value
        }

    override fun onAlertDialogResult(result: AlertDialogResult) {
        if (result.isPositive && isCertificateStatusChanged) {
            onBackPressed()
        }
    }

    fun setupModel(applicationId: String?, certificateId: String?) {
        logDebug("setupModel applicationId: $applicationId", TAG)
        if (applicationId.isNullOrEmpty()) {
            showErrorState(
                title = StringSource(R.string.error_internal_error_short),
                description = StringSource("Required element is empty")
            )
            return
        }
        this.applicationId = applicationId
        this.certificateId = certificateId
        refreshScreen()
    }

    fun refreshScreen() {
        logDebug("refreshScreen", TAG)
        getApplicationDetailsUseCase.invoke(
            id = applicationId ?: return
        ).onEach { result ->
            result.onLoading {
                logDebug("getApplicationDetailsUseCase onLoading", TAG)
                showLoader()
            }.onSuccess { model, _, _ ->
                logDebug("getApplicationDetailsUseCase onSuccess", TAG)
                hideLoader()
                val language = APPLICATION_LANGUAGE.type
                val reason = model.nomenclatures?.mapNotNull { it.nomenclatures }?.flatten()
                        ?.firstOrNull { nomenclature ->
                            nomenclature.id == model.information?.fromJSON?.reasonId && nomenclature.language == language
                        }?.description ?: model.information?.fromJSON?.reasonText
                val modifiedModel = model.copy(
                    information = model.information?.copy(
                        fromJSON = model.information?.fromJSON?.copy(reasonText = reason)
                            ?: return@onSuccess
                    )
                )
                applicationDetails = modifiedModel
                _adapterListLiveData.emit(applicationDetailsUiMapper.map(modifiedModel))
            }.onFailure { _, _, message, responseCode, errorType ->
                logError("getApplicationDetailsUseCase onFailure", message, TAG)
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

    fun onTextClicked(model: CommonSimpleTextUi) {
        logDebug("onTextClicked", TAG)
        navigateInTab(
            MainTabEIMFragmentDirections.toCertificatesFlowFragment(
                certificateId = applicationDetails?.information?.fromJSON?.certificateId,
                applicationId = applicationId,
            )
        )
    }

    fun onButtonClicked(model: CommonButtonUi) {
        logDebug("onButtonClicked", TAG)
        when (model.elementEnum) {
            ApplicationDetailsElementsEnumUi.BUTTON_BACK -> onBackPressed()
            ApplicationDetailsElementsEnumUi.BUTTON_CONTINUE -> onContinueClicked()
            ApplicationDetailsElementsEnumUi.BUTTON_PAYMENT -> _openPaymentEvent.setValueOnMainThread(
                applicationDetails?.information?.fromJSON?.paymentAccessCode
            )

            ApplicationDetailsElementsEnumUi.BUTTON_REVOKE,
            ApplicationDetailsElementsEnumUi.BUTTON_STOP,
            ApplicationDetailsElementsEnumUi.BUTTON_RESUME -> completeApplication()
        }
    }

    private fun completeApplication() {
        logDebug("completeApplication", TAG)
        completeApplicationUseCase.invoke(
            id = applicationId ?: return
        ).onEach { result ->
            result.onLoading {
                logDebug("completeApplicationUseCase onLoading", TAG)
                showLoader()
            }.onSuccess { model, _, _ ->
                logDebug("completeApplicationUseCase onSuccess", TAG)
                isCertificateStatusChanged = true
                hideLoader()
                when (model) {
                    ApplicationCompletionStatusEnum.COMPLETED -> {
                        val applicationType = getEnumValue<ApplicationDetailsTypeEnum>(
                            applicationDetails?.information?.fromJSON?.applicationType ?: ""
                        ) ?: ApplicationDetailsTypeEnum.UNKNOWN

                        val message = when (applicationType) {
                            ApplicationDetailsTypeEnum.STOP_EID -> StringSource(R.string.certificate_stop_success_message)
                            ApplicationDetailsTypeEnum.RESUME_EID -> StringSource(R.string.certificate_resume_success_message)
                            ApplicationDetailsTypeEnum.REVOKE_EID -> StringSource(R.string.certificate_revoke_success_message)
                            else -> StringSource(R.string.unknown)
                        }

                        showMessage(
                            DialogMessage(
                                message = message,
                                title = StringSource(R.string.information),
                                positiveButtonText = StringSource(R.string.ok),
                            )
                        )
                    }

                    else -> {}
                }
            }.onFailure { _, title, message, responseCode, errorType ->
                logError("completeApplicationUseCase onFailure", message, TAG)
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

    private fun onContinueClicked() {
        logDebug("onContinueClicked", TAG)
        navigateInTab(
            MainTabEIMFragmentDirections.toApplicationContinueCreationFlowFragment(
                applicationId = applicationId ?: return
            )
        )
    }

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        when {
            findTabNavController().isFragmentInBackStack(R.id.certificatesFlowFragment) -> popBackStackToFragmentInTab(
                R.id.certificatesFlowFragment
            )

            findTabNavController().isFragmentInBackStack(R.id.certificatesFragment) -> popBackStackToFragmentInTab(
                R.id.certificatesFragment
            )

            certificateId.isNullOrEmpty() -> popBackStackToFragment(R.id.applicationsFragment)

            else -> navigateInTab(
                MainTabEIMFragmentDirections.toCertificatesFlowFragment(
                    applicationId = null,
                    certificateId = certificateId,
                )
            )
        }
    }
}