/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.certificates.filter

import androidx.lifecycle.viewModelScope
import com.digitall.eid.R
import com.digitall.eid.domain.DELAY_500
import com.digitall.eid.domain.DEVICES
import com.digitall.eid.domain.UiDateFormats
import com.digitall.eid.domain.extensions.getCalendar
import com.digitall.eid.domain.extensions.readOnly
import com.digitall.eid.domain.models.administrators.AdministratorModel
import com.digitall.eid.domain.models.base.ErrorType
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onLoading
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.usecase.administrators.GetAdministratorsUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.extensions.launchWithDispatcher
import com.digitall.eid.extensions.readOnly
import com.digitall.eid.extensions.setValueOnMainThread
import com.digitall.eid.models.certificates.all.CertificatesStatusEnum
import com.digitall.eid.models.certificates.filter.CertificateDeviceType
import com.digitall.eid.models.certificates.filter.CertificateFilterAdapterMarker
import com.digitall.eid.models.certificates.filter.CertificatesFilterElementsEnumUi
import com.digitall.eid.models.certificates.filter.CertificatesFilterModel
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonDatePickerUi
import com.digitall.eid.models.list.CommonDialogWithSearchItemUi
import com.digitall.eid.models.list.CommonDialogWithSearchUi
import com.digitall.eid.models.list.CommonEditTextUi
import com.digitall.eid.models.list.CommonEditTextUiType
import com.digitall.eid.models.list.CommonSpinnerMenuItemUi
import com.digitall.eid.models.list.CommonSpinnerUi
import com.digitall.eid.ui.BaseViewModel
import com.digitall.eid.utils.SingleLiveEvent
import kotlinx.coroutines.Job
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.onEach
import org.koin.core.component.inject

class CertificateFilterViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "CertificateFilterViewModelTag"
    }

    private val getAdministratorsUseCase: GetAdministratorsUseCase by inject()

    private val currentList = mutableListOf<CertificateFilterAdapterMarker>()

    private var administrators: List<AdministratorModel>? = null

    private val _adapterListLiveData =
        MutableStateFlow<List<CertificateFilterAdapterMarker>>(emptyList())
    val adapterListLiveData = _adapterListLiveData.readOnly()

    private val _applyFilterDataLiveData = SingleLiveEvent<CertificatesFilterModel>()
    val applyFilterDataLiveData = _applyFilterDataLiveData.readOnly()

    private val _scrollToPositionLiveData = SingleLiveEvent<Int>()
    val scrollToPositionLiveData = _scrollToPositionLiveData.readOnly()

    private var editTextChangedJob: Job? = null
    private var startElementsJob: Job? = null

    private lateinit var filteringModel: CertificatesFilterModel

    fun setFilteringModel(filterModel: CertificatesFilterModel) {
        showLoader()
        filteringModel = filterModel
        setStartScreenElements()
        refreshScreen()
        hideLoader()
    }

    fun refreshScreen() {
        logDebug("refreshScreen", TAG)
        getAdministratorsUseCase.invoke().onEach { result ->
            result.onLoading {
                logDebug("getAdministratorsUseCase onLoading", TAG)
            }.onSuccess { model, _, _ ->
                logDebug("getAdministratorsUseCase onSuccess", TAG)
                administrators = model
                setStartScreenElements()
                delay(DELAY_500)
                hideErrorState()
                hideLoader()
            }.onFailure { _, _, message, responseCode, errorType ->
                logError("getAdministratorsUseCase onFailure", message, TAG)
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

    private fun setStartScreenElements() {
        startElementsJob?.cancel()
        startElementsJob = viewModelScope.launchWithDispatcher {
            currentList.apply {
                clear()
                add(
                    CommonEditTextUi(
                        question = false,
                        required = false,
                        elementEnum = CertificatesFilterElementsEnumUi.EDIT_TEXT_SERIAL_NUMBER,
                        title = CertificatesFilterElementsEnumUi.EDIT_TEXT_SERIAL_NUMBER.title,
                        selectedValue = filteringModel.serialNumber,
                        type = CommonEditTextUiType.TEXT_INPUT
                    )
                )
                add(
                    CommonEditTextUi(
                        question = false,
                        required = false,
                        elementEnum = CertificatesFilterElementsEnumUi.EDIT_TEXT_ALIAS,
                        title = CertificatesFilterElementsEnumUi.EDIT_TEXT_ALIAS.title,
                        selectedValue = filteringModel.alias,
                        type = CommonEditTextUiType.TEXT_INPUT_CAP
                    )
                )
                add(
                    CommonDatePickerUi(
                        question = false,
                        required = false,
                        selectedValue = filteringModel.validityFrom?.let {
                            getCalendar(timeInMillis = it)
                        },
                        minDate = getCalendar(minusYears = 100),
                        maxDate = getCalendar(plusYears = 100),
                        title = CertificatesFilterElementsEnumUi.DATE_PICKER_ISSUED_ON.title,
                        elementEnum = CertificatesFilterElementsEnumUi.DATE_PICKER_ISSUED_ON,
                        dateFormat = UiDateFormats.WITHOUT_TIME,
                    )
                )
                add(
                    CommonDatePickerUi(
                        question = false,
                        required = false,
                        selectedValue = filteringModel.validityUntil?.let {
                            getCalendar(timeInMillis = it)
                        },
                        minDate = getCalendar(minusYears = 100),
                        maxDate = getCalendar(plusYears = 100),
                        title = CertificatesFilterElementsEnumUi.DATE_PICKER_VALID_UNTIL.title,
                        elementEnum = CertificatesFilterElementsEnumUi.DATE_PICKER_VALID_UNTIL,
                        dateFormat = UiDateFormats.WITHOUT_TIME,
                    )
                )
                add(
                    CommonSpinnerUi(
                        required = false,
                        question = false,
                        list = statusList,
                        selectedValue = filteringModel.status?.let {
                            CommonSpinnerMenuItemUi(
                                isSelected = false,
                                elementEnum = it,
                                text = it.title,
                            )
                        } ?: statusList.first(),
                        title = CertificatesFilterElementsEnumUi.SPINNER_STATUS.title,
                        elementEnum = CertificatesFilterElementsEnumUi.SPINNER_STATUS,
                    ),
                )
                add(
                    CommonSpinnerUi(
                        required = false,
                        question = false,
                        selectedValue = filteringModel.deviceType?.let {
                            CommonSpinnerMenuItemUi(
                                isSelected = false,
                                elementEnum = it,
                                text = it.title,
                                serverValue = it.serverValue,
                            )
                        } ?: deviceList.first(),
                        title = CertificatesFilterElementsEnumUi.SPINNER_DEVICE_TYPE.title,
                        elementEnum = CertificatesFilterElementsEnumUi.SPINNER_DEVICE_TYPE,
                        list = deviceList,
                    )
                )
                add(
                    CommonDialogWithSearchUi(
                        required = false,
                        question = false,
                        selectedValue = filteringModel.administrator?.let {
                            CommonDialogWithSearchItemUi(
                                serverValue = it.id,
                                originalModel = it,
                                text = StringSource(it.name ?: ""),
                                elementEnum = CertificatesFilterElementsEnumUi.DIALOG_ADMINISTRATOR,
                            )
                        },
                        title = CertificatesFilterElementsEnumUi.DIALOG_ADMINISTRATOR.title,
                        elementEnum = CertificatesFilterElementsEnumUi.DIALOG_ADMINISTRATOR,
                        list = administrators?.filter { administrator ->
                            administrator.active == true
                        }?.map { data ->
                            CommonDialogWithSearchItemUi(
                                serverValue = data.id,
                                originalModel = data,
                                text = StringSource(data.name ?: ""),
                                elementEnum = CertificatesFilterElementsEnumUi.DIALOG_ADMINISTRATOR,
                            )
                        }?.takeIf { list -> list.isNotEmpty() } ?: listOf(
                            CommonDialogWithSearchItemUi(
                                text = StringSource(R.string.no_search_results),
                                selectable = false
                            )
                        )
                    )
                )
            }
            _adapterListLiveData.emit(currentList.toList())
        }
    }

    override fun onDialogElementSelected(model: CommonDialogWithSearchUi) {
        logDebug("onDialogElementSelected", TAG)
        viewModelScope.launchWithDispatcher {
            if (model.selectedValue == null) return@launchWithDispatcher
            currentList.firstOrNull {
                it.elementEnum == model.elementEnum
            }?.let {
                currentList[currentList.indexOf(it)] = model.copy()
                _adapterListLiveData.emit(currentList.toList())
            } ?: run {
                logError("onDialogElementSelected null", TAG)
                showErrorState(
                    title = StringSource(R.string.error_internal_error_short),
                    description = StringSource("List element not found")
                )
            }
        }
    }

    override fun onSpinnerSelected(model: CommonSpinnerUi) {
        logDebug("onSpinnerSelected", TAG)
        viewModelScope.launchWithDispatcher {
            logDebug("onSpinnerSelected", TAG)
            if (model.selectedValue?.text == null) {
                logError("onSpinnerSelected not value", TAG)
                return@launchWithDispatcher
            }
            currentList.firstOrNull {
                it.elementEnum == model.elementEnum
            }?.let {
                currentList[currentList.indexOf(it)] = model.copy()
                _adapterListLiveData.emit(currentList.toList())
            } ?: run {
                logError("onSpinnerSelected null", TAG)
                showErrorState(
                    title = StringSource(R.string.error_internal_error_short),
                    description = StringSource("List element not found")
                )
            }
        }
    }

    override fun onDatePickerChanged(model: CommonDatePickerUi) {
        logDebug("onDatePickerChanged", TAG)
        viewModelScope.launchWithDispatcher {
            logDebug("onDatePickerChanged", TAG)
            currentList.firstOrNull {
                it.elementEnum == model.elementEnum
            }?.let {
                currentList[currentList.indexOf(it)] = model.copy()
                _adapterListLiveData.emit(currentList.toList())
            } ?: run {
                logError("onDatePickerChanged null", TAG)
                showErrorState(
                    title = StringSource(R.string.error_internal_error_short),
                    description = StringSource("List element not found")
                )
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

    private fun changeEditText(model: CommonEditTextUi) {
        currentList.firstOrNull {
            it.elementEnum == model.elementEnum
        }?.let {
            currentList[currentList.indexOf(it)] = model.copy()
        } ?: run {
            logError("onDatePickerChanged null", TAG)
            showErrorState(
                title = StringSource(R.string.error_internal_error_short),
                description = StringSource("List element not found")
            )
        }
    }

    fun tryApplyFilterData() {
        viewModelScope.launchWithDispatcher {
            tryToApplyFilterData()
        }
    }

    fun clearFilterData() {
        viewModelScope.launchWithDispatcher {
            clearFilter()
        }
    }

    private fun tryToApplyFilterData() {
        logDebug("tryToApplyFilterData", TAG)
        applyFilterData()
    }

    private fun applyFilterData() {
        logDebug("applyFilterData", TAG)
        var serialNumber: String? = null
        var validityFrom: Long? = null
        var validityUntil: Long? = null
        var status: CertificatesStatusEnum? = null
        var administrator: AdministratorModel? = null
        var deviceType: CertificateDeviceType? = null
        var alias: String? = null
        currentList.forEach {
            when {
                it is CommonSpinnerUi &&
                        it.elementEnum == CertificatesFilterElementsEnumUi.SPINNER_STATUS &&
                        it.selectedValue?.elementEnum != null &&
                        it.selectedValue.elementEnum is CertificatesStatusEnum -> {
                    status = it.selectedValue.elementEnum
                    logDebug(
                        "applyFilterData SPINNER_STATUS status: ${status?.serverValue}",
                        TAG
                    )
                }

                it is CommonEditTextUi &&
                        it.elementEnum == CertificatesFilterElementsEnumUi.EDIT_TEXT_SERIAL_NUMBER &&
                        !it.selectedValue.isNullOrEmpty() -> {
                    serialNumber = it.selectedValue
                    logDebug(
                        "applyFilterData EDIT_TEXT_SERIAL_NUMBER serialNumber: $serialNumber",
                        TAG
                    )
                }

                it is CommonDatePickerUi &&
                        it.elementEnum == CertificatesFilterElementsEnumUi.DATE_PICKER_ISSUED_ON &&
                        it.selectedValue != null -> {
                    validityFrom = it.selectedValue.timeInMillis
                    logDebug(
                        "applyFilterData DATE_PICKER_ISSUED_ON validityFrom: $validityFrom",
                        TAG
                    )
                }

                it is CommonDatePickerUi &&
                        it.elementEnum == CertificatesFilterElementsEnumUi.DATE_PICKER_VALID_UNTIL &&
                        it.selectedValue != null -> {
                    validityUntil = it.selectedValue.timeInMillis
                    logDebug(
                        "applyFilterData DATE_PICKER_VALID_UNTIL validityUntil: $validityUntil",
                        TAG
                    )
                }

                it is CommonDialogWithSearchUi &&
                        it.elementEnum == CertificatesFilterElementsEnumUi.DIALOG_ADMINISTRATOR &&
                        it.selectedValue?.originalModel != null &&
                        it.selectedValue.originalModel is AdministratorModel -> {
                    administrator = it.selectedValue.originalModel
                    logDebug(
                        "applyFilterData DIALOG_ADMINISTRATOR administrator: ${administrator?.id}",
                        TAG
                    )
                }

                it is CommonSpinnerUi &&
                        it.elementEnum == CertificatesFilterElementsEnumUi.SPINNER_DEVICE_TYPE &&
                        it.selectedValue?.elementEnum != null &&
                        it.selectedValue.elementEnum is CertificateDeviceType -> {
                    deviceType = it.selectedValue.elementEnum
                    logDebug(
                        "applyFilterData SPINNER_DEVICE_TYPE deviceType: ${deviceType?.serverValue}",
                        TAG
                    )
                }

                it is CommonEditTextUi &&
                        it.elementEnum == CertificatesFilterElementsEnumUi.EDIT_TEXT_ALIAS &&
                        !it.selectedValue.isNullOrEmpty() -> {
                    alias = it.selectedValue
                    logDebug(
                        "applyFilterData EDIT_TEXT_SERIAL_NUMBER serialNumber: $serialNumber",
                        TAG
                    )
                }
            }
        }
        filteringModel = filteringModel.copy(
            status = if (status == CertificatesStatusEnum.ALL) null else status,
            deviceType = if (deviceType?.serverValue.isNullOrEmpty()) null else deviceType,
            serialNumber = serialNumber,
            validityFrom = validityFrom,
            validityUntil = validityUntil,
            administrator = administrator,
            alias = alias,
        )
        _applyFilterDataLiveData.setValueOnMainThread(filteringModel)
        onBackPressed()
    }

    private fun clearFilter() {
        logDebug("clearFilter", TAG)
        filteringModel = CertificatesFilterModel(
            id = null,
            status = null,
            deviceType = null,
            serialNumber = null,
            validityFrom = null,
            validityUntil = null,
            administrator = null,
            alias = null,
        )
        _applyFilterDataLiveData.setValueOnMainThread(filteringModel)
        onBackPressed()
    }

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        popBackStackToFragment(R.id.certificatesFragment)
    }

    private val statusList = listOf(
        CommonSpinnerMenuItemUi(
            isSelected = false,
            elementEnum = CertificatesStatusEnum.ALL,
            text = CertificatesStatusEnum.ALL.title,
        ),
        CommonSpinnerMenuItemUi(
            isSelected = false,
            elementEnum = CertificatesStatusEnum.ACTIVE,
            text = CertificatesStatusEnum.ACTIVE.title,
        ),
        CommonSpinnerMenuItemUi(
            isSelected = false,
            elementEnum = CertificatesStatusEnum.STOPPED,
            text = CertificatesStatusEnum.STOPPED.title,
        ),
        CommonSpinnerMenuItemUi(
            isSelected = false,
            elementEnum = CertificatesStatusEnum.REVOKED,
            text = CertificatesStatusEnum.REVOKED.title,
        ),
        CommonSpinnerMenuItemUi(
            isSelected = false,
            elementEnum = CertificatesStatusEnum.EXPIRED,
            text = CertificatesStatusEnum.EXPIRED.title,
        )
    )

    private val deviceList = buildList {
        val allElement = CertificateDeviceType(
            type = "",
            serverValue = null,
            title = StringSource(R.string.all)
        )
        add(
            CommonSpinnerMenuItemUi(
                isSelected = false,
                elementEnum = allElement,
                text = allElement.title,
                serverValue = allElement.serverValue
            )
        )
        DEVICES.forEach { deviceModel ->
            val element = CertificateDeviceType(
                type = deviceModel.type ?: "",
                serverValue = deviceModel.id,
                title = StringSource(deviceModel.name)
            )
            add(
                CommonSpinnerMenuItemUi(
                    isSelected = false,
                    elementEnum = element,
                    text = element.title,
                    serverValue = element.serverValue,
                )
            )
        }
    }

}