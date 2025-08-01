package com.digitall.eid.ui.fragments.common.search.multiselect

import androidx.lifecycle.viewModelScope
import com.digitall.eid.R
import com.digitall.eid.domain.extensions.readOnly
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.extensions.launchWithDispatcher
import com.digitall.eid.extensions.readOnly
import com.digitall.eid.extensions.setValueOnMainThread
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonDialogWithSearchAdapterMarker
import com.digitall.eid.models.list.CommonDialogWithSearchItemUi
import com.digitall.eid.models.list.CommonDialogWithSearchMultiselectItemUi
import com.digitall.eid.models.list.CommonDialogWithSearchMultiselectUi
import com.digitall.eid.ui.BaseViewModel
import com.digitall.eid.utils.CurrentContext
import com.digitall.eid.utils.SingleLiveEvent
import kotlinx.coroutines.Job
import kotlinx.coroutines.flow.MutableStateFlow
import org.koin.core.component.inject

class CommonBottomSheetWithSearchMultiselectViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "CommonBottomSheetWithSearchMultiselectViewModelTag"
    }

    private val currentContext: CurrentContext by inject()

    private var list = mutableListOf<CommonDialogWithSearchMultiselectItemUi>()

    private var currentModel: CommonDialogWithSearchMultiselectUi? = null

    private var searchJob: Job? = null

    private val _adapterList =
        MutableStateFlow<List<CommonDialogWithSearchAdapterMarker>>(emptyList())
    val adapterList = _adapterList.readOnly()

    private val _resultModel = SingleLiveEvent<CommonDialogWithSearchMultiselectUi>()
    val resultModel = _resultModel.readOnly()

    fun setModel(model: CommonDialogWithSearchMultiselectUi) {
        viewModelScope.launchWithDispatcher {
            currentModel = model
            list = model.list.toMutableList()
            _adapterList.emit(list)
        }
    }

    fun onChecked(selected: CommonDialogWithSearchMultiselectItemUi) {
        viewModelScope.launchWithDispatcher {
            when {
                selected.isSelectAllOption -> {
                    list = list.map { element -> element.copy(isSelected = selected.isSelected) }
                        .toMutableList()
                }

                else -> {
                    list = list.map { element ->
                        when {
                            element.isSelectAllOption -> element.copy(
                                isSelected = false
                            )

                            element.elementId == selected.elementId -> element.copy(isSelected = selected.isSelected)

                            else -> element
                        }
                    }.toMutableList()
                }
            }

            currentModel = currentModel?.copy(
                selectedValue = list.filter { element -> element.isSelected && element.isSelectAllOption.not() },
                list = list
            )
            _adapterList.emit(list)
        }
    }


    fun onDone() {
        _resultModel.setValueOnMainThread(currentModel)
    }

    fun onSearch(text: String?) {
        searchJob?.cancel()
        searchJob = viewModelScope.launchWithDispatcher {
            if (text.isNullOrEmpty()) {
                _adapterList.emit(list)
            } else {
                if (currentModel?.customInputEnabled == true) {
                    val filtered = buildList {
                        add(
                            CommonDialogWithSearchMultiselectItemUi(
                                serverValue = text,
                                text = StringSource(text),
                            )
                        )
                        list.filter {
                            it.text.getString(currentContext.get())
                                .contains(text, true) && it.isSelectAllOption.not()
                        }.let {
                            addAll(it)
                        }

                    }
                    _adapterList.emit(filtered)
                } else {
                    val filtered =
                        list.filter {
                            it.text.getString(currentContext.get())
                                .contains(text, true) && it.isSelectAllOption.not()
                        }.takeIf { items -> items.isNotEmpty() } ?: listOf(
                            CommonDialogWithSearchItemUi(
                                text = StringSource(R.string.no_search_results),
                                selectable = false
                            )
                        )

                    _adapterList.emit(filtered)
                }
            }
        }
    }

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        popBackStack()
    }

}