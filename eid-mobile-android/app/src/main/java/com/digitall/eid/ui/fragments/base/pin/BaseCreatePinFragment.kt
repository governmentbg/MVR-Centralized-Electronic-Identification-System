/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.base.pin

import androidx.annotation.CallSuper
import androidx.viewbinding.ViewBinding
import com.digitall.eid.models.common.CreatePinScreenStates
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.ui.fragments.base.BaseFragment

abstract class BaseCreatePinFragment<VB : ViewBinding, VM : BaseCreatePinViewModel> :
    BaseFragment<VB, VM>() {

    companion object {
        private const val TAG = "BaseCreatePinFragmentTag"
    }

    protected abstract fun setEnterPinState()

    protected abstract fun setConfirmPinState()

    protected abstract fun setReadyPinState()

    protected abstract fun clearPin()

    protected abstract fun setPinErrorText(error: StringSource?)

    @CallSuper
    override fun subscribeToLiveData() {
        viewModel.screenStateLiveData.observe(viewLifecycleOwner) {
            when (it) {
                CreatePinScreenStates.ENTER -> {
                    setEnterPinState()
                }

                CreatePinScreenStates.CONFIRM -> {
                    setConfirmPinState()
                }

                CreatePinScreenStates.READY -> {
                    setReadyPinState()
                }

                else -> {}
            }
        }
        viewModel.errorMessageLiveData.observe(viewLifecycleOwner) {
            setPinErrorText(it)
        }
        viewModel.clearPinLiveData.observe(viewLifecycleOwner) {
            clearPin()
        }
    }


}