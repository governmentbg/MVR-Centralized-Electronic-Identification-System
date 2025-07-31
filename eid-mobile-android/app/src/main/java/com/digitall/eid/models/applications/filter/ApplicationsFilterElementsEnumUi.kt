/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.models.applications.filter

import android.os.Parcelable
import com.digitall.eid.R
import com.digitall.eid.domain.models.base.TypeEnum
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonListElementIdentifier
import kotlinx.parcelize.Parcelize

@Parcelize
enum class ApplicationsFilterElementsEnumUi(
    override val type: String,
    val title: StringSource,
) : TypeEnum, CommonListElementIdentifier, Parcelable {
    SPINNER_STATUS("SPINNER_STATUS", StringSource(R.string.applications_filter_status_title)),
    EDIT_TEXT_IDENTIFIER("EDIT_TEXT_IDENTIFIER", StringSource(R.string.applications_filter_identifier_title)),
    EDIT_TEXT_APPLICATION_NUMBER("EDIT_TEXT_APPLICATION_NUMBER", StringSource(R.string.applications_filter_application_number_title)),
    DATE_PICKER_CREATION_DATE("DATE_PICKER_CREATION_DATE", StringSource(R.string.applications_filter_creation_date_title)),
    DIALOG_ADMINISTRATOR("DIALOG_ADMINISTRATOR", StringSource(R.string.applications_filter_administrator_title)),
    SPINNER_DEVICE_TYPE("SPINNER_DEVICE_TYPE", StringSource(R.string.applications_filter_carrier_title)),
    SPINNER_SUBJECT("SPINNER_SUBJECT", StringSource(R.string.applications_filter_subject_title)),
    BUTTONS("BUTTONS", StringSource("")),
}

@Parcelize
enum class ApplicationsFilterButtonsEnumUi(
    override val type: String,
    val title: StringSource,
) : CommonListElementIdentifier, TypeEnum, Parcelable {
    BUTTON_SEND("BUTTON_SEND", StringSource(R.string.applications_filter_apply_filter_button_title)),
    BUTTON_CANCEL("BUTTON_CANCEL", StringSource(R.string.applications_filter_clear_button_title)),
}