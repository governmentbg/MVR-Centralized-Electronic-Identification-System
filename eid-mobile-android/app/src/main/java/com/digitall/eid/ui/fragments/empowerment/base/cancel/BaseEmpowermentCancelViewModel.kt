/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.empowerment.base.cancel

import androidx.lifecycle.viewModelScope
import com.digitall.eid.R
import com.digitall.eid.domain.UiDateFormats
import com.digitall.eid.domain.extensions.fromServerDateToTextDate
import com.digitall.eid.domain.extensions.getEnumValue
import com.digitall.eid.domain.extensions.readOnly
import com.digitall.eid.domain.models.empowerment.common.all.EmpowermentItem
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.launchWithDispatcher
import com.digitall.eid.extensions.notEmpty
import com.digitall.eid.models.common.ButtonColorUi
import com.digitall.eid.models.common.DialogMessage
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.empowerment.common.cancel.EmpowermentCancelAdapterMarker
import com.digitall.eid.models.empowerment.common.cancel.EmpowermentCancelEnumUi
import com.digitall.eid.models.empowerment.common.details.EmpowermentDetailsStatementsElementsUi
import com.digitall.eid.models.empowerment.common.filter.EmpowermentFilterStatusEnumUi
import com.digitall.eid.models.empowerment.common.filter.EmpowermentOnBehalfOf
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonEditTextUi
import com.digitall.eid.models.list.CommonEditTextUiType
import com.digitall.eid.models.list.CommonSeparatorUi
import com.digitall.eid.models.list.CommonSimpleTextInFieldUi
import com.digitall.eid.models.list.CommonSimpleTextUi
import com.digitall.eid.models.list.CommonSpinnerMenuItemUi
import com.digitall.eid.models.list.CommonSpinnerUi
import com.digitall.eid.models.list.CommonTextFieldUi
import com.digitall.eid.models.list.CommonTitleSmallInFieldUi
import com.digitall.eid.models.list.CommonTitleSmallUi
import com.digitall.eid.models.list.CommonTitleUi
import com.digitall.eid.ui.BaseViewModel
import com.digitall.eid.utils.translateUidType
import kotlinx.coroutines.Job
import kotlinx.coroutines.flow.MutableStateFlow

abstract class BaseEmpowermentCancelViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "BaseEmpowermentCancelViewModelTag"
    }

    abstract fun refreshScreen()

    abstract fun sendResult(
        reason: String,
        empowermentId: String,
    )

    abstract val firstElementTitle: StringSource
    abstract val buttonTitle: StringSource
    abstract val successMessage: StringSource

    private var showErrors = false

    private var empowermentItem: EmpowermentItem? = null
    private var reason: String? = null

    protected val currentList = mutableListOf<EmpowermentCancelAdapterMarker>()

    private var editTextChangedJob: Job? = null

    protected fun onCancelSuccess() {
        showMessage(
            DialogMessage(
                message = successMessage,
                title = StringSource(R.string.information),
                positiveButtonText = StringSource(R.string.ok),
            )
        )
    }

    private val _adapterListLiveData =
        MutableStateFlow<List<EmpowermentCancelAdapterMarker>>(emptyList())
    val adapterListLiveData = _adapterListLiveData.readOnly()

    override fun onAlertDialogResult() {
        onBackPressed()
    }

    fun setupModel(model: EmpowermentItem, reasons: List<String>) {
        empowermentItem = model
        reason = reasons.firstOrNull()
        viewModelScope.launchWithDispatcher {
            buildUiElements()
        }
    }

    final override fun onSpinnerSelected(model: CommonSpinnerUi) {
        logDebug("onSpinnerSelected", TAG)
        viewModelScope.launchWithDispatcher {
            if (model.selectedValue !is CommonSpinnerMenuItemUi) return@launchWithDispatcher
            currentList.firstOrNull {
                it.elementEnum == model.elementEnum &&
                        it.elementId == model.elementId
            }?.let {
                val index = currentList.indexOf(it)
                currentList[index] = model.copy()
                if (model.elementEnum == EmpowermentCancelEnumUi.SPINNER_REASON) {
                    onReasonChanged(
                        index = index,
                        model = model,
                    )
                }
                checkErrors()
            } ?: run {
                logError("onSpinnerIncludedSelected null", TAG)
                showErrorState(
                    title = StringSource(R.string.error_internal_error_short),
                    description = StringSource("List element not found")
                )
            }
        }
    }

    suspend fun buildUiElements(reasons: List<String> = emptyList()) {
        val empowermentItem = empowermentItem ?: return
        val status =
            getEnumValue<EmpowermentFilterStatusEnumUi>(empowermentItem.calculatedStatusOn ?: "")
        val onBehalfOf = getEnumValue<EmpowermentOnBehalfOf>(empowermentItem.onBehalfOf ?: "")
        currentList.clear()
        currentList.add(
            CommonTitleUi(
                title = firstElementTitle,
            )
        )
        currentList.add(CommonSeparatorUi())

        if (reasons.isNotEmpty()) {
            currentList.add(
                CommonTextFieldUi(
                    required = true,
                    question = false,
                    title = EmpowermentCancelEnumUi.SPINNER_REASON.title,
                    elementEnum = EmpowermentCancelEnumUi.SPINNER_REASON,
                    text = StringSource(reasons.first()),
                    serverValue = reasons.first()
                )
            )
        }
        currentList.add(
            CommonTitleSmallUi(
                title = StringSource(R.string.empowerment_cancel_empowerment_details_title)
            )
        )
        currentList.add(
            CommonSimpleTextUi(
                title = StringSource(R.string.empowerment_cancel_number_title),
                text = StringSource(empowermentItem.number),
            )
        )
        status?.title?.let { statusName ->
            currentList.add(
                CommonSimpleTextUi(
                    text = statusName,
                    title = StringSource(R.string.empowerment_cancel_status_title),
                    colorRes = status.colorRes
                )
            )
        }
        empowermentItem.startDate?.let {
            currentList.add(
                CommonSimpleTextUi(
                    title = StringSource(R.string.empowerment_cancel_start_date_title),
                    text = StringSource(
                        it.fromServerDateToTextDate(
                            dateFormat = UiDateFormats.WITH_TIME,
                        )
                    ),
                )
            )
        }
        currentList.add(
            CommonSimpleTextUi(
                title = StringSource(R.string.empowerment_cancel_end_date_title),
                text = empowermentItem.expiryDate?.fromServerDateToTextDate(
                    dateFormat = UiDateFormats.WITH_TIME,
                )?.let {
                    StringSource(it)
                } ?: StringSource(R.string.empowerment_cancel_unlimited),
            )
        )
        when (onBehalfOf) {
            EmpowermentOnBehalfOf.INDIVIDUAL -> {
                currentList.add(
                    CommonSimpleTextUi(
                        title = StringSource(R.string.empowerment_cancel_from_title),
                        text = StringSource(R.string.empowerment_cancel_individual),
                    )
                )
                empowermentItem.uid?.let { uid ->
                    currentList.add(
                        CommonSimpleTextUi(
                            title = StringSource(R.string.empowerment_cancel_egn_lnch_empowerer_title),
                            text = StringSource(uid),
                        )
                    )
                }
            }

            EmpowermentOnBehalfOf.LEGAL_ENTITY -> {
                currentList.add(
                    CommonSimpleTextUi(
                        title = StringSource(R.string.empowerment_cancel_from_title),
                        text = StringSource(R.string.empowerment_cancel_legal_entity),
                    )
                )
                empowermentItem.uid?.let { uid ->
                    currentList.add(
                        CommonSimpleTextUi(
                            title = StringSource(R.string.empowerment_cancel_eik_bulstat_empowerer_title),
                            text = StringSource(uid),
                        )
                    )
                }
            }

            else -> {
                // NO
            }
        }
        empowermentItem.name?.let { name ->
            currentList.add(
                CommonSimpleTextUi(
                    title = StringSource(R.string.empowerment_cancel_name_title),
                    text = StringSource(name),
                )
            )
        }

        val empoweredUids = empowermentItem.empoweredUids?.filter {
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
                    title = StringSource(R.string.empowerment_cancel_egn_lnch_empowered_people),
                    text = StringSource(
                        sources = empoweredUids ?: emptyList(),
                        separator = "\n"
                    ),
                    maxLines = empoweredUids?.size ?: 0
                )
            )
        }
        val authorizedUids = empowermentItem.authorizerUids?.filter {
            !it.uid.isNullOrEmpty() && !it.uidType.isNullOrEmpty()
        }?.map {
            translateUidType(
                uidType = it.uidType,
                uidEmpowerer = it.uid,
                nameEmpowerer = it.name,
            )
        }
        if (authorizedUids.isNullOrEmpty().not()) {
            currentList.add(
                CommonSimpleTextUi(
                    title = StringSource(R.string.empowerment_cancel_legal_representatives_title),
                    text = StringSource(
                        sources = authorizedUids ?: emptyList(),
                        separator = "\n"
                    ),
                    maxLines = authorizedUids?.size ?: 0
                )
            )
        }

        empowermentItem.providerName?.let { providerName ->
            currentList.add(
                CommonSimpleTextUi(
                    title = StringSource(R.string.empowerment_cancel_supplier_title),
                    text = StringSource(providerName),
                )
            )
        }
        currentList.add(
            CommonSimpleTextUi(
                title = StringSource(R.string.empowerment_cancel_service_title),
                text = StringSource("${empowermentItem.serviceId} - ${empowermentItem.serviceName}"),
            )
        )
        empowermentItem.volumeOfRepresentation?.filter {
            !it.name.isNullOrEmpty()
        }?.forEach {
            it.name?.let { name ->
                currentList.add(
                    CommonSimpleTextUi(
                        title = StringSource(R.string.empowerment_cancel_extent_representative_power_title),
                        text = StringSource(name),
                    )
                )
            }
        }
        currentList.add(
            CommonTitleSmallInFieldUi(
                title = StringSource(R.string.empowerment_cancel_history_of_empowerment_title)
            )
        )
        empowermentItem.statusHistory?.filter {
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
                EmpowermentDetailsStatementsElementsUi.AWAITING_SIGNATURE,
            ).contains(element.first).not()
        }?.forEach { mappedStatus ->
            when (mappedStatus.first) {
                EmpowermentDetailsStatementsElementsUi.COLLECTING_AUTHORIZER_SIGNATURES -> {
                    empowermentItem.empowermentSignatures?.forEach { empowermentSignature ->
                        val signerModel =
                            empowermentItem.authorizerUids?.first { element -> element.uid == empowermentSignature.signerUid }
                        signerModel?.let { signer ->
                            mappedStatus.second?.fromServerDateToTextDate(
                                dateFormat = UiDateFormats.WITH_TIME,
                            )?.notEmpty { dateTime ->
                                currentList.add(
                                    CommonSimpleTextInFieldUi(
                                        title = mappedStatus.first.title,
                                        text = StringSource("$dateTime ${signer.name}"),
                                    )
                                )
                            }
                        }
                    }
                }

                else -> mappedStatus.second?.fromServerDateToTextDate(
                    dateFormat = UiDateFormats.WITH_TIME,
                )?.notEmpty { dateTime ->
                    currentList.add(
                        CommonSimpleTextInFieldUi(
                            title = mappedStatus.first.title,
                            text = StringSource(dateTime),
                        )
                    )
                }
            }
        }
        currentList.add(
            CommonButtonUi(
                title = buttonTitle,
                elementEnum = EmpowermentCancelEnumUi.BUTTON_APPLY,
                buttonColor = ButtonColorUi.RED,
                isEnabled = true,
            )
        )
        currentList.add(
            CommonButtonUi(
                title = EmpowermentCancelEnumUi.BUTTON_BACK.title,
                elementEnum = EmpowermentCancelEnumUi.BUTTON_BACK,
                buttonColor = ButtonColorUi.TRANSPARENT,
            )
        )
        _adapterListLiveData.emit(currentList.toList())
    }

    private suspend fun onReasonChanged(
        index: Int,
        model: CommonSpinnerUi,
    ) {
        if (model.selectedValue?.elementEnum == EmpowermentCancelEnumUi.SPINNER_REASON_OTHER) {
            logDebug("onReasonChanged SPINNER_REASON_OTHER", TAG)
            currentList.add(
                index + 1,
                CommonEditTextUi(
                    required = true,
                    question = false,
                    selectedValue = null,
                    type = CommonEditTextUiType.TEXT_INPUT,
                    title = EmpowermentCancelEnumUi.EDIT_TEXT_REASON.title,
                    elementEnum = EmpowermentCancelEnumUi.EDIT_TEXT_REASON,
                )
            )
            currentList.filterIsInstance<CommonButtonUi>().firstOrNull {
                it.elementEnum == EmpowermentCancelEnumUi.BUTTON_APPLY
            }?.let {
                currentList[currentList.indexOf(it)] = it.copy(
                    isEnabled = false,
                )
            } ?: run {
                logError("onEditTextChanged null", TAG)
                showErrorState(
                    title = StringSource(R.string.error_internal_error_short),
                    description = StringSource("List element not found")
                )
            }
        } else {
            logDebug("onReasonChanged not SPINNER_REASON_OTHER", TAG)
            currentList.filter {
                it.elementEnum == EmpowermentCancelEnumUi.EDIT_TEXT_REASON
            }.forEach {
                currentList.remove(it)
            }
            currentList.filterIsInstance<CommonButtonUi>().firstOrNull {
                it.elementEnum == EmpowermentCancelEnumUi.BUTTON_APPLY
            }?.let {
                currentList[currentList.indexOf(it)] = it.copy(
                    isEnabled = true,
                )
            } ?: run {
                logError("onEditTextChanged null", TAG)
                showErrorState(
                    title = StringSource(R.string.error_internal_error_short),
                    description = StringSource("List element not found")
                )
            }
        }
        _adapterListLiveData.emit(currentList.toList())
    }

    fun onButtonClicked(model: CommonButtonUi) {
        logDebug("onButtonClicked", TAG)
        viewModelScope.launchWithDispatcher {
            showLoader()
            when (model.elementEnum) {
                EmpowermentCancelEnumUi.BUTTON_APPLY -> {
                    showErrors = true
                    checkErrors(true)
                    _adapterListLiveData.emit(currentList.toList())
                }

                EmpowermentCancelEnumUi.BUTTON_BACK -> {
                    onBackPressed()
                }
            }
            hideLoader()
        }
    }

    fun onEnterTextDone(model: CommonEditTextUi) {
        logDebug("onEnterTextDone text: ${model.selectedValue}", TAG)
        editTextChangedJob?.cancel()
        editTextChangedJob = viewModelScope.launchWithDispatcher {
            changeEditText(model)
            _adapterListLiveData.emit(currentList.toList())
        }
    }

    fun onFocusChanged(model: CommonEditTextUi) {
        logDebug("onFocusChanged hasFocus: ${model.hasFocus}", TAG)
        editTextChangedJob?.cancel()
        editTextChangedJob = viewModelScope.launchWithDispatcher {
            changeEditText(model)
            if (!model.hasFocus) {
                _adapterListLiveData.emit(currentList.toList())
            }
        }
    }

    fun onEditTextChanged(model: CommonEditTextUi) {
        logDebug("onEditTextChanged text: ${model.selectedValue}", TAG)
        editTextChangedJob?.cancel()
        editTextChangedJob = viewModelScope.launchWithDispatcher {
            changeEditText(model)
        }
    }

    private fun changeEditText(model: CommonEditTextUi) {
        currentList.firstOrNull {
            it.elementEnum == EmpowermentCancelEnumUi.EDIT_TEXT_REASON
        }?.let {
            currentList[currentList.indexOf(it)] = model.copy()
        } ?: run {
            logError("onFocusChanged null", TAG)
            showErrorState(
                title = StringSource(R.string.error_internal_error_short),
                description = StringSource("List element not found")
            )
        }
        currentList.filterIsInstance<CommonButtonUi>().firstOrNull {
            it.elementEnum == EmpowermentCancelEnumUi.BUTTON_APPLY
        }?.let {
            currentList[currentList.indexOf(it)] = it.copy(
                isEnabled = !model.selectedValue.isNullOrEmpty(),
            )
        } ?: run {
            logError("onFocusChanged null", TAG)
            showErrorState(
                title = StringSource(R.string.error_internal_error_short),
                description = StringSource("List element not found")
            )
        }
    }

    private fun checkErrors(tryToSend: Boolean = false) {
        if (!showErrors) return
        when {
            tryToSend -> sendResult(
                reason = reason ?: return,
                empowermentId = empowermentItem?.id ?: return,
            )

            else -> {
                // no
            }
        }
    }


}