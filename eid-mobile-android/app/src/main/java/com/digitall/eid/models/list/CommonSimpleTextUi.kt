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
import kotlinx.parcelize.Parcelize

@Parcelize
data class CommonSimpleTextUi(
    override val elementId: Int? = null,
    override val elementEnum: CommonListElementIdentifier? = null,
    val maxLines: Int = 3,
    val text: StringSource,
    val title: StringSource,
    val isClickable: Boolean = false,
    val action: CommonActionUi? = null,
    @param:ColorRes val colorRes: Int = R.color.color_1C3050,
    @param:DrawableRes val iconResLeft: Int? = null,
    @param:DrawableRes val iconResRight: Int? = null,
    val originalModel: OriginalModel? = null,
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
            { action },
            { maxLines },
            { elementId },
            { elementEnum },
            { colorRes },
        )
    }

}