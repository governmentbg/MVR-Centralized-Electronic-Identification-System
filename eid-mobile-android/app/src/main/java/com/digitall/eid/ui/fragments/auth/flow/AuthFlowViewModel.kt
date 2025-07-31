/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.auth.flow

import android.content.Context
import com.digitall.eid.R
import com.digitall.eid.models.common.StartDestination
import com.digitall.eid.ui.fragments.base.flow.BaseFlowViewModel

class AuthFlowViewModel : BaseFlowViewModel() {

    companion object {
        private const val TAG = "AuthEnterFlowViewModelTag"
    }

    override val isAuthorizationActive: Boolean = false

    fun getStartDestination(context: Context): StartDestination = StartDestination(R.id.authStartFragment)

}