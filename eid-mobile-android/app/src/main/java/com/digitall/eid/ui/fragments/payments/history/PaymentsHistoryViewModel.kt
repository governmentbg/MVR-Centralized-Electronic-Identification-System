package com.digitall.eid.ui.fragments.payments.history

import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.viewModelScope
import com.digitall.eid.R
import com.digitall.eid.domain.DELAY_1000
import com.digitall.eid.domain.DELAY_500
import com.digitall.eid.domain.extensions.atStartOfDay
import com.digitall.eid.domain.extensions.fromServerDate
import com.digitall.eid.domain.models.base.ErrorType
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onLoading
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.usecase.payments.history.GetPaymentsHistoryUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.extensions.launchWithDispatcher
import com.digitall.eid.extensions.readOnly
import com.digitall.eid.extensions.setValueOnMainThread
import com.digitall.eid.mappers.payments.history.PaymentsHistoryUiMapper
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonSpinnerMenuItemUi
import com.digitall.eid.models.list.CommonSpinnerUi
import com.digitall.eid.models.main.MainTabsEnum
import com.digitall.eid.models.payments.history.all.PaymentAmountEnum
import com.digitall.eid.models.payments.history.all.PaymentAmountUnit
import com.digitall.eid.models.payments.history.all.PaymentHistoryElementsEnumUi
import com.digitall.eid.models.payments.history.all.PaymentHistorySortCriteriaEnum
import com.digitall.eid.models.payments.history.all.PaymentHistoryUi
import com.digitall.eid.models.payments.history.all.PaymentReasonEnum
import com.digitall.eid.models.payments.history.all.PaymentStatusEnum
import com.digitall.eid.models.payments.history.filter.PaymentsHistoryFilterModel
import com.digitall.eid.ui.BaseViewModel
import com.digitall.eid.utils.SingleLiveEvent
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.onEach
import org.koin.core.component.inject
import kotlin.properties.Delegates

class PaymentsHistoryViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "PaymentHistoryViewModelTag"
    }

    override var mainTabsEnum: MainTabsEnum? = MainTabsEnum.TAB_MORE

    private val getPaymentsHistoryUseCase: GetPaymentsHistoryUseCase by inject()
    private val paymentsHistoryUiMapper: PaymentsHistoryUiMapper by inject()

    private val _adapterListLiveData =
        MutableLiveData<List<PaymentHistoryUi>>(emptyList())
    val adapterListLiveData = _adapterListLiveData.readOnly()

    private val _isFilterInitEvent = SingleLiveEvent<Boolean>()
    val isFilterInitEvent = _isFilterInitEvent.readOnly()

    private val _sortingCriteriaSpinnerLiveData = SingleLiveEvent<CommonSpinnerUi>()
    val sortingCriteriaSpinnerLiveData = _sortingCriteriaSpinnerLiveData.readOnly()

    private val _currentSortingCriteriaLiveData =
        MutableLiveData(PaymentHistorySortCriteriaEnum.DEFAULT)
    val currentSortingCriteriaLiveData = _currentSortingCriteriaLiveData.readOnly()

    private var payments = emptyList<PaymentHistoryUi>()
    private var sortedPayments = emptyList<PaymentHistoryUi>()
    private var filteredPayments = emptyList<PaymentHistoryUi>()
    private var currentSortingCriteria = PaymentHistorySortCriteriaEnum.DEFAULT
        set(value) {
            field = value
            applySortingAndFilter()
        }

    private var filteringModel: PaymentsHistoryFilterModel by Delegates.observable(
        PaymentsHistoryFilterModel(
            paymentId = null,
            createdOn = null,
            status = null,
            subject = null,
            amount = null,
            paymentDate = null,
            validUntil = null,
        )
    ) { _, _, newValue ->
        _isFilterInitEvent.setValueOnMainThread(newValue.allPropertiesAreNull.not())
        applySortingAndFilter()
    }

    override fun onFirstAttach() {
        refreshScreen()
    }

    fun refreshScreen() {
        getPaymentsHistoryUseCase.invoke().onEach { result ->
            result.onLoading {
                logDebug("getHistory onLoading", TAG)
                showLoader()
            }.onSuccess { model, _, _ ->
                logDebug("getHistory onSuccess", TAG)
                payments = paymentsHistoryUiMapper.mapList(model)
                applySortingAndFilter()
                delay(DELAY_1000)
                hideLoader()
                hideErrorState()
            }.onFailure { _, title, message, responseCode, errorType ->
                logError("getHistory onFailure", message, TAG)
                delay(DELAY_500)
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

    fun checkIfFilterIsInit() {
        _isFilterInitEvent.setValueOnMainThread(filteringModel.allPropertiesAreNull.not())
    }

    fun updateFilteringModel(filterModel: PaymentsHistoryFilterModel) {
        viewModelScope.launchWithDispatcher {
            filterModel.apply {
                filteringModel = copy(
                    paymentId = paymentId,
                    createdOn = createdOn,
                    status = status,
                    subject = subject,
                    amount = amount,
                    paymentDate = paymentDate,
                    validUntil = validUntil,
                )
            }
        }
    }

    fun onFilterClicked() {
        logDebug("onFilterClicked", TAG)
        navigateInFlow(
            PaymentsHistoryFragmentDirections.toPaymentsHistoryFilterFragment(
                model = filteringModel
            )
        )
    }

    fun onSortingSpinnerClicked() {
        logDebug("onSortingCriteriaClicked", TAG)
        viewModelScope.launchWithDispatcher {
            val current = currentSortingCriteriaLiveData.value
            val data = CommonSpinnerUi(
                required = false,
                question = false,
                selectedValue = null,
                title = StringSource(""),
                elementEnum = PaymentHistoryElementsEnumUi.SPINNER_SORTING_CRITERIA,
                list = PaymentHistorySortCriteriaEnum.entries.map {
                    CommonSpinnerMenuItemUi(
                        elementEnum = it,
                        text = it.title,
                        isSelected = it == current
                    )
                }
            )
            _sortingCriteriaSpinnerLiveData.setValueOnMainThread(data)
        }
    }

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        backToTab()
    }

    override fun onSpinnerSelected(model: CommonSpinnerUi) {
        logDebug("onSpinnerSelected", TAG)
        viewModelScope.launchWithDispatcher {
            when (model.elementEnum) {
                PaymentHistoryElementsEnumUi.SPINNER_SORTING_CRITERIA -> {
                    val newCriteria =
                        model.selectedValue?.elementEnum as? PaymentHistorySortCriteriaEnum
                            ?: return@launchWithDispatcher
                    currentSortingCriteria = newCriteria
                    _currentSortingCriteriaLiveData.setValueOnMainThread(newCriteria)
                }
            }
        }
    }

    private fun applySortingAndFilter() {
        applySorting()
        applyFilterData()
    }

    private fun applySorting() {
        sortedPayments = when (currentSortingCriteria) {
            PaymentHistorySortCriteriaEnum.DEFAULT -> payments
            PaymentHistorySortCriteriaEnum.CREATED_ON_ASC -> payments.sortedBy { element -> element.createdOn.fromServerDate()?.timeInMillis }
            PaymentHistorySortCriteriaEnum.CREATED_ON_DESC -> payments.sortedByDescending { element -> element.createdOn.fromServerDate()?.timeInMillis }
            PaymentHistorySortCriteriaEnum.SUBJECT_ASC -> payments.sortedBy { element -> element.reason.ordinal }
            PaymentHistorySortCriteriaEnum.SUBJECT_DESC -> payments.sortedByDescending { element -> element.reason.ordinal }
            PaymentHistorySortCriteriaEnum.PAYMENT_DATE_ASC -> payments.sortedBy { element -> element.paymentDate.fromServerDate()?.timeInMillis }
            PaymentHistorySortCriteriaEnum.PAYMENT_DATE_DESC -> payments.sortedByDescending { element -> element.paymentDate.fromServerDate()?.timeInMillis }
            PaymentHistorySortCriteriaEnum.VALID_UNTIL_ASC -> payments.sortedBy { element -> element.paymentDeadline.fromServerDate()?.timeInMillis }
            PaymentHistorySortCriteriaEnum.VALID_UNTIL_DESC -> payments.sortedByDescending { element -> element.paymentDeadline.fromServerDate()?.timeInMillis }
            PaymentHistorySortCriteriaEnum.STATUS_ASC -> payments.sortedBy { element -> element.status.ordinal }
            PaymentHistorySortCriteriaEnum.STATUS_DESC -> payments.sortedByDescending { element -> element.status.ordinal }
            PaymentHistorySortCriteriaEnum.AMOUNT_ASC -> payments.sortedBy { element -> element.amount }
            PaymentHistorySortCriteriaEnum.AMOUNT_DESC -> payments.sortedByDescending { element -> element.amount }
            PaymentHistorySortCriteriaEnum.LAST_SYNC_ASC -> payments.sortedBy { element -> element.lastSync.fromServerDate()?.timeInMillis }
            PaymentHistorySortCriteriaEnum.LAST_SYNC_DESC -> payments.sortedByDescending { element -> element.lastSync.fromServerDate()?.timeInMillis }

        }
    }

    private fun applyFilterData() {
        filteredPayments = filteringModel.paymentId?.let { paymentId ->
            sortedPayments.filter { element -> element.ePaymentId.contains(paymentId) }
        } ?: sortedPayments

        filteredPayments = filteringModel.status?.let { status ->
            when (status) {
                PaymentStatusEnum.ALL -> filteredPayments
                else -> filteredPayments.filter { element -> element.status == status }
            }
        } ?: filteredPayments

        filteredPayments = filteringModel.createdOn?.let { createdOn ->
            filteredPayments.filter { element -> element.createdOn.fromServerDate()?.atStartOfDay()?.timeInMillis == createdOn }
        } ?: filteredPayments

        filteredPayments = filteringModel.subject?.let { subject ->
            when (subject) {
                PaymentReasonEnum.ALL -> filteredPayments
                else -> filteredPayments.filter { element -> element.reason == subject }
            }
        } ?: filteredPayments

        filteredPayments = filteringModel.amount?.let { amount ->
            when (amount) {
                PaymentAmountEnum.ALL -> filteredPayments
                PaymentAmountEnum.BELOW -> filteredPayments.filter { element -> element.amount < (amount.amount as PaymentAmountUnit.Integer).value }
                PaymentAmountEnum.BETWEEN -> filteredPayments.filter { element ->
                    (amount.amount as PaymentAmountUnit.IntegerRange).value.contains(
                        element.amount.toInt()
                    )
                }

                PaymentAmountEnum.OVER -> filteredPayments.filter { element -> element.amount > (amount.amount as PaymentAmountUnit.Integer).value }
            }
        } ?: filteredPayments

        filteredPayments = filteringModel.paymentDate?.let { paymentDate ->
            filteredPayments.filter { element -> element.paymentDate.fromServerDate()?.atStartOfDay()?.timeInMillis == paymentDate }
        } ?: filteredPayments

        filteredPayments = filteringModel.validUntil?.let { validUntil ->
            filteredPayments.filter { element ->
                (element.paymentDeadline.fromServerDate()?.timeInMillis ?: 0) <= validUntil
            }
        } ?: filteredPayments

        _adapterListLiveData.setValueOnMainThread(filteredPayments)
    }
}