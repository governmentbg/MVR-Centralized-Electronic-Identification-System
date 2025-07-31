package com.digitall.eid.models.journal.common.filter

import android.os.Parcelable
import com.digitall.eid.R
import com.digitall.eid.domain.models.base.TypeEnum
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonListElementIdentifier
import kotlinx.parcelize.Parcelize

@Parcelize
enum class JournalFilterElementEnumUi(
    override val type: String,
    val title: StringSource,
): CommonListElementIdentifier, TypeEnum, Parcelable {
    DATE_PICKER_START_DATE(
        "DATE_PICKER_START_DATE",
        StringSource(R.string.journals_filter_start_date_title)
    ),
    DATE_PICKER_END_DATE(
        "DATE_PICKER_END_DATE",
        StringSource(R.string.journals_filter_end_date_title)
    ),
    DIALOG_EVENT_TYPES(
        "DIALOG_EVENT_TYPES",
        StringSource(R.string.journals_filter_event_type_title)
    ),
    BUTTON(
        "BUTTON",
        StringSource("")
    );
}

@Parcelize
enum class JournalFilterButtonsEnumUi(
    override val type: String,
    val title: StringSource,
) : CommonListElementIdentifier, TypeEnum, Parcelable {
    BUTTON_SEND(
        "BUTTON_SEND",
        StringSource(R.string.empowerments_entity_filter_apply_filters_button_title)
    ),
    BUTTON_CANCEL(
        "BUTTON_CANCEL",
        StringSource(R.string.empowerments_entity_filter_clear_button_title)
    ),
}