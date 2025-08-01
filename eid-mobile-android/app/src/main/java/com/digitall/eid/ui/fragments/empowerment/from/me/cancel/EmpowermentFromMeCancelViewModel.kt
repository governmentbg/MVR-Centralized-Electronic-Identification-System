/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.empowerment.from.me.cancel

import androidx.lifecycle.viewModelScope
import com.digitall.eid.R
import com.digitall.eid.domain.models.base.ErrorType
import com.digitall.eid.domain.models.base.onFailure
import com.digitall.eid.domain.models.base.onLoading
import com.digitall.eid.domain.models.base.onSuccess
import com.digitall.eid.domain.usecase.empowerment.cancel.CancelEmpowermentFromMeUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.isFragmentInBackStack
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.extensions.launchWithDispatcher
import com.digitall.eid.extensions.readOnly
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.ui.fragments.empowerment.base.cancel.BaseEmpowermentCancelViewModel
import com.digitall.eid.utils.SingleLiveEvent
import kotlinx.coroutines.flow.onEach
import org.koin.core.component.inject

class EmpowermentFromMeCancelViewModel : BaseEmpowermentCancelViewModel() {

    companion object {
        private const val TAG = "EmpowermentFromMeCancelViewModelTag"
    }

    private val cancelEmpowermentFromMeUseCase: CancelEmpowermentFromMeUseCase by inject()

    override val firstElementTitle = StringSource(R.string.empowerment_cancel_from_me_title)
    override val buttonTitle = StringSource(R.string.empowerment_cancel_from_me_button_title)
    override val successMessage = StringSource(R.string.empowerment_cancel_from_me_success)

    private val _showWidthawalInformationEvent = SingleLiveEvent<Unit>()
    val showWithdrawalInformationEvent = _showWidthawalInformationEvent.readOnly()

    private var reason: String = ""
    private var empowermentId: String = ""

    override fun refreshScreen() {
        logDebug("refreshScreen", TAG)
        viewModelScope.launchWithDispatcher {
            buildUiElements()
        }
    }

    override fun sendResult(
        reason: String,
        empowermentId: String,
    ) {
        logDebug("sendResult", TAG)
        this.reason = reason
        this.empowermentId = empowermentId
        _showWidthawalInformationEvent.callOnMainThread()
    }

    fun cancelEmpowerment() {
        logDebug("cancelEmpowerment", TAG)
        cancelEmpowermentFromMeUseCase.invoke(
            reason = reason,
            empowermentId = empowermentId,
        ).onEach { result ->
            result.onLoading {
                logDebug("cancelEmpowermentFromMeUseCase onLoading", TAG)
                showLoader()
            }.onSuccess { _, _, _ ->
                logDebug("cancelEmpowermentFromMeUseCase onSuccess", TAG)
                hideLoader()
                onCancelSuccess()
            }.onFailure { _, _, message, responseCode, errorType ->
                logError("cancelEmpowermentFromMeUseCase onFailure", message, TAG)
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