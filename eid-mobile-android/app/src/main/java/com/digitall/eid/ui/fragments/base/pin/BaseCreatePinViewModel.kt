/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.base.pin

import androidx.lifecycle.MutableLiveData
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.extensions.readOnly
import com.digitall.eid.extensions.setValueOnMainThread
import com.digitall.eid.models.base.pin.create.ConfirmPinErrorsEnum
import com.digitall.eid.models.common.CreatePinScreenStates
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.ui.BaseViewModel
import com.digitall.eid.utils.SingleLiveEvent

abstract class BaseCreatePinViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "BaseCreatePinViewModelTag"
    }

    protected var pin: String? = null

    private val _screenStateLiveData = MutableLiveData(CreatePinScreenStates.ENTER)
    val screenStateLiveData = _screenStateLiveData.readOnly()

    private val _errorMessageLiveData = SingleLiveEvent<StringSource?>()
    val errorMessageLiveData = _errorMessageLiveData.readOnly()

    private val _clearPinLiveData = SingleLiveEvent<Unit>()
    val clearPinLiveData = _clearPinLiveData.readOnly()

    fun onPinStartEnter() {
        _errorMessageLiveData.setValueOnMainThread(null)
    }

    fun onPinEntered(pin: String) {
        when (screenStateLiveData.value) {
            CreatePinScreenStates.ENTER,
            CreatePinScreenStates.READY-> setupPin(pin)
            CreatePinScreenStates.CONFIRM -> confirmPin(pin)
            else -> {}
        }
        logDebug("onPinReady pin: $pin", TAG)
    }

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