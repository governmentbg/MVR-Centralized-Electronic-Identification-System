/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.empowerment.base.details

import com.digitall.eid.domain.extensions.readOnly
import com.digitall.eid.domain.models.empowerment.common.all.EmpowermentItem
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.models.empowerment.common.details.EmpowermentDetailsAdapterMarker
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.ui.BaseViewModel
import kotlinx.coroutines.flow.MutableStateFlow

abstract class BaseEmpowermentDetailsViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "BaseEmpowermentDetailsViewModelTag"
    }

    protected val _adapterListLiveData =
        MutableStateFlow<List<EmpowermentDetailsAdapterMarker>>(emptyList())
    val adapterListLiveData = _adapterListLiveData.readOnly()

    abstract fun setupModel(model: EmpowermentItem)

    fun refreshScreen() {
        logDebug("refreshScreen", TAG)

    }

    fun onButtonClicked(model: CommonButtonUi) {
        logDebug("onButtonClicked", TAG)
        onBackPressed()
    }

}