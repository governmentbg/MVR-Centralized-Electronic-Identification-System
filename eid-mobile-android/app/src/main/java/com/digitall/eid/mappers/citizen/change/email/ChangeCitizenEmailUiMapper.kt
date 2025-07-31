package com.digitall.eid.mappers.citizen.change.email

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.models.citizen.change.email.ChangeCitizenEmailAdapterMarker
import com.digitall.eid.models.citizen.change.email.ChangeCitizenEmailElementsEnumUi
import com.digitall.eid.models.common.ButtonColorUi
import com.digitall.eid.models.common.validator.EmailValidator
import com.digitall.eid.models.common.validator.NonEmptyEditTextValidator
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonEditTextUi
import com.digitall.eid.models.list.CommonEditTextUiType

class ChangeCitizenEmailUiMapper: BaseMapper<Unit, List<ChangeCitizenEmailAdapterMarker>>() {

    override fun map(from: Unit): List<ChangeCitizenEmailAdapterMarker> {
        return buildList {
            add(
                CommonEditTextUi(
                    elementEnum = ChangeCitizenEmailElementsEnumUi.EDIT_TEXT_EMAIL,
                    required = true,
                    question = false,
                    title = ChangeCitizenEmailElementsEnumUi.EDIT_TEXT_EMAIL.title,
                    type = CommonEditTextUiType.EMAIL,
                    selectedValue = null,
                    validators = listOf(
                        NonEmptyEditTextValidator(),
                        EmailValidator()
                    )
                )
            )
            add(
                CommonButtonUi(
                    elementEnum = ChangeCitizenEmailElementsEnumUi.BUTTON_CONFIRM,
                    title = ChangeCitizenEmailElementsEnumUi.BUTTON_CONFIRM.title,
                    buttonColor = ButtonColorUi.BLUE,
                )
            )
        }
    }
}