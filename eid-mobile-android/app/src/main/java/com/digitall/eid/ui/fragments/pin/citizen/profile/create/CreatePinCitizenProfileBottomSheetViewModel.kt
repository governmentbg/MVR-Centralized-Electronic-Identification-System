package com.digitall.eid.ui.fragments.pin.citizen.profile.create

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
import kotlin.math.abs

class CreatePinCitizenProfileBottomSheetViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "CreatePinCitizenProfileBottomSheetViewModelTag"

        const val DIALOG_EXIT_PIN_CREATION = "DIALOG_EXIT_PIN_CREATION"
        private const val ACCEPTED_CONSECUTIVE_DIGITS_COUNT = 3
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

    fun showCancellationMessage() {
        if (_screenStateLiveData.value != CreatePinScreenStates.READY) {
            showMessage(
                DialogMessage(
                    messageID = DIALOG_EXIT_PIN_CREATION,
                    message = StringSource(R.string.bottom_sheet_application_confirm_pin_cancelation_message),
                    title = StringSource(R.string.information),
                    positiveButtonText = StringSource(R.string.yes),
                    negativeButtonText = StringSource(R.string.no),
                )
            )
        }
    }

    private fun setupPin(pin: String) {
        this.pin = pin

        when {
            validateEnteredPin(pin).not() ->
                _errorMessageLiveData.setValueOnMainThread(ConfirmPinErrorsEnum.HAS_REPETITION_OR_CONSECUTIVE_DIGITS.title)

            else -> {
                _screenStateLiveData.setValueOnMainThread(CreatePinScreenStates.CONFIRM)
                _clearPinLiveData.callOnMainThread()
            }
        }
    }

    private fun confirmPin(pin: String) {
        when {
            this.pin != pin -> {
                _errorMessageLiveData.setValueOnMainThread(ConfirmPinErrorsEnum.NOT_MATCH_WITH_ENTER.title)
                _clearPinLiveData.callOnMainThread()
            }

            else -> _screenStateLiveData.setValueOnMainThread(CreatePinScreenStates.READY)
        }
    }

    private fun validateEnteredPin(pin: String): Boolean {
        logDebug("validateEnteredPin pin: $pin", TAG)

        if (pin.codePoints().distinct().count().toInt() == 1) {
            return false
        }

        val isValidRepetition = isValidRepetition(pin)
        val hasConsecutiveDigits = hasConsecutiveDigits(pin)

        return (isValidRepetition || hasConsecutiveDigits).not()
    }

    private fun isValidRepetition(code: String): Boolean {
        val repetition = findRepetition(code)
        return (repetition.first == 0 || (repetition.first == 1 && repetition.second.length == 1)).not()
    }


    private fun findRepetition(code: String): Pair<Int, String> {
        if (code.isEmpty()) {
            return Pair(0, "")
        }

        val pattern = "([0-9]+)\\1+"
        val regex = Regex(pattern)
        val matcher = regex.findAll(code)
        val matches = mutableListOf<String>()

        matcher.forEach { match ->
            match.groupValues.drop(1).forEach {
                matches.add(it)
            }
        }

        var matchedString = ""
        if (matches.isNotEmpty()) {
            matchedString = matches.first()
        }

        return Pair(matches.size, matchedString)
    }

    private fun hasConsecutiveDigits(code: String): Boolean {
        code.toIntOrNull() ?: return false

        val consecutiveDigitsLists = getConsecutiveDigitsLists(code.map(Char::digitToInt))

        return consecutiveDigitsLists.any { list -> list.size > ACCEPTED_CONSECUTIVE_DIGITS_COUNT }
    }

    private fun getConsecutiveDigitsLists(digits: List<Int>): List<List<Int>> {
        return digits.fold(mutableListOf<MutableList<Int>>()) { accumulator, digit ->

            val isAccumulatorNotInitialized = accumulator.isEmpty()
            // As a consecutive digit is considered such that the absolute difference between the
            // previous and the current is 1. The previous digit is considered to be the last one
            // added to the last list in the initial one
            val isDigitNotConsecutive =
                abs((accumulator.lastOrNull()?.lastOrNull() ?: 0) - digit) != 1

            // Checks if the initial list is initialized and the current digit is consecutive. If
            // the condition is met we add that digit to the last list and if not we initialize a
            // new list and add it to the initial one
            if (isAccumulatorNotInitialized || isDigitNotConsecutive) {
                accumulator.add(mutableListOf(digit))
            } else {
                accumulator.last().add(digit)
            }

            accumulator
        }
    }

}