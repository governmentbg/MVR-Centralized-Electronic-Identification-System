/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.models.applications.details

import android.os.Parcelable
import com.digitall.eid.domain.models.base.TypeEnum
import com.digitall.eid.models.list.CommonListElementIdentifier
import kotlinx.parcelize.Parcelize

@Parcelize
enum class ApplicationDetailsElementsEnumUi(
    override val type: String,
) : CommonListElementIdentifier, TypeEnum, Parcelable {
    BUTTON_BACK("BUTTON_BACK"),
    BUTTON_PAYMENT("BUTTON_PAYMENT"),
    BUTTON_REVOKE("BUTTON_REVOKE"),
    BUTTON_RESUME("BUTTON_RESUME"),
    BUTTON_STOP("BUTTON_STOP"),
    BUTTON_CONTINUE("BUTTON_CONTINUE"),
}