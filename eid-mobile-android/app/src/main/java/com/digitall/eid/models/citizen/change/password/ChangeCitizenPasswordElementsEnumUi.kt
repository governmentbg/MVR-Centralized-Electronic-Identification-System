package com.digitall.eid.models.citizen.change.password

import android.os.Parcelable
import com.digitall.eid.R
import com.digitall.eid.domain.models.base.TypeEnum
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonListElementIdentifier
import kotlinx.parcelize.Parcelize

@Parcelize
enum class ChangeCitizenPasswordElementsEnumUi(override val type: String, val title: StringSource) :
    CommonListElementIdentifier, TypeEnum, Parcelable {
    EDIT_TEXT_OLD_PASSWORD("EDIT_TEXT_OLD_PASSWORD", StringSource(R.string.change_user_password_input_old_password_title)),
    EDIT_TEXT_NEW_PASSWORD("EDIT_TEXT_NEW_PASSWORD", StringSource(R.string.change_user_password_input_new_password_title)),
    EDIT_TEXT_CONFIRM_NEW_PASSWORD("EDIT_TEXT_CONFIRM_NEW_PASSWORD", StringSource(R.string.change_user_password_input_confirm_new_password_title)),
    BUTTON_CONFIRM("BUTTON_CONFIRM", StringSource(R.string.change_user_password_confirm_button_title))
}