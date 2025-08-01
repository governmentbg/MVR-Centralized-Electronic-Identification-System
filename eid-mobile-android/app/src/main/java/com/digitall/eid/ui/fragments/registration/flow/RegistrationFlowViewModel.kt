package com.digitall.eid.ui.fragments.registration.flow

import android.content.Context
import com.digitall.eid.R
import com.digitall.eid.models.common.StartDestination
import com.digitall.eid.ui.fragments.base.flow.BaseFlowViewModel

class RegistrationFlowViewModel: BaseFlowViewModel() {

    companion object {
        private const val TAG = "RegistrationFlowViewModelTag"
    }

    fun getStartDestination(context: Context): StartDestination = StartDestination(R.id.authStartFragment)
}