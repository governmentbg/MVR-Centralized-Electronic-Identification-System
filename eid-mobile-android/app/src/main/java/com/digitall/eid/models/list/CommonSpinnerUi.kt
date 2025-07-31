/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.models.list

import android.os.Parcelable
import androidx.annotation.ColorRes
import androidx.annotation.DrawableRes
import com.digitall.eid.R
import com.digitall.eid.domain.models.common.OriginalModel
import com.digitall.eid.extensions.equalTo
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.common.validator.Validator
import kotlinx.parcelize.IgnoredOnParcel
import kotlinx.parcelize.Parcelize

@Parcelize
data class CommonSpinnerUi(
    override val elementId: Int? = null,
    override val elementEnum: CommonListElementIdentifier,
    override val selectedValue: CommonSpinnerMenuItemUi?,
    @IgnoredOnParcel
    @Transient
    override val validators: List<Validator<CommonSpinnerMenuItemUi?>> = emptyList(),
    override var validationError: StringSource? = null,
    val required: Boolean? = null,
    val question: Boolean? = null,
    val title: StringSource? = null,
    val hasEraseButton: Boolean? = false,
    val list: List<CommonSpinnerMenuItemUi>,
    val maxLinesTitle: Int = 2,
    val maxLinesText: Int = 1,
    val maxLinesError: Int = 2,
) : CommonValidationFieldUi<CommonSpinnerMenuItemUi?>(
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
            { list },
            { title },
            { validationError },
            { question },
            { required },
            { elementId },
            { elementEnum },
            { maxLinesText },
            { selectedValue },
            { maxLinesTitle },
            { maxLinesError },
            { hasEraseButton },
        )
    }
}

@Parcelize
data class CommonSpinnerMenuItemUi(
    val id: Int? = null,
    val text: StringSource,
    val isSelected: Boolean,
    val serverValue: String? = null,
    @param:DrawableRes val iconRes: Int? = null,
    @param:ColorRes val iconColorRes: Int? = null,
    val originalModel: OriginalModel? = null,
    @param:ColorRes val textColorRes: Int = R.color.color_1C3050,
    val elementEnum: CommonListElementIdentifier? = null,
    val maxLines: Int = 1,
) : Parcelable