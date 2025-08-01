/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.models.list

import android.os.Parcelable
import com.digitall.eid.domain.models.common.OriginalModel
import com.digitall.eid.extensions.equalTo
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.common.validator.Validator
import kotlinx.parcelize.IgnoredOnParcel
import kotlinx.parcelize.Parcelize

@Parcelize
data class CommonDialogWithSearchUi(
    override val elementId: Int? = null,
    override val elementEnum: CommonListElementIdentifier,
    override val selectedValue: CommonDialogWithSearchItemUi?,
    @IgnoredOnParcel
    @Transient
    override val validators: List<Validator<CommonDialogWithSearchItemUi?>> = emptyList(),
    override var validationError: StringSource? = null,
    val required: Boolean,
    val question: Boolean,
    val title: StringSource,
    val hasEraseButton: Boolean = false,
    val customInputEnabled: Boolean = false,
    val list: List<CommonDialogWithSearchItemUi>,
    val maxLinesTitle: Int = 2,
    val maxLinesText: Int = 1,
    val maxLinesError: Int = 2,
) : CommonValidationFieldUi<CommonDialogWithSearchItemUi?>(
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
data class CommonDialogWithSearchItemUi(
    override val elementId: Int? = null,
    override val elementEnum: CommonListElementIdentifier? = null,
    val text: StringSource,
    val serverValue: String? = null,
    val originalModel: OriginalModel? = null,
    val maxLines: Int = 1,
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
            { selectable },
            { elementId },
            { serverValue },
            { elementEnum },
        )
    }
}
