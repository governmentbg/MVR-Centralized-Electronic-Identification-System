package com.digitall.eid.models.list

import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.common.validator.ValidationResult
import com.digitall.eid.models.common.validator.Validator
import kotlinx.parcelize.IgnoredOnParcel
import kotlinx.parcelize.Parcelize
import kotlinx.parcelize.RawValue

@Parcelize
open class CommonValidationFieldUi<T>(
    override val elementId: Int?,
    override val elementEnum: CommonListElementIdentifier,
    open val selectedValue: @RawValue T?,
    @IgnoredOnParcel
    @Transient
    open val validators: List<Validator<@RawValue T?>> = emptyList(),
    open var validationError: StringSource?,
): CommonListElementAdapterMarker {

    override fun isContentSame(other: Any?): Boolean = true

    override fun isItemSame(other: Any?): Boolean = true

    fun triggerValidation(): Boolean {
        if (validators.isEmpty()) {
            validationError = null
            return true
        }

        for (validator in validators) {
            when (val result = validator.validate(selectedValue)) {
                is ValidationResult.Success -> { /* continue */
                }

                is ValidationResult.Error -> {
                    validationError = result.message
                    return false
                }
            }
        }

        validationError = null
        return true
    }
}