package com.digitall.eid.ui.fragments.pin.citizen.profile.enter

import androidx.lifecycle.viewModelScope
import com.digitall.eid.extensions.launchWithDispatcher
import com.digitall.eid.extensions.readOnly
import com.digitall.eid.ui.BaseViewModel
import com.digitall.eid.utils.AuthenticationManager
import com.digitall.eid.utils.SingleLiveEvent
import org.koin.core.component.inject

class EnterPinCitizenProfileBottomSheetViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "EnterPinCitizenProfileBottomSheetViewModelTag"
    }

    private val authenticationManager: AuthenticationManager by inject()

    private var pin: String? = null

    val authenticationResultEvent = authenticationManager.authenticationResultEvent

    private val _readyStateEvent = SingleLiveEvent<Unit>()
    val readyStateEvent = _readyStateEvent.readOnly()

    override fun onBackPressed() {}

    fun onPinEntered(pin: String) {
        this.pin = pin
        _readyStateEvent.callOnMainThread()
    }

    fun verifyPin() { confirmPin(pin = pin ?: return) }

    private fun confirmPin(pin: String) {
        viewModelScope.launchWithDispatcher {
            authenticationManager.verifyApplicationPin(enteredPin = pin)
        }
    }
}