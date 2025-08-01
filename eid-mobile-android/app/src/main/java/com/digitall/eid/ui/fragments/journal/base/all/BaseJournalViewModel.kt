/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.journal.base.all

import androidx.lifecycle.LiveData
import androidx.lifecycle.asLiveData
import androidx.lifecycle.viewModelScope
import androidx.paging.PagingData
import com.digitall.eid.domain.models.journal.filter.JournalFilterModel
import com.digitall.eid.extensions.readOnly
import com.digitall.eid.extensions.setValueOnMainThread
import com.digitall.eid.models.journal.common.all.JournalAdapterMarker
import com.digitall.eid.ui.BaseViewModel
import com.digitall.eid.utils.SingleLiveEvent
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.launch

abstract class BaseJournalViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "BaseJournalViewModelTag"
    }

    abstract fun toFilter()

    abstract val logsPagingDataFlow: Flow<PagingData<JournalAdapterMarker>>

    lateinit var adapterListLiveData: LiveData<PagingData<JournalAdapterMarker>>

    private val _isFilterInitEvent = SingleLiveEvent<Boolean>()
    val isFilterInitEvent = _isFilterInitEvent.readOnly()

    protected var filteringModel = MutableStateFlow(
        JournalFilterModel(
            startDate = null,
            endDate = null,
            eventTypes = emptyList(),
        )
    )

    init {
        viewModelScope.launch {
            filteringModel.collect { filter ->
                val isFilterActive = filter.allPropertiesAreNull.not()
                if (_isFilterInitEvent.value != isFilterActive) {
                    _isFilterInitEvent.postValue(isFilterActive)
                }
            }
        }
    }

    override fun onCreated() {
        super.onCreated()
        adapterListLiveData = logsPagingDataFlow.asLiveData()
    }

    fun checkFilteringModelState() = _isFilterInitEvent.setValueOnMainThread(filteringModel.value.allPropertiesAreNull.not())

    fun updateFilteringModel(filterModel: JournalFilterModel) {
        filteringModel.value = filterModel
    }
}