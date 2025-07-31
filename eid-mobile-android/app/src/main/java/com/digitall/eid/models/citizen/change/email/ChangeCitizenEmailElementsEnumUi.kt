package com.digitall.eid.models.citizen.change.email

import android.os.Parcelable
import com.digitall.eid.R
import com.digitall.eid.domain.models.base.TypeEnum
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonListElementIdentifier
import kotlinx.parcelize.Parcelize

@Parcelize
enum class ChangeCitizenEmailElementsEnumUi(override val type: String, val title: StringSource) :
    CommonListElementIdentifier, TypeEnum, Parcelable {
    EDIT_TEXT_EMAIL("EDIT_TEXT_EMAIL", StringSource(R.string.change_user_email_input_email_title)),
    BUTTON_CONFIRM("BUTTON_CONFIRM", StringSource(R.string.change_user_email_confirm_button_title)),
}