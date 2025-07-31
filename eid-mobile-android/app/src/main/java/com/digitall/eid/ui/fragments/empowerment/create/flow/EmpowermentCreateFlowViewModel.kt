/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.empowerment.create.flow

import androidx.core.os.bundleOf
import com.digitall.eid.R
import com.digitall.eid.domain.models.empowerment.common.all.EmpowermentItem
import com.digitall.eid.models.common.StartDestination
import com.digitall.eid.ui.fragments.base.flow.BaseFlowViewModel

class EmpowermentCreateFlowViewModel : BaseFlowViewModel() {

    companion object {
        private const val TAG = "EmpowermentCreateFlowViewModelTag"
    }

    fun getStartDestination(model: EmpowermentItem?): StartDestination {
        return StartDestination(
            destination = R.id.empowermentCreateFragment,
            arguments = bundleOf(
                "model" to model
            )
        )
    }

}