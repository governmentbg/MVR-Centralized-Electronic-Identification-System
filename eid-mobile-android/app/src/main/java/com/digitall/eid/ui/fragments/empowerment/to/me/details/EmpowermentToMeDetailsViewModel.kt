/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.empowerment.to.me.details

import androidx.lifecycle.viewModelScope
import com.digitall.eid.R
import com.digitall.eid.domain.models.empowerment.common.all.EmpowermentItem
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.extensions.launchWithDispatcher
import com.digitall.eid.mappers.empowerment.to.me.details.EmpowermentToMeDetailsUiMapper
import com.digitall.eid.ui.fragments.empowerment.base.details.BaseEmpowermentDetailsViewModel
import org.koin.core.component.inject

class EmpowermentToMeDetailsViewModel : BaseEmpowermentDetailsViewModel() {

    companion object {
        private const val TAG = "EmpowermentToMeDetailsViewModelTag"
    }

    private val empowermentToMeDetailsUiMapper: EmpowermentToMeDetailsUiMapper by inject()

    private var model: EmpowermentItem? = null

    override fun setupModel(model: EmpowermentItem) {
        logDebug("setupModel", TAG)
        this.model = model
        viewModelScope.launchWithDispatcher {
            _adapterListLiveData.emit(
                empowermentToMeDetailsUiMapper.map(
                    from = model,
                )
            )
        }
    }

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        popBackStackToFragment(R.id.empowermentToMeFragment)
    }

}