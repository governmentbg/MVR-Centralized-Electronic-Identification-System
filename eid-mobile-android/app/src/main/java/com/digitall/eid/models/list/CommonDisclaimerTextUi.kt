package com.digitall.eid.models.list

import android.os.Parcelable
import com.digitall.eid.extensions.equalTo
import com.digitall.eid.models.common.StringSource
import kotlinx.parcelize.Parcelize

@Parcelize
data class CommonDisclaimerTextUi(
    override val elementId: Int? = null,
    override val elementEnum: CommonListElementIdentifier,
    val text: StringSource,
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
            { text },
        )
    }

}
