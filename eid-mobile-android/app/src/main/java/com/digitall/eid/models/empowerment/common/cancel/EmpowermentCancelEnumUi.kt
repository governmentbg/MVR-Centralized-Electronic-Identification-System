/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.models.empowerment.common.cancel

import android.os.Parcelable
import com.digitall.eid.R
import com.digitall.eid.domain.models.base.TypeEnum
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonListElementIdentifier
import kotlinx.parcelize.Parcelize

@Parcelize
enum class EmpowermentCancelEnumUi(
    override val type: String,
    val title: StringSource,
) : CommonListElementIdentifier, TypeEnum, Parcelable {
    SPINNER_REASON("SPINNER_REASON", StringSource(R.string.empowerment_cancel_reason_title)),
    SPINNER_REASON_OTHER("SPINNER_REASON_OTHER", StringSource("Друго")),
    EDIT_TEXT_REASON("EDIT_TEXT_REASON", StringSource("Описание")),
    BUTTON_APPLY("BUTTON_APPLY", StringSource("")),
    BUTTON_BACK("BUTTON_BACK", StringSource(R.string.back)),
}