package com.digitall.eid.ui.fragments.journal.to.me.filter

import com.digitall.eid.R
import com.digitall.eid.domain.UiDateFormats
import com.digitall.eid.domain.extensions.fromServerDate
import com.digitall.eid.domain.extensions.getCalendar
import com.digitall.eid.domain.models.assets.localization.logs.LogLocalizationModel
import com.digitall.eid.domain.models.journal.filter.JournalFilterModel
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.models.common.ButtonColorUi
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.journal.common.filter.JournalFilterAdapterMarker
import com.digitall.eid.models.journal.common.filter.JournalFilterButtonsEnumUi
import com.digitall.eid.models.journal.common.filter.JournalFilterElementEnumUi
import com.digitall.eid.models.list.CommonDatePickerUi
import com.digitall.eid.models.list.CommonDialogWithSearchMultiselectItemUi
import com.digitall.eid.models.list.CommonDialogWithSearchMultiselectUi
import com.digitall.eid.models.list.CommonDoubleButtonItem
import com.digitall.eid.models.list.CommonDoubleButtonUi
import com.digitall.eid.ui.fragments.journal.base.filter.BaseJournalFilterViewModel

class JournalToMeFilterViewModel : BaseJournalFilterViewModel() {

    companion object {
        private const val TAG = "JournalToMeFilterViewModelTag"
    }

    override fun getStartScreenElements(
        model: JournalFilterModel,
        localizations: Array<LogLocalizationModel>
    ): List<JournalFilterAdapterMarker> {
        logDebug("getStartScreenElements", TAG)
        return buildList {
            val hasValidStartDate = model.startDate.isNullOrEmpty().not()
            val hasValidEndDate = model.endDate.isNullOrEmpty().not()
            add(
                CommonDialogWithSearchMultiselectUi(
                    required = true,
                    question = false,
                    selectedValue = model.eventTypes.map {
                        CommonDialogWithSearchMultiselectItemUi(
                            serverValue = it,
                            text = StringSource(localizations.firstOrNull { localization -> localization.type == it }?.description),
                            elementEnum = JournalFilterElementEnumUi.DIALOG_EVENT_TYPES,
                        )
                    },
                    title = JournalFilterElementEnumUi.DIALOG_EVENT_TYPES.title,
                    elementEnum = JournalFilterElementEnumUi.DIALOG_EVENT_TYPES,
                    list = localizations.mapIndexed { index, localization ->
                        CommonDialogWithSearchMultiselectItemUi(
                            elementId = index,
                            text = StringSource(localization.description),
                            serverValue = localization.type,
                            isSelected = model.eventTypes.contains(localization.type),
                            elementEnum = JournalFilterElementEnumUi.DIALOG_EVENT_TYPES,
                        )
                    }.toMutableList().takeIf { list -> list.isNotEmpty() }?.apply {
                        add(
                            0, CommonDialogWithSearchMultiselectItemUi(
                                text = StringSource(R.string.select_all),
                                isSelectAllOption = true,
                                isSelected = model.allEventTypesSelected,
                            )
                        )
                    } ?: listOf(
                        CommonDialogWithSearchMultiselectItemUi(
                            text = StringSource(R.string.no_search_results),
                            selectable = false
                        )
                    ),
                )
            )
            add(
                CommonDatePickerUi(
                    required = false,
                    question = false,
                    title = JournalFilterElementEnumUi.DATE_PICKER_START_DATE.title,
                    elementEnum = JournalFilterElementEnumUi.DATE_PICKER_START_DATE,
                    selectedValue = if (hasValidStartDate) model.startDate?.fromServerDate()
                    else null,
                    minDate = getCalendar(minusYears = 100),
                    maxDate = if (hasValidEndDate) model.endDate?.fromServerDate() ?: getCalendar()
                    else getCalendar(),
                    dateFormat = UiDateFormats.WITHOUT_TIME,
                ),
            )
            add(
                CommonDatePickerUi(
                    required = false,
                    question = false,
                    title = JournalFilterElementEnumUi.DATE_PICKER_END_DATE.title,
                    elementEnum = JournalFilterElementEnumUi.DATE_PICKER_END_DATE,
                    selectedValue = if (hasValidEndDate) model.endDate?.fromServerDate()
                    else null,
                    minDate = if (hasValidStartDate) model.startDate?.fromServerDate()
                        ?: getCalendar(minusYears = 100)
                    else getCalendar(minusYears = 100),
                    maxDate = getCalendar(),
                    dateFormat = UiDateFormats.WITHOUT_TIME,
                ),
            )
        }
    }

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        popBackStackToFragment(R.id.journalToMeFragment)
    }
}