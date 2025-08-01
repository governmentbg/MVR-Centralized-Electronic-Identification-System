package com.digitall.eid.ui.fragments.certificates.change.pin.flow

import com.digitall.eid.R
import com.digitall.eid.models.common.StartDestination
import com.digitall.eid.ui.fragments.base.flow.BaseFlowViewModel

class CertificateChangePinFlowViewModel : BaseFlowViewModel() {

    companion object {
        private const val TAG = "CardChangePinFlowViewModelTag"
    }

    fun getStartDestination() = StartDestination(R.id.cardChangePinFragment)

}