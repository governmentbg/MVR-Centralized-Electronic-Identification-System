/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.models.applications.all

import android.os.Parcelable
import androidx.annotation.ColorRes
import com.digitall.eid.R
import com.digitall.eid.domain.models.base.TypeEnum
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonListElementIdentifier
import kotlinx.parcelize.Parcelize

@Parcelize
enum class ApplicationsElementsEnumUi(
    override val type: String,
) : TypeEnum, CommonListElementIdentifier, Parcelable {
    SPINNER_SORTING_CRITERIA("SPINNER_SORTING_CRITERIA"),
    SPINNER_MENU("SPINNER_MENU"),
}

@Parcelize
enum class ApplicationsSpinnerElementsEnumUi(
    override val type: String,
    val title: StringSource,
    @param:ColorRes val colorRes: Int,
) : CommonListElementIdentifier, TypeEnum, Parcelable {
    SPINNER_PAYMENT(
        "SPINNER_PAYMENT",
        StringSource(R.string.application_payment),
        R.color.color_1C3050,
    ),
}