package com.digitall.eid.ui.fragments.payments.filter

import androidx.lifecycle.viewModelScope
import com.digitall.eid.R
import com.digitall.eid.domain.UiDateFormats
import com.digitall.eid.domain.extensions.atEndOfDay
import com.digitall.eid.domain.extensions.atStartOfDay
import com.digitall.eid.domain.extensions.getCalendar
import com.digitall.eid.domain.extensions.readOnly
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.launchWithDispatcher
import com.digitall.eid.extensions.readOnly
import com.digitall.eid.extensions.setValueOnMainThread
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonDatePickerUi
import com.digitall.eid.models.list.CommonEditTextUi
import com.digitall.eid.models.list.CommonEditTextUiType
import com.digitall.eid.models.list.CommonSpinnerMenuItemUi
import com.digitall.eid.models.list.CommonSpinnerUi
import com.digitall.eid.models.payments.history.all.PaymentAmountEnum
import com.digitall.eid.models.payments.history.all.PaymentReasonEnum
import com.digitall.eid.models.payments.history.all.PaymentStatusEnum
import com.digitall.eid.models.payments.history.filter.PaymentsHistoryFilterAdapterMarker
import com.digitall.eid.models.payments.history.filter.PaymentsHistoryFilterElementsEnum
import com.digitall.eid.models.payments.history.filter.PaymentsHistoryFilterModel
import com.digitall.eid.ui.BaseViewModel
import com.digitall.eid.utils.SingleLiveEvent
import kotlinx.coroutines.Job
import kotlinx.coroutines.flow.MutableStateFlow

class PaymentsHistoryFilterViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "PaymentsHistoryFilterViewModelTag"
    }

    private val currentList = mutableListOf<PaymentsHistoryFilterAdapterMarker>()

    private val _adapterListLiveData =
        MutableStateFlow<List<PaymentsHistoryFilterAdapterMarker>>(emptyList())
    val adapterListLiveData = _adapterListLiveData.readOnly()

    private val _applyFilterDataLiveData = SingleLiveEvent<PaymentsHistoryFilterModel>()
    val applyFilterDataLiveData = _applyFilterDataLiveData.readOnly()

    private var editTextChangedJob: Job? = null
    private var startElementsJob: Job? = null

    private val statusList = buildList {
        PaymentStatusEnum.entries.filterNot { element -> element == PaymentStatusEnum.UNKNOWN }
            .forEach { element ->
                add(
                    CommonSpinnerMenuItemUi(
                        text = element.title,
                        elementEnum = element,
                        isSelected = false
                    )
                )
            }
    }

    private val subjectList = buildList {
        PaymentReasonEnum.entries.filterNot { element -> element == PaymentReasonEnum.UNKNOWN }
            .forEach { element ->
                add(
                    CommonSpinnerMenuItemUi(
                        text = element.title,
                        elementEnum = element,
                        isSelected = false
                    )
                )
            }
    }

    private val amountList = buildList {
        PaymentAmountEnum.entries.forEach { element ->
            add(
                CommonSpinnerMenuItemUi(
                    text = element.title,
                    elementEnum = element,
                    isSelected = false
                )
            )
        }
    }

    private lateinit var filteringModel: PaymentsHistoryFilterModel

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        popBackStackToFragment(R.id.paymentsHistoryFragment)
    }

    fun setFilterModel(filterModel: PaymentsHistoryFilterModel) {
        filteringModel = filterModel
        setStartScreenElements()
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
            applyFilterData()
        }
    }

    fun clearFilterData() {
        viewModelScope.launchWithDispatcher {
            clearFilter()
        }
    }

    private fun applyFilterData() {
        logDebug("applyFilterData", TAG)
        var paymentId: String? = null
        var createdOn: Long? = null
        var status: PaymentStatusEnum? = null
        var subject: PaymentReasonEnum? = null
        var amount: PaymentAmountEnum? = null
        var paymentDate: Long? = null
        var validUntil: Long? = null
        currentList.forEach { element ->
            when (element) {
                is CommonSpinnerUi -> {
                    when (element.elementEnum) {
                        PaymentsHistoryFilterElementsEnum.SPINNER_STATUS -> status =
                            element.selectedValue?.elementEnum as PaymentStatusEnum

                        PaymentsHistoryFilterElementsEnum.SPINNER_SUBJECT -> subject =
                            element.selectedValue?.elementEnum as PaymentReasonEnum

                        PaymentsHistoryFilterElementsEnum.SPINNER_AMOUNT -> amount =
                            element.selectedValue?.elementEnum as PaymentAmountEnum
                    }
                }

                is CommonEditTextUi -> {
                    when (element.elementEnum) {
                        PaymentsHistoryFilterElementsEnum.EDIT_TEXT_PAYMENT_NUMBER -> paymentId =
                            element.selectedValue
                    }
                }

                is CommonDatePickerUi -> {
                    when (element.elementEnum) {
                        PaymentsHistoryFilterElementsEnum.DATE_PICKER_PAYMENT_DATE -> paymentDate =
                            element.selectedValue?.atStartOfDay()?.timeInMillis

                        PaymentsHistoryFilterElementsEnum.DATE_PICKER_CREATED_ON -> createdOn =
                            element.selectedValue?.atStartOfDay()?.timeInMillis

                        PaymentsHistoryFilterElementsEnum.DATE_PICKER_VALID_UNTIL -> validUntil =
                            element.selectedValue?.atEndOfDay()?.timeInMillis
                    }
                }
            }
        }
        filteringModel = filteringModel.copy(
            paymentId = paymentId,
            createdOn = createdOn,
            status = if (status != PaymentStatusEnum.ALL) status else null,
            subject = if (subject != PaymentReasonEnum.ALL) subject else null,
            amount = if (amount != PaymentAmountEnum.ALL) amount else null,
            paymentDate = paymentDate,
            validUntil = validUntil
        )
        _applyFilterDataLiveData.setValueOnMainThread(filteringModel)
        onBackPressed()
    }

    private fun clearFilter() {
        logDebug("clearFilter", TAG)
        filteringModel = PaymentsHistoryFilterModel(
            paymentId = null,
            status = null,
            createdOn = null,
            subject = null,
            amount = null,
            paymentDate = null,
            validUntil = null,
        )
        _applyFilterDataLiveData.setValueOnMainThread(filteringModel)
        onBackPressed()
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
                        elementEnum = PaymentsHistoryFilterElementsEnum.EDIT_TEXT_PAYMENT_NUMBER,
                        title = PaymentsHistoryFilterElementsEnum.EDIT_TEXT_PAYMENT_NUMBER.title,
                        selectedValue = filteringModel.paymentId,
                        type = CommonEditTextUiType.NUMBERS
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
                        title = PaymentsHistoryFilterElementsEnum.SPINNER_STATUS.title,
                        elementEnum = PaymentsHistoryFilterElementsEnum.SPINNER_STATUS,
                    ),
                )
                add(
                    CommonDatePickerUi(
                        question = false,
                        required = false,
                        selectedValue = filteringModel.createdOn?.let {
                            getCalendar(timeInMillis = it)
                        },
                        minDate = getCalendar(minusYears = 100),
                        maxDate = getCalendar(plusYears = 100),
                        title = PaymentsHistoryFilterElementsEnum.DATE_PICKER_CREATED_ON.title,
                        elementEnum = PaymentsHistoryFilterElementsEnum.DATE_PICKER_CREATED_ON,
                        dateFormat = UiDateFormats.WITHOUT_TIME,
                    )
                )
                add(
                    CommonSpinnerUi(
                        required = false,
                        question = false,
                        list = subjectList,
                        selectedValue = filteringModel.subject?.let {
                            CommonSpinnerMenuItemUi(
                                isSelected = false,
                                elementEnum = it,
                                text = it.title,
                            )
                        } ?: subjectList.first(),
                        title = PaymentsHistoryFilterElementsEnum.SPINNER_SUBJECT.title,
                        elementEnum = PaymentsHistoryFilterElementsEnum.SPINNER_SUBJECT,
                    ),
                )
                add(
                    CommonSpinnerUi(
                        required = false,
                        question = false,
                        list = amountList,
                        selectedValue = filteringModel.amount?.let {
                            CommonSpinnerMenuItemUi(
                                isSelected = false,
                                elementEnum = it,
                                text = it.title,
                            )
                        } ?: amountList.first(),
                        title = PaymentsHistoryFilterElementsEnum.SPINNER_AMOUNT.title,
                        elementEnum = PaymentsHistoryFilterElementsEnum.SPINNER_AMOUNT,
                    ),
                )
                add(
                    CommonDatePickerUi(
                        question = false,
                        required = false,
                        selectedValue = filteringModel.paymentDate?.let {
                            getCalendar(timeInMillis = it)
                        },
                        minDate = getCalendar(minusYears = 100),
                        maxDate = getCalendar(plusYears = 100),
                        title = PaymentsHistoryFilterElementsEnum.DATE_PICKER_PAYMENT_DATE.title,
                        elementEnum = PaymentsHistoryFilterElementsEnum.DATE_PICKER_PAYMENT_DATE,
                        dateFormat = UiDateFormats.WITHOUT_TIME,
                    )
                )
                add(
                    CommonDatePickerUi(
                        question = false,
                        required = false,
                        selectedValue = filteringModel.validUntil?.let {
                            getCalendar(timeInMillis = it)
                        },
                        minDate = getCalendar(minusYears = 100),
                        maxDate = getCalendar(plusYears = 100),
                        title = PaymentsHistoryFilterElementsEnum.DATE_PICKER_VALID_UNTIL.title,
                        elementEnum = PaymentsHistoryFilterElementsEnum.DATE_PICKER_VALID_UNTIL,
                        dateFormat = UiDateFormats.WITHOUT_TIME,
                    )
                )
            }
            _adapterListLiveData.emit(currentList.toList())
        }
    }
}