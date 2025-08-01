/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.models.list

import com.digitall.eid.domain.UiDateFormats
import com.digitall.eid.domain.extensions.getString
import com.digitall.eid.domain.models.common.OriginalModel
import com.digitall.eid.extensions.equalTo
import com.digitall.eid.models.common.DiffEquals
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.common.validator.Validator
import kotlinx.parcelize.IgnoredOnParcel
import kotlinx.parcelize.Parcelize
import java.util.Calendar

sealed interface CommonDialogWithSearchAdapterMarker : DiffEquals

@Parcelize
data class CommonDatePickerUi(
    override val elementId: Int? = null,
    override val elementEnum: CommonListElementIdentifier,
    override val selectedValue: Calendar?,
    @IgnoredOnParcel
    @Transient
    override val validators: List<Validator<Calendar?>> = emptyList(),
    override var validationError: StringSource? = null,
    val minDate: Calendar,
    val maxDate: Calendar,
    val required: Boolean,
    val question: Boolean,
    val title: StringSource,
    val hasEraseButton: Boolean = false,
    val originalModel: OriginalModel? = null,
    val dateFormat: UiDateFormats,
    val maxLinesTitle: Int = 2,
    val maxLinesError: Int = 2,
) : CommonValidationFieldUi<Calendar?>(
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
            { title },
            { validationError },
            { maxDate },
            { minDate },
            { required },
            { question },
            { elementId },
            { selectedValue },
            { elementEnum },
            { maxLinesTitle },
            { maxLinesError },
            { hasEraseButton },
        )
    }

    override fun toString(): String {
        return "\nselectedValue: ${selectedValue?.getString()}\nminDate: ${minDate.getString()}\nmaxDate: ${maxDate.getString()}\n"
    }
}