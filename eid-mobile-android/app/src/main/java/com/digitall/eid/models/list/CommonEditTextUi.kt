/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.models.list

import androidx.annotation.ColorRes
import com.digitall.eid.domain.models.base.TypeEnum
import com.digitall.eid.domain.models.common.OriginalModel
import com.digitall.eid.extensions.equalTo
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.common.validator.Validator
import kotlinx.parcelize.IgnoredOnParcel
import kotlinx.parcelize.Parcelize

@Parcelize
data class CommonEditTextUi(
    override val elementId: Int? = null,
    override val elementEnum: CommonListElementIdentifier,
    override val selectedValue: String?,
    @IgnoredOnParcel
    @Transient
    override val validators: List<Validator<String?>> = emptyList(),
    override var validationError: StringSource? = null,
    val required: Boolean,
    val question: Boolean,
    val title: StringSource,
    val minSymbols: Int = 0,
    val maxSymbols: Int = 512,
    val hasFocus: Boolean = false,
    val isEnabled: Boolean = true,
    val hint: StringSource? = null,
    val prefix: StringSource? = null,
    @param:ColorRes val prefixTextColor: Int? = null,
    val suffix: StringSource? = null,
    @param:ColorRes val suffixTextColor: Int? = null,
    val hasEraseButton: Boolean = false,
    val type: CommonEditTextUiType,
    val originalModel: OriginalModel? = null,
) : CommonValidationFieldUi<String?>(
    elementId = elementId,
    elementEnum = elementEnum,
    selectedValue = selectedValue,
    validators = validators,
    validationError = validationError
) {

    override fun isItemSame(other: Any?): Boolean {
        return equalTo(
            other,
            { elementId },
            { elementEnum },
        )
    }

    override fun isContentSame(other: Any?): Boolean {
        return equalTo(
            other,
            { hint },
            { prefix },
            { suffix },
            { title },
            { validationError },
            { question },
            { required },
            { elementId },
            { selectedValue },
            { hasEraseButton },
            { prefixTextColor },
            { suffixTextColor }
        )
    }
}


enum class CommonEditTextUiType(
    override val type: String,
    val hint: StringSource,
    val inputType: CommonEditTextInputType
) : TypeEnum {
    EMAIL("EMAIL", StringSource(""), CommonEditTextInputType.EMAIL),
    PASSWORD("PASSWORD", StringSource(""), CommonEditTextInputType.PASSWORD),
    PASSWORD_NUMBERS(
        "PASSWORD_NUMBERS",
        StringSource(""),
        CommonEditTextInputType.PASSWORD_NUMBERS
    ),
    PHONE_NUMBER(
        "PHONE_NUMBER",
        StringSource(""),
        CommonEditTextInputType.PHONE
    ),
    NUMBERS("NUMBERS", StringSource(""), CommonEditTextInputType.DIGITS),
    TEXT_INPUT("INPUT", StringSource(""), CommonEditTextInputType.CHARS),
    TEXT_INPUT_CAP(
        "TEXT_INPUT_CAP",
        StringSource(""),
        CommonEditTextInputType.CHARS_CAP
    ),
    TEXT_INPUT_CAP_ALL(
        "TEXT_INPUT_CAP_ALL",
        StringSource(""),
        CommonEditTextInputType.CHARS_ALL_CAPS
    )
}

enum class CommonEditTextInputType(
    override val type: String,
) : TypeEnum {
    CHARS("CHARS"),
    DIGITS("DIGITS"),
    CHARS_CAP("CHARS_CAP"),
    CHARS_ALL_CAPS("CHARS_ALL_CAPS"),
    PHONE("PHONE"),
    PASSWORD("PASSWORD"),
    PASSWORD_NUMBERS("PASSWORD_NUMBERS"),
    EMAIL("EMAIL")
}