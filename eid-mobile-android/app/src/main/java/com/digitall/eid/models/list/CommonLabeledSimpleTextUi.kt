package com.digitall.eid.models.list

import android.os.Parcelable
import androidx.annotation.ColorRes
import androidx.annotation.DrawableRes
import com.digitall.eid.R
import com.digitall.eid.extensions.equalTo
import com.digitall.eid.models.common.StringSource
import kotlinx.parcelize.Parcelize

@Parcelize
data class CommonLabeledSimpleTextUi(
    override val elementId: Int? = null,
    override val elementEnum: CommonListElementIdentifier? = null,
    val maxLines: Int = 10,
    val text: StringSource,
    val labeledText: StringSource,
    val title: StringSource,
    @param:ColorRes val labelBackgroundColorRes: Int = R.color.color_F59E0B,
    @param:ColorRes val labelTextColorRes: Int = R.color.color_1C3050,
    @param:DrawableRes val iconResLeft: Int? = null,
    @param:DrawableRes val iconResRight: Int? = null,
) : CommonListElementAdapterMarker, Parcelable {

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
            { title },
            { maxLines },
            { elementId },
            { elementEnum },
            { labelTextColorRes },
            { labelBackgroundColorRes },
        )
    }

}
