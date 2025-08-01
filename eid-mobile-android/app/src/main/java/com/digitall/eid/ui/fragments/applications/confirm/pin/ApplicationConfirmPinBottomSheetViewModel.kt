package com.digitall.eid.ui.fragments.applications.confirm.pin

import androidx.lifecycle.MutableLiveData
import com.digitall.eid.R
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.extensions.readOnly
import com.digitall.eid.extensions.setValueOnMainThread
import com.digitall.eid.models.base.pin.create.ConfirmPinErrorsEnum
import com.digitall.eid.models.common.AlertDialogResult
import com.digitall.eid.models.common.CreatePinScreenStates
import com.digitall.eid.models.common.DialogMessage
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.ui.BaseViewModel
import com.digitall.eid.utils.SingleLiveEvent

class ApplicationConfirmPinBottomSheetViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "ApplicationConfirmPinBottomSheetViewModelTag"

        const val DIALOG_EXIT_PIN_CREATION = "DIALOG_EXIT_PIN_CREATION"
    }

    private val _screenStateLiveData = MutableLiveData(CreatePinScreenStates.ENTER)
    val screenStateLiveData = _screenStateLiveData.readOnly()

    private val _errorMessageLiveData = SingleLiveEvent<StringSource?>()
    val errorMessageLiveData = _errorMessageLiveData.readOnly()

    private val _clearPinLiveData = SingleLiveEvent<Unit>()
    val clearPinLiveData = _clearPinLiveData.readOnly()

    private val _pinLiveData = SingleLiveEvent<String?>()
    val pinLiveData = _pinLiveData.readOnly()

    private var pin: String? = null

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        showCancellationMessage()
    }

    override fun onAlertDialogResult(result: AlertDialogResult) {
        if (result.isPositive) {
            _pinLiveData.setValueOnMainThread(null)
        }
    }

    fun onPinStartEnter() {
        _errorMessageLiveData.setValueOnMainThread(null)
    }

    fun completePinCreation() {
        _pinLiveData.setValueOnMainThread(pin)
    }

    fun onPinEntered(pin: String) {
        when (screenStateLiveData.value) {
            CreatePinScreenStates.ENTER,
            CreatePinScreenStates.READY -> setupPin(pin = pin)

            CreatePinScreenStates.CONFIRM -> confirmPin(pin = pin)
            else -> {}
        }
        logDebug("onPinReady pin: $pin", TAG)
    }

    fun showCancellationMessage() = showMessage(
        DialogMessage(
            messageID = DIALOG_EXIT_PIN_CREATION,
            message = StringSource(R.string.bottom_sheet_application_confirm_pin_cancelation_message),
            title = StringSource(R.string.information),
            positiveButtonText = StringSource(R.string.yes),
            negativeButtonText = StringSource(R.string.no),
        )
    )

    private fun setupPin(pin: String) {
        this.pin = pin
        _screenStateLiveData.setValueOnMainThread(CreatePinScreenStates.CONFIRM)
        _clearPinLiveData.callOnMainThread()
    }

    private fun confirmPin(pin: String) {
        when {
            this.pin != pin -> {
                _errorMessageLiveData.setValueOnMainThread(ConfirmPinErrorsEnum.NOT_MATCH_WITH_ENTER.title)
                _screenStateLiveData.setValueOnMainThread(CreatePinScreenStates.ENTER)
                _clearPinLiveData.callOnMainThread()
            }
            else -> _screenStateLiveData.setValueOnMainThread(CreatePinScreenStates.READY)
        }
    }

}