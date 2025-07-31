/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.models.empowerment.common.all

import android.os.Parcelable
import androidx.annotation.ColorRes
import com.digitall.eid.R
import com.digitall.eid.domain.models.base.TypeEnum
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonListElementIdentifier
import kotlinx.parcelize.Parcelize

@Parcelize
enum class EmpowermentElementsEnumUi(
    override val type: String,
) : CommonListElementIdentifier, TypeEnum, Parcelable {
    SPINNER_SORTING_CRITERIA("SPINNER_SORTING_CRITERIA"),
    SPINNER_MENU("SPINNER_MENU"),
}

@Parcelize
enum class EmpowermentSpinnerElementsEnumUi(
    override val type: String,
    val title: StringSource,
    @param:ColorRes val colorRes: Int,
) : CommonListElementIdentifier, TypeEnum, Parcelable {
    SPINNER_COPY(
        "SPINNER_COPY",
        StringSource(R.string.empowerment_action_type_copy),
        R.color.color_0C53B2,
    ),
    SPINNER_FROM_ME_CANCEL(
        "SPINNER_FROM_ME_CANCEL",
        StringSource(R.string.empowerment_action_type_cancel_from_me),
        R.color.color_BF1212,
    ),
    SPINNER_TO_ME_CANCEL(
        "SPINNER_TO_ME_CANCEL",
        StringSource(R.string.empowerment_action_type_cancel_to_me),
        R.color.color_BF1212,
    ),
    SPINNER_FROM_ME_SIGN(
        "SPINNER_FROM_ME_SIGN",
        StringSource(R.string.empowerment_action_type_sign),
        R.color.color_0C53B2,
    ),
}