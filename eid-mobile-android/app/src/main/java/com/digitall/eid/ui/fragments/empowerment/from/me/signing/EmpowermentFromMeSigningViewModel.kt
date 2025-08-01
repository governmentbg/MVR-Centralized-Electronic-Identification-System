/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.empowerment.from.me.signing

import androidx.lifecycle.viewModelScope
import com.digitall.eid.R
import com.digitall.eid.domain.DEFAULT_INACTIVITY_TIMEOUT_MILLISECONDS
import com.digitall.eid.domain.SIGNING_REQUEST_TIMEOUT
import com.digitall.eid.domain.UiDateFormats
import com.digitall.eid.domain.extensions.fromServerDateToTextDate
import com.digitall.eid.domain.extensions.getEnumValue
import com.digitall.eid.domain.extensions.readOnly
import com.digitall.eid.domain.models.base.ErrorType
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onLoading
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.models.empowerment.common.all.EmpowermentItem
import com.digitall.eid.domain.usecase.empowerment.signing.EmpowermentSignWithBoricaUseCase
import com.digitall.eid.domain.usecase.empowerment.signing.EmpowermentSignWithEvrotrustUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.isFragmentInBackStack
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.extensions.launchWithDispatcher
import com.digitall.eid.extensions.notEmpty
import com.digitall.eid.extensions.readOnly
import com.digitall.eid.extensions.setValueOnMainThread
import com.digitall.eid.models.common.AlertDialogResult
import com.digitall.eid.models.common.ButtonColorUi
import com.digitall.eid.models.common.DialogMessage
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.empowerment.common.details.EmpowermentDetailsStatementsElementsUi
import com.digitall.eid.models.empowerment.common.filter.EmpowermentFilterStatusEnumUi
import com.digitall.eid.models.empowerment.common.filter.EmpowermentOnBehalfOf
import com.digitall.eid.models.empowerment.signing.EmpowermentFromMeSigningAdapterMarker
import com.digitall.eid.models.empowerment.signing.EmpowermentFromMeSigningEnumUi
import com.digitall.eid.models.empowerment.signing.EmpowermentFromMeSigningMethodsEnumUi
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonLabeledSimpleTextUi
import com.digitall.eid.models.list.CommonSeparatorUi
import com.digitall.eid.models.list.CommonSimpleTextInFieldUi
import com.digitall.eid.models.list.CommonSimpleTextUi
import com.digitall.eid.models.list.CommonSpinnerMenuItemUi
import com.digitall.eid.models.list.CommonSpinnerUi
import com.digitall.eid.models.list.CommonTitleSmallInFieldUi
import com.digitall.eid.models.list.CommonTitleSmallUi
import com.digitall.eid.models.list.CommonTitleUi
import com.digitall.eid.ui.BaseViewModel
import com.digitall.eid.utils.SingleLiveEvent
import com.digitall.eid.utils.translateUidType
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.onEach
import org.koin.core.component.inject

class EmpowermentFromMeSigningViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "EmpowermentFromMeSigningViewModelTag"
        private val LOADING_MESSAGES = mapOf(
            "PLEASE_WAIT" to R.string.wait,
            "OPEN_EVROTRUST_APPLICATION" to R.string.evrotrust_open_application_message,
            "OPEN_BORICA_APPLICATION" to R.string.borica_open_application_message,
        )
    }

    private val empowermentSignWithBoricaUseCase: EmpowermentSignWithBoricaUseCase by inject()
    private val empowermentSignWithEvrotrustUseCase: EmpowermentSignWithEvrotrustUseCase by inject()

    private var empowermentFromMeSigningMethodsEnumUi =
        EmpowermentFromMeSigningMethodsEnumUi.EVROTRUST

    private val currentList = mutableListOf<EmpowermentFromMeSigningAdapterMarker>()

    private val _adapterListLiveData =
        MutableStateFlow<List<EmpowermentFromMeSigningAdapterMarker>>(emptyList())
    val adapterListLiveData = _adapterListLiveData.readOnly()

    private val _requestRefreshLiveData = SingleLiveEvent<Unit>()
    val requestRefreshLiveData = _requestRefreshLiveData.readOnly()

    private var empowermentItem: EmpowermentItem? = null

    fun setupModel(model: EmpowermentItem) {
        logDebug("setupModel", TAG)
        viewModelScope.launchWithDispatcher {
            empowermentItem = model
            val status = getEnumValue<EmpowermentFilterStatusEnumUi>(model.calculatedStatusOn ?: "")
            val onBehalfOf = getEnumValue<EmpowermentOnBehalfOf>(model.onBehalfOf ?: "")
            currentList.add(
                CommonTitleUi(
                    title = StringSource(R.string.empowerment_signing_sign_button_title),
                )
            )
            currentList.add(CommonSeparatorUi())
            currentList.add(
                CommonTitleSmallUi(
                    title = StringSource(R.string.empowerment_details_title)
                )
            )
            currentList.add(
                CommonSimpleTextUi(
                    title = StringSource(R.string.empowerment_details_number_title),
                    text = StringSource(model.id),
                )
            )
            status?.title?.let { statusName ->
                currentList.add(
                    CommonSimpleTextUi(
                        text = statusName,
                        title = StringSource(R.string.empowerment_details_status_title),
                        colorRes = status.colorRes
                    )
                )
            }
            model.startDate?.fromServerDateToTextDate(
                dateFormat = UiDateFormats.WITH_TIME,
            )?.notEmpty {
                currentList.add(
                    CommonSimpleTextUi(
                        title = StringSource(R.string.empowerment_details_start_date_title),
                        text = StringSource(it),
                    )
                )
            }
            currentList.add(
                CommonSimpleTextUi(
                    title = StringSource(R.string.empowerment_details_end_date_title),
                    text = if (model.expiryDate.isNullOrEmpty())
                        StringSource(R.string.empowerment_details_indefinitely)
                    else StringSource(
                        model.expiryDate?.fromServerDateToTextDate(
                            dateFormat = UiDateFormats.WITH_TIME,
                        )
                    ),
                )
            )
            when (onBehalfOf) {
                EmpowermentOnBehalfOf.INDIVIDUAL -> {
                    currentList.add(
                        CommonSimpleTextUi(
                            title = StringSource(R.string.empowerment_details_onbehalfof_title),
                            text = StringSource(R.string.empowerment_details_onbehalfof_individual_enum_type),
                        )
                    )
                    model.uid?.let { uid ->
                        currentList.add(
                            CommonSimpleTextUi(
                                title = StringSource(R.string.empowerment_details_from_id_individual_title),
                                text = StringSource(uid),
                            )
                        )
                    }
                }

                EmpowermentOnBehalfOf.LEGAL_ENTITY -> {
                    currentList.add(
                        CommonSimpleTextUi(
                            title = StringSource(R.string.empowerment_details_onbehalfof_title),
                            text = StringSource(R.string.empowerment_details_onbehalfof_legal_entity_enum_type),
                        )
                    )
                    model.uid?.let { uid ->
                        currentList.add(
                            CommonSimpleTextUi(
                                title = StringSource(R.string.empowerment_details_from_id_legal_entity_title),
                                text = StringSource(uid),
                            )
                        )
                    }
                }

                else -> {
                    // NO
                }
            }
            model.name?.let { name ->
                currentList.add(
                    CommonSimpleTextUi(
                        title = StringSource(R.string.empowerment_details_name_title),
                        text = StringSource(name),
                    )
                )
            }

            val empoweredUids = model.empoweredUids?.filter {
                !it.uid.isNullOrEmpty() && !it.uidType.isNullOrEmpty()
            }?.map {
                translateUidType(
                    uidType = it.uidType,
                    uidEmpowerer = it.uid,
                    nameEmpowerer = it.name,
                )
            }
            if (empoweredUids.isNullOrEmpty().not()) {
                currentList.add(
                    CommonSimpleTextUi(
                        title = StringSource(R.string.empowerment_details_empowered_people_ids_title),
                        text = StringSource(
                            sources = empoweredUids,
                            separator = "\n"
                        ),
                        maxLines = empoweredUids.size
                    )
                )
                if (empoweredUids.size > 1) {
                    currentList.add(
                        CommonLabeledSimpleTextUi(
                            title = StringSource(R.string.empowerment_details_empowerment_type_title),
                            labeledText = StringSource(R.string.empowerment_details_empowerment_type_together_title),
                            text = StringSource(R.string.empowerment_details_empowerment_type_together_description),
                            iconResLeft = R.drawable.ic_caution
                        )
                    )
                }
            }
            val authorizedUids = model.authorizerUids?.filter {
                !it.uid.isNullOrEmpty() && !it.uidType.isNullOrEmpty()
            }?.map {
                translateUidType(
                    uidType = it.uidType,
                    uidEmpowerer = it.uid,
                    nameEmpowerer = it.name,
                )
            }
            if (authorizedUids.isNullOrEmpty().not()
                && onBehalfOf == EmpowermentOnBehalfOf.LEGAL_ENTITY) {
                currentList.add(
                    CommonSimpleTextUi(
                        title = StringSource(R.string.empowerment_details_legal_representatives_title),
                        text = StringSource(
                            sources = authorizedUids,
                            separator = "\n"
                        ),
                        maxLines = authorizedUids.size
                    )
                )
            }
            model.providerName?.let { providerName ->
                currentList.add(
                    CommonSimpleTextUi(
                        title = StringSource(R.string.empowerment_details_services_provider_title),
                        text = StringSource(providerName),
                    )
                )
            }
            currentList.add(
                CommonSimpleTextUi(
                    title = StringSource(R.string.empowerment_details_service_title),
                    text = StringSource("${model.serviceId} - ${model.serviceName}"),
                )
            )
            model.volumeOfRepresentation?.filter {
                !it.name.isNullOrEmpty()
            }?.mapNotNull { it.name }?.let {
                currentList.add(
                    CommonSimpleTextUi(
                        title = StringSource(R.string.empowerment_details_scope_title),
                        text = StringSource(it.joinToString(separator = ",\n")),
                        maxLines = 24,
                    )
                )
            }
            currentList.add(
                CommonTitleSmallInFieldUi(
                    title = StringSource(R.string.empowerment_details_empowerment_history_title)
                )
            )
            model.statusHistory?.filter {
                !it.id.isNullOrEmpty() &&
                        !it.status.isNullOrEmpty() &&
                        !it.dateTime.isNullOrEmpty()
            }?.map {
                (getEnumValue<EmpowermentDetailsStatementsElementsUi>(it.status ?: "")
                    ?: EmpowermentDetailsStatementsElementsUi.NONE) to it.dateTime
            }?.filter { element ->
                listOf(
                    EmpowermentDetailsStatementsElementsUi.NONE,
                    EmpowermentDetailsStatementsElementsUi.COLLECTING_WITHDRAWAL_SIGNATURES,
                    EmpowermentDetailsStatementsElementsUi.AWAITING_SIGNATURE
                ).contains(element.first).not()
            }?.forEach { statusHistory ->
                when (statusHistory.first) {
                    EmpowermentDetailsStatementsElementsUi.COLLECTING_AUTHORIZER_SIGNATURES -> {
                        model.empowermentSignatures?.forEach { empowermentSignature ->
                            val signerModel = model.authorizerUids?.first { element -> element.uid == empowermentSignature.signerUid }
                            signerModel?.let { signer ->
                                statusHistory.second?.fromServerDateToTextDate(
                                    dateFormat = UiDateFormats.WITH_TIME,
                                )?.notEmpty { dateTime ->
                                    currentList.add(
                                        CommonSimpleTextInFieldUi(
                                            title = statusHistory.first.title,
                                            text = StringSource("$dateTime ${signer.name}"),
                                        )
                                    )
                                }
                            }
                        }
                    }
                    else -> statusHistory.second?.fromServerDateToTextDate(
                        dateFormat = UiDateFormats.WITH_TIME,
                    )?.notEmpty { dateTime ->
                        currentList.add(
                            CommonSimpleTextInFieldUi(
                                title = statusHistory.first.title,
                                text = StringSource(dateTime),
                            )
                        )
                    }
                }
            }

            val isSignedByMe =
                model.empowermentSignatures?.any {
                        element -> element.signerUid == preferences.readApplicationInfo()?.userModel?.citizenIdentifier
                } == true

            if (isSignedByMe.not()) {
                currentList.add(
                    CommonSpinnerUi(
                        required = true,
                        question = false,
                        title = EmpowermentFromMeSigningEnumUi.SPINNER_METHOD.title,
                        elementEnum = EmpowermentFromMeSigningEnumUi.SPINNER_METHOD,
                        selectedValue = CommonSpinnerMenuItemUi(
                            isSelected = false,
                            text = EmpowermentFromMeSigningMethodsEnumUi.EVROTRUST.title,
                            elementEnum = EmpowermentFromMeSigningMethodsEnumUi.EVROTRUST,
                        ),
                        list = EmpowermentFromMeSigningMethodsEnumUi.entries.map { entity ->
                            CommonSpinnerMenuItemUi(
                                isSelected = entity == empowermentFromMeSigningMethodsEnumUi,
                                text = entity.title,
                                elementEnum = entity
                            )
                        }
                    )
                )
                currentList.add(
                    CommonButtonUi(
                        title = EmpowermentFromMeSigningEnumUi.BUTTON_SIGN.title,
                        elementEnum = EmpowermentFromMeSigningEnumUi.BUTTON_SIGN,
                        buttonColor = ButtonColorUi.BLUE,
                    )
                )
            }
            currentList.add(
                CommonButtonUi(
                    title = EmpowermentFromMeSigningEnumUi.BUTTON_BACK.title,
                    elementEnum = EmpowermentFromMeSigningEnumUi.BUTTON_BACK,
                    buttonColor = ButtonColorUi.TRANSPARENT,
                )
            )
            updateList()
        }
    }

    fun onButtonClicked(model: CommonButtonUi) {
        logDebug("onButtonClicked", TAG)
        viewModelScope.launchWithDispatcher {
            when(model.elementEnum) {
                EmpowermentFromMeSigningEnumUi.BUTTON_SIGN  -> signEmpowerment()
                EmpowermentFromMeSigningEnumUi.BUTTON_BACK -> onBackPressed()
            }
        }
    }

    private fun signEmpowerment() {
        logDebug("signEmpowerment", TAG)
        inactivityTimer.setNewInactivityTimeout(timeoutInMilliseconds = SIGNING_REQUEST_TIMEOUT)
        (when (empowermentFromMeSigningMethodsEnumUi) {
            EmpowermentFromMeSigningMethodsEnumUi.EVROTRUST -> empowermentSignWithEvrotrustUseCase.invoke(
                empowermentItem ?: return
            )
            EmpowermentFromMeSigningMethodsEnumUi.BORIKA -> empowermentSignWithBoricaUseCase.invoke(
                empowermentItem ?: return
            )
        }).onEach { result ->
            result.onLoading { message ->
                logDebug("signWithEvrotrust onLoading", TAG)
                showFullscreenLoader(
                    message = LOADING_MESSAGES[message]?.let { resId ->
                        StringSource(resId)
                    } ?: run { StringSource(message) }
                )
            }.onSuccess { _, _, _ ->
                logDebug("signWithEvrotrust onSuccess", TAG)
                inactivityTimer.setNewInactivityTimeout(timeoutInMilliseconds = DEFAULT_INACTIVITY_TIMEOUT_MILLISECONDS)
                hideFullscreenLoader()
                showMessage(
                    DialogMessage(
                        message = StringSource(R.string.empowerment_signing_success_message),
                        title = StringSource(R.string.information),
                        positiveButtonText = StringSource(R.string.ok),
                    )
                )
            }.onFailure { _, title, message, responseCode, errorType ->
                logError("signWithEvrotrust onFailure", message, TAG)
                inactivityTimer.setNewInactivityTimeout(timeoutInMilliseconds = DEFAULT_INACTIVITY_TIMEOUT_MILLISECONDS)
                hideFullscreenLoader()
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

    override fun onSpinnerSelected(model: CommonSpinnerUi) {
        logDebug("onSpinnerSelected", TAG)
        viewModelScope.launchWithDispatcher {
            if (model.selectedValue !is CommonSpinnerMenuItemUi) return@launchWithDispatcher
            if (model.selectedValue.elementEnum !is EmpowermentFromMeSigningMethodsEnumUi) return@launchWithDispatcher
            empowermentFromMeSigningMethodsEnumUi = model.selectedValue.elementEnum
            currentList.firstOrNull {
                it.elementEnum == EmpowermentFromMeSigningEnumUi.SPINNER_METHOD
            }?.let {
                currentList[currentList.indexOf(it)] = model.copy(validationError = null)
                updateList()
            } ?: run {
                logError("onSpinnerSelected null", TAG)
                showErrorState(
                    title = StringSource(R.string.error_internal_error_short),
                    description = StringSource("List element not found")
                )
            }
        }
    }

    private suspend fun updateList() {
        logDebug("updateList", TAG)
        _adapterListLiveData.emit(currentList.toList())
    }

    override fun onAlertDialogResult(result: AlertDialogResult) {
        logDebug("onAlertDialogResult", TAG)
        if (result.isPositive.not()) return
        _requestRefreshLiveData.setValueOnMainThread(Unit)
        onBackPressed()
    }

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        when {
            findFlowNavController().isFragmentInBackStack(R.id.empowermentsLegalFragment) -> popBackStackToFragment(
                R.id.empowermentsLegalFragment
            )

            else -> popBackStackToFragment(
                R.id.empowermentFromMeFragment
            )
        }
    }

}