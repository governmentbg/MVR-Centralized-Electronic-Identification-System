package com.digitall.eid.models.list

import android.os.Parcelable
import com.digitall.eid.extensions.equalTo
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.ui.common.list.CommonActionUi
import kotlinx.parcelize.Parcelize

@Parcelize
data class CommonTitleDescriptionUi(
    override val elementId: Int? = null,
    override val elementEnum: CommonListElementIdentifier? = null,
    val title: StringSource,
    val description: StringSource,
    val action: CommonActionUi? = null,
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
            { elementId },
            { elementEnum },
            { title },
            { description },
            { action }
        )
    }

}