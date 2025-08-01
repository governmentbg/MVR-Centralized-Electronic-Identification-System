/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.models.certificates.filter

import com.digitall.eid.R
import com.digitall.eid.domain.models.base.TypeEnum
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonListElementIdentifier
import kotlinx.parcelize.Parcelize

@Parcelize
enum class CertificatesFilterElementsEnumUi (
    override val type: String,
    val title: StringSource,
) : CommonListElementIdentifier, TypeEnum {
    EDIT_TEXT_SERIAL_NUMBER("EDIT_TEXT_SERIAL_NUMBER", StringSource(R.string.certificates_filter_serial_number_title)),
    EDIT_TEXT_ALIAS("EDIT_TEXT_ALIAS", StringSource(R.string.certificates_filter_alias_title)),
    DATE_PICKER_ISSUED_ON("DATE_PICKER_ISSUED_ON", StringSource(R.string.certificates_filter_issued_on_title)),
    DATE_PICKER_VALID_UNTIL("DATE_PICKER_VALID_UNTIL", StringSource(R.string.certificates_filter_valid_until_title)),
    SPINNER_STATUS("SPINNER_STATUS", StringSource(R.string.certificates_filter_status_title)),
    DIALOG_ADMINISTRATOR("DIALOG_ADMINISTRATOR", StringSource(R.string.certificates_filter_administrator_title)),
    SPINNER_DEVICE_TYPE("SPINNER_DEVICE_TYPE", StringSource(R.string.certificates_filter_carrier_title)),
    BUTTONS("BUTTONS", StringSource("")),
}

@Parcelize
enum class CertificatesFilterButtonsEnumUi(
    override val type: String,
    val title: StringSource,
) : CommonListElementIdentifier, TypeEnum {
    BUTTON_SEND("BUTTON_SEND", StringSource(R.string.certificates_filter_apply_filter_button_title)),
    BUTTON_CANCEL("BUTTON_CANCEL", StringSource(R.string.certificates_filter_clear_button_title)),
}