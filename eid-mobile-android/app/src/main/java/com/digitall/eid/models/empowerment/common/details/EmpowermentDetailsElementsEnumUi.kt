/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.models.empowerment.common.details

import android.os.Parcelable
import com.digitall.eid.domain.models.base.TypeEnum
import com.digitall.eid.models.list.CommonListElementIdentifier
import kotlinx.parcelize.Parcelize

@Parcelize
enum class EmpowermentDetailsElementsEnumUi(
    override val type: String,
) : CommonListElementIdentifier, TypeEnum, Parcelable {
    BUTTON_BACK("BUTTON_BACK"),
    SPINNER_MENU("SPINNER_MENU"),
}