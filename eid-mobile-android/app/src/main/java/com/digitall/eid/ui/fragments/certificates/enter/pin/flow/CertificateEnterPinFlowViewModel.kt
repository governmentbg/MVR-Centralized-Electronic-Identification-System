package com.digitall.eid.ui.fragments.certificates.enter.pin.flow

import android.content.Context
import com.digitall.eid.R
import com.digitall.eid.models.common.StartDestination
import com.digitall.eid.ui.fragments.base.flow.BaseFlowViewModel

class CertificateEnterPinFlowViewModel : BaseFlowViewModel() {

    companion object {
        private const val TAG = "CertificateEnterPinFlowViewModelTag"
    }

    override val isAuthorizationActive: Boolean = false

    fun getStartDestination(context: Context): StartDestination = StartDestination(R.id.certificateEnterPinFragment)

}