package com.digitall.eid.models.list

import androidx.annotation.ColorRes
import com.digitall.eid.domain.models.common.OriginalModel
import com.digitall.eid.extensions.equalTo
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.common.validator.Validator
import kotlinx.parcelize.IgnoredOnParcel
import kotlinx.parcelize.Parcelize

@Parcelize
data class CommonPhoneTextUi(
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
    val countryCode: StringSource? = null,
    @param:ColorRes val countryCodeTextColor: Int? = null,
    val originalModel: OriginalModel? = null,
) : CommonValidationFieldUi<String?>(
    elementId = elementId,
    elementEnum = elementEnum,
    selectedValue = selectedValue,
    validators = validators,
    validationError = validationError
)  {

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
            { countryCode },
            { title },
            { validationError },
            { question },
            { required },
            { elementId },
            { selectedValue },
            { countryCodeTextColor },
        )
    }
}
