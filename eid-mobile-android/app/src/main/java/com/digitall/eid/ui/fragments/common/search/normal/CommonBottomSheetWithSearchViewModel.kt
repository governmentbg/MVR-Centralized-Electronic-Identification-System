/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.common.search.normal

import androidx.lifecycle.viewModelScope
import com.digitall.eid.R
import com.digitall.eid.domain.extensions.readOnly
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.extensions.launchWithDispatcher
import com.digitall.eid.extensions.readOnly
import com.digitall.eid.extensions.setValueOnMainThread
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonDialogWithSearchItemUi
import com.digitall.eid.models.list.CommonDialogWithSearchUi
import com.digitall.eid.ui.BaseViewModel
import com.digitall.eid.utils.CurrentContext
import com.digitall.eid.utils.SingleLiveEvent
import kotlinx.coroutines.Job
import kotlinx.coroutines.flow.MutableStateFlow
import org.koin.core.component.inject

class CommonBottomSheetWithSearchViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "CommonBottomSheetWithSearchViewModelTag"
    }

    private val currentContext: CurrentContext by inject()

    private var list: List<CommonDialogWithSearchItemUi>? = null

    private var currentModel: CommonDialogWithSearchUi? = null

    private var searchJob: Job? = null

    private val _adapterList = MutableStateFlow<List<CommonDialogWithSearchItemUi>>(emptyList())
    val adapterList = _adapterList.readOnly()

    private val _resultModel = SingleLiveEvent<CommonDialogWithSearchUi>()
    val resultModel = _resultModel.readOnly()

    fun setModel(model: CommonDialogWithSearchUi) {
        viewModelScope.launchWithDispatcher {
            currentModel = model
            list = model.list
            _adapterList.emit(list ?: emptyList())
        }
    }

    fun onClicked(selected: CommonDialogWithSearchItemUi) {
        _resultModel.setValueOnMainThread(
            currentModel?.copy(
                selectedValue = selected
            )
        )
    }

    fun onSearch(text: String?) {
        searchJob?.cancel()
        searchJob = viewModelScope.launchWithDispatcher {
            if (text.isNullOrEmpty()) {
                _adapterList.emit(list ?: emptyList())
            } else {
                if (currentModel?.customInputEnabled == true) {
                    val filtered = buildList {
                        add(
                            CommonDialogWithSearchItemUi(
                                serverValue = text,
                                text = StringSource(text),
                            )
                        )
                        list?.filter {
                            it.text.getString(currentContext.get()).contains(text, true)
                        }?.let {
                            addAll(it)
                        }

                    }
                    _adapterList.emit(filtered)
                } else {
                    val filtered =
                        list?.filter {
                            it.text.getString(currentContext.get()).contains(text, true)
                        }?.takeIf { items -> items.isNotEmpty() } ?: listOf(
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