/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.applications.show.filter

import androidx.lifecycle.viewModelScope
import com.digitall.eid.R
import com.digitall.eid.domain.APPLICATION_LANGUAGE
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
import com.digitall.eid.domain.models.common.ApplicationLanguage
import com.digitall.eid.domain.usecase.administrators.GetAdministratorsUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.extensions.launchWithDispatcher
import com.digitall.eid.extensions.readOnly
import com.digitall.eid.extensions.setValueOnMainThread
import com.digitall.eid.models.applications.all.ApplicationStatusEnum
import com.digitall.eid.models.applications.all.ApplicationTypeEnum
import com.digitall.eid.models.applications.filter.ApplicationDeviceType
import com.digitall.eid.models.applications.filter.ApplicationsFilterAdapterMarker
import com.digitall.eid.models.applications.filter.ApplicationsFilterElementsEnumUi
import com.digitall.eid.models.applications.filter.ApplicationsFilterModel
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

class ApplicationFilterViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "ApplicationFilterViewModelTag"
    }

    private val getAdministratorsUseCase: GetAdministratorsUseCase by inject()

    private val currentList = mutableListOf<ApplicationsFilterAdapterMarker>()

    private var administrators: List<AdministratorModel>? = null

    private val _adapterListLiveData =
        MutableStateFlow<List<ApplicationsFilterAdapterMarker>>(emptyList())
    val adapterListLiveData = _adapterListLiveData.readOnly()

    private val _applyFilterDataLiveData = SingleLiveEvent<ApplicationsFilterModel>()
    val applyFilterDataLiveData = _applyFilterDataLiveData.readOnly()

    private val _scrollToPositionLiveData = SingleLiveEvent<Int>()
    val scrollToPositionLiveData = _scrollToPositionLiveData.readOnly()

    private var editTextChangedJob: Job? = null
    private var startElementsJob: Job? = null

    private lateinit var filteringModel: ApplicationsFilterModel

    fun setFilteringModel(filterModel: ApplicationsFilterModel) {
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
            }.onFailure { _, title, message, responseCode, errorType ->
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
                        required = false,
                        question = false,
                        selectedValue = filteringModel.id,
                        type = CommonEditTextUiType.TEXT_INPUT,
                        title = ApplicationsFilterElementsEnumUi.EDIT_TEXT_APPLICATION_NUMBER.title,
                        elementEnum = ApplicationsFilterElementsEnumUi.EDIT_TEXT_APPLICATION_NUMBER,
                    )
                )
                add(
                    CommonDatePickerUi(
                        question = false,
                        required = false,
                        selectedValue = filteringModel.createDate?.let {
                            getCalendar(timeInMillis = it)
                        },
                        minDate = getCalendar(minusYears = 100),
                        maxDate = getCalendar(plusYears = 100),
                        title = ApplicationsFilterElementsEnumUi.DATE_PICKER_CREATION_DATE.title,
                        elementEnum = ApplicationsFilterElementsEnumUi.DATE_PICKER_CREATION_DATE,
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
                        title = ApplicationsFilterElementsEnumUi.SPINNER_STATUS.title,
                        elementEnum = ApplicationsFilterElementsEnumUi.SPINNER_STATUS,
                    ),
                )
                add(
                    CommonSpinnerUi(
                        required = false,
                        question = false,
                        selectedValue = filteringModel.applicationType?.let {
                            CommonSpinnerMenuItemUi(
                                isSelected = false,
                                elementEnum = it,
                                text = it.title,
                                serverValue = it.serverValue,
                            )
                        } ?: applicationTypeList.first(),
                        title = ApplicationsFilterElementsEnumUi.SPINNER_SUBJECT.title,
                        elementEnum = ApplicationsFilterElementsEnumUi.SPINNER_SUBJECT,
                        list = applicationTypeList,
                    )
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
                        title = ApplicationsFilterElementsEnumUi.SPINNER_DEVICE_TYPE.title,
                        elementEnum = ApplicationsFilterElementsEnumUi.SPINNER_DEVICE_TYPE,
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
                                elementEnum = ApplicationsFilterElementsEnumUi.DIALOG_ADMINISTRATOR,
                            )
                        } ?: CommonDialogWithSearchItemUi(
                            text = StringSource(R.string.all),
                            elementEnum = ApplicationsFilterElementsEnumUi.DIALOG_ADMINISTRATOR,
                        ),
                        title = ApplicationsFilterElementsEnumUi.DIALOG_ADMINISTRATOR.title,
                        elementEnum = ApplicationsFilterElementsEnumUi.DIALOG_ADMINISTRATOR,
                        list = buildList {
                            add(
                                CommonDialogWithSearchItemUi(
                                    text = StringSource(R.string.all),
                                    elementEnum = ApplicationsFilterElementsEnumUi.DIALOG_ADMINISTRATOR,
                                )
                            )
                            administrators?.let {
                                val language =
                                    APPLICATION_LANGUAGE
                                addAll(
                                    it.map { data ->
                                        CommonDialogWithSearchItemUi(
                                            serverValue = data.id,
                                            originalModel = data,
                                            text = when (language) {
                                                ApplicationLanguage.BG -> StringSource(
                                                    data.name ?: ""
                                                )

                                                ApplicationLanguage.EN -> StringSource(
                                                    data.nameLatin ?: ""
                                                )
                                            },
                                            elementEnum = ApplicationsFilterElementsEnumUi.DIALOG_ADMINISTRATOR,
                                        )
                                    }
                                )
                            }
                        },
                    )
                )
            }
            _adapterListLiveData.emit(currentList.toList())
            delay(100)
            _scrollToPositionLiveData.setValueOnMainThread(0)
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

    private suspend fun tryToApplyFilterData() {
        logDebug("tryToApplyFilterData", TAG)
        currentList.filterIsInstance<CommonDialogWithSearchUi>().filter {
            it.elementEnum == ApplicationsFilterElementsEnumUi.DIALOG_ADMINISTRATOR
        }.forEach {
            if (it.selectedValue == null) {
                logError("tryToApplyFilterData DIALOG_ADMINISTRATOR selectedValue == null", TAG)
                currentList[currentList.indexOf(it)] = it.copy(
                    validationError = StringSource(R.string.please_select)
                )
                _adapterListLiveData.emit(currentList.toList())
            } else {
                currentList[currentList.indexOf(it)] = it.copy(
                    validationError = null,
                )
                _adapterListLiveData.emit(currentList.toList())
                applyFilterData()
            }
        }
    }

    private fun applyFilterData() {
        logDebug("applyFilterData", TAG)
        var applicationNumber: String? = null
        var createDate: Long? = null
        var status: ApplicationStatusEnum? = null
        var administrator: AdministratorModel? = null
        var deviceType: ApplicationDeviceType? = null
        var applicationType: ApplicationTypeEnum? = null
        currentList.forEach {
            when {
                it is CommonSpinnerUi &&
                        it.elementEnum == ApplicationsFilterElementsEnumUi.SPINNER_STATUS &&
                        it.selectedValue?.elementEnum != null &&
                        it.selectedValue.elementEnum is ApplicationStatusEnum -> {
                    status = it.selectedValue.elementEnum
                    logDebug(
                        "applyFilterData SPINNER_STATUS status: ${status.serverValue}",
                        TAG
                    )
                }

                it is CommonEditTextUi &&
                        it.elementEnum == ApplicationsFilterElementsEnumUi.EDIT_TEXT_APPLICATION_NUMBER &&
                        !it.selectedValue.isNullOrEmpty() -> {
                    applicationNumber = it.selectedValue
                    logDebug(
                        "applyFilterData EDIT_TEXT_APPLICATION_NUMBER id: $applicationNumber",
                        TAG
                    )
                }

                it is CommonDatePickerUi &&
                        it.elementEnum == ApplicationsFilterElementsEnumUi.DATE_PICKER_CREATION_DATE &&
                        it.selectedValue != null -> {
                    createDate = it.selectedValue.timeInMillis
                    logDebug(
                        "applyFilterData DATE_PICKER_CREATION_DATE createDate: $createDate",
                        TAG
                    )
                }

                it is CommonDialogWithSearchUi &&
                        it.elementEnum == ApplicationsFilterElementsEnumUi.DIALOG_ADMINISTRATOR &&
                        it.selectedValue?.originalModel != null &&
                        it.selectedValue.originalModel is AdministratorModel -> {
                    administrator = it.selectedValue.originalModel
                    logDebug(
                        "applyFilterData DIALOG_ADMINISTRATOR administrator: ${administrator.id}",
                        TAG
                    )
                }

                it is CommonSpinnerUi &&
                        it.elementEnum == ApplicationsFilterElementsEnumUi.SPINNER_DEVICE_TYPE &&
                        it.selectedValue?.elementEnum != null &&
                        it.selectedValue.elementEnum is ApplicationDeviceType -> {
                    deviceType = it.selectedValue.elementEnum
                    logDebug(
                        "applyFilterData SPINNER_DEVICE_TYPE deviceType: ${deviceType.serverValue}",
                        TAG
                    )
                }

                it is CommonSpinnerUi &&
                        it.elementEnum == ApplicationsFilterElementsEnumUi.SPINNER_SUBJECT &&
                        it.selectedValue?.elementEnum != null &&
                        it.selectedValue.elementEnum is ApplicationTypeEnum -> {
                    applicationType = it.selectedValue.elementEnum
                    logDebug(
                        "applyFilterData SPINNER_SUBJECT applicationType: ${applicationType.serverValue}",
                        TAG
                    )
                }
            }
        }
        filteringModel = filteringModel.copy(
            applicationNumber = applicationNumber,
            createDate = createDate,
            status = if (status == ApplicationStatusEnum.ALL) null else status,
            administrator = administrator,
            applicationType = if (applicationType == ApplicationTypeEnum.ALL) null else applicationType,
            deviceType = if (deviceType?.serverValue.isNullOrEmpty()) null else deviceType,
        )
        _applyFilterDataLiveData.setValueOnMainThread(filteringModel)
        onBackPressed()
    }

    private fun clearFilter() {
        logDebug("clearFilter", TAG)
        filteringModel = ApplicationsFilterModel(
            id = null,
            applicationNumber = null,
            status = null,
            createDate = null,
            deviceType = null,
            administrator = null,
            applicationType = null,
        )
        _applyFilterDataLiveData.setValueOnMainThread(filteringModel)
        onBackPressed()
    }

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        popBackStackToFragment(R.id.applicationsFragment)
    }

    private val statusList = listOf(
        CommonSpinnerMenuItemUi(
            isSelected = false,
            elementEnum = ApplicationStatusEnum.ALL,
            text = ApplicationStatusEnum.ALL.title,
        ),
        CommonSpinnerMenuItemUi(
            isSelected = false,
            elementEnum = ApplicationStatusEnum.SIGNED,
            text = ApplicationStatusEnum.SIGNED.title,
        ),
        CommonSpinnerMenuItemUi(
            isSelected = false,
            elementEnum = ApplicationStatusEnum.SUBMITTED,
            text = ApplicationStatusEnum.SUBMITTED.title,
        ),
        CommonSpinnerMenuItemUi(
            isSelected = false,
            elementEnum = ApplicationStatusEnum.PENDING_PAYMENT,
            text = ApplicationStatusEnum.PENDING_PAYMENT.title,
        ),
        CommonSpinnerMenuItemUi(
            isSelected = false,
            elementEnum = ApplicationStatusEnum.PAID,
            text = ApplicationStatusEnum.PAID.title,
        ),
        CommonSpinnerMenuItemUi(
            isSelected = false,
            elementEnum = ApplicationStatusEnum.DENIED,
            text = ApplicationStatusEnum.DENIED.title,
        ),
        CommonSpinnerMenuItemUi(
            isSelected = false,
            elementEnum = ApplicationStatusEnum.APPROVED,
            text = ApplicationStatusEnum.APPROVED.title,
        ),
        CommonSpinnerMenuItemUi(
            isSelected = false,
            elementEnum = ApplicationStatusEnum.COMPLETED,
            text = ApplicationStatusEnum.COMPLETED.title,
        ),
    )

    private val deviceList = buildList {
        val allElement = ApplicationDeviceType(
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
        val language = APPLICATION_LANGUAGE
        DEVICES.forEach { deviceModel ->
            val element = ApplicationDeviceType(
                type = deviceModel.type ?: "",
                serverValue = deviceModel.id,
                title = when (language) {
                    ApplicationLanguage.BG -> StringSource(deviceModel.name ?: "")
                    ApplicationLanguage.EN -> StringSource(deviceModel.description ?: "")
                }
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

    private val applicationTypeList = listOf(
        CommonSpinnerMenuItemUi(
            isSelected = false,
            elementEnum = ApplicationTypeEnum.ALL,
            text = ApplicationTypeEnum.ALL.title,
            serverValue = ApplicationTypeEnum.ALL.serverValue,
        ),
        CommonSpinnerMenuItemUi(
            isSelected = false,
            elementEnum = ApplicationTypeEnum.ISSUE_EID,
            text = ApplicationTypeEnum.ISSUE_EID.title,
            serverValue = ApplicationTypeEnum.ISSUE_EID.serverValue,
        ),
        CommonSpinnerMenuItemUi(
            isSelected = false,
            elementEnum = ApplicationTypeEnum.RESUME_EID,
            text = ApplicationTypeEnum.RESUME_EID.title,
            serverValue = ApplicationTypeEnum.RESUME_EID.serverValue,
        ),
        CommonSpinnerMenuItemUi(
            isSelected = false,
            elementEnum = ApplicationTypeEnum.REVOKE_EID,
            text = ApplicationTypeEnum.REVOKE_EID.title,
            serverValue = ApplicationTypeEnum.REVOKE_EID.serverValue,
        ),
        CommonSpinnerMenuItemUi(
            isSelected = false,
            elementEnum = ApplicationTypeEnum.STOP_EID,
            text = ApplicationTypeEnum.STOP_EID.title,
            serverValue = ApplicationTypeEnum.STOP_EID.serverValue,
        ),
    )

}