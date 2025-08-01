/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.main.base

import com.digitall.eid.R
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.models.common.DialogMessage
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.ui.BaseViewModel
import com.digitall.eid.ui.fragments.base.BaseFragment.Companion.DIALOG_EXIT

abstract class BaseMainTabViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "BaseMainTabViewModelTag"
    }

    final override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        showMessage(
            DialogMessage(
                messageID = DIALOG_EXIT,
                message = StringSource(R.string.exit_application_message),
                title = StringSource(R.string.information),
                positiveButtonText = StringSource(R.string.yes),
                negativeButtonText = StringSource(R.string.no),
            )
        )
    }

}