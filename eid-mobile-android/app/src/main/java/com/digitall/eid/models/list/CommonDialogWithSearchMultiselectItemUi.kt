package com.digitall.eid.models.list

import android.os.Parcelable
import com.digitall.eid.domain.models.common.OriginalModel
import com.digitall.eid.extensions.equalTo
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.common.validator.Validator
import kotlinx.parcelize.IgnoredOnParcel
import kotlinx.parcelize.Parcelize

@Parcelize
data class CommonDialogWithSearchMultiselectUi(
    override val elementId: Int? = null,
    override val elementEnum: CommonListElementIdentifier,
    override val selectedValue: List<CommonDialogWithSearchMultiselectItemUi>?,
    @IgnoredOnParcel
    @Transient
    override val validators: List<Validator<List<CommonDialogWithSearchMultiselectItemUi>?>> = emptyList(),
    override var validationError: StringSource? = null,
    val required: Boolean,
    val question: Boolean,
    val title: StringSource,
    val hasEraseButton: Boolean = false,
    val customInputEnabled: Boolean = false,
    val list: List<CommonDialogWithSearchMultiselectItemUi>,
    val maxLinesTitle: Int = 2,
    val maxLinesText: Int = 1,
    val maxLinesError: Int = 2,
) : CommonValidationFieldUi<List<CommonDialogWithSearchMultiselectItemUi>?>(
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
            { list },
            { title },
            { validationError },
            { required },
            { question },
            { elementId },
            { elementEnum },
            { maxLinesText },
            { selectedValue },
            { maxLinesTitle },
            { maxLinesError },
            { hasEraseButton },
            { customInputEnabled },
        )
    }
}

@Parcelize
data class CommonDialogWithSearchMultiselectItemUi(
    override val elementId: Int? = null,
    override val elementEnum: CommonListElementIdentifier? = null,
    val text: StringSource,
    val serverValue: String? = null,
    val originalModel: OriginalModel? = null,
    val maxLines: Int = 3,
    val isSelected: Boolean = false,
    val isSelectAllOption: Boolean = false,
    val selectable: Boolean = true
) : CommonListElementAdapterMarker, CommonDialogWithSearchAdapterMarker, Parcelable {

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
            { text },
            { maxLines },
            { isSelected },
            { selectable },
            { elementId },
            { serverValue },
            { elementEnum },
            { isSelectAllOption },
        )
    }
}
