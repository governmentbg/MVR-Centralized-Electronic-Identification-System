package com.digitall.eid.mappers.citizen.change.password

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.models.citizen.change.password.ChangeCitizenPasswordAdapterMarker
import com.digitall.eid.models.citizen.change.password.ChangeCitizenPasswordElementsEnumUi
import com.digitall.eid.models.common.ButtonColorUi
import com.digitall.eid.models.common.validator.FieldsMatchValidator
import com.digitall.eid.models.common.validator.NonEmptyEditTextValidator
import com.digitall.eid.models.common.validator.PasswordValidator
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonEditTextUi
import com.digitall.eid.models.list.CommonEditTextUiType

class ChangeCitizenPasswordUiMapper: BaseMapper<Unit, List<ChangeCitizenPasswordAdapterMarker>>() {

    override fun map(from: Unit): List<ChangeCitizenPasswordAdapterMarker> {
        return buildList {
            add(
                CommonEditTextUi(
                    elementEnum = ChangeCitizenPasswordElementsEnumUi.EDIT_TEXT_OLD_PASSWORD,
                    required = true,
                    question = false,
                    title = ChangeCitizenPasswordElementsEnumUi.EDIT_TEXT_OLD_PASSWORD.title,
                    selectedValue = null,
                    type = CommonEditTextUiType.PASSWORD,
                    validators = listOf(
                        NonEmptyEditTextValidator(),
                    )
                )
            )
            val passwordField = CommonEditTextUi(
                elementEnum = ChangeCitizenPasswordElementsEnumUi.EDIT_TEXT_NEW_PASSWORD,
                required = true,
                question = false,
                title = ChangeCitizenPasswordElementsEnumUi.EDIT_TEXT_NEW_PASSWORD.title,
                selectedValue = null,
                type = CommonEditTextUiType.PASSWORD,
                validators = listOf(
                    NonEmptyEditTextValidator(),
                    PasswordValidator(),
                )
            )
            add(passwordField)
            add(
                CommonEditTextUi(
                    elementEnum = ChangeCitizenPasswordElementsEnumUi.EDIT_TEXT_CONFIRM_NEW_PASSWORD,
                    required = true,
                    question = false,
                    title = ChangeCitizenPasswordElementsEnumUi.EDIT_TEXT_CONFIRM_NEW_PASSWORD.title,
                    selectedValue = null,
                    type = CommonEditTextUiType.PASSWORD,
                    validators = listOf(
                        NonEmptyEditTextValidator(),
                        PasswordValidator(),
                        FieldsMatchValidator(originalFieldTextProvider = { passwordField.selectedValue })
                    )
                )
            )
            add(
                CommonButtonUi(
                    elementEnum = ChangeCitizenPasswordElementsEnumUi.BUTTON_CONFIRM,
                    title = ChangeCitizenPasswordElementsEnumUi.BUTTON_CONFIRM.title,
                    buttonColor = ButtonColorUi.BLUE,
                )
            )
        }
    }
}