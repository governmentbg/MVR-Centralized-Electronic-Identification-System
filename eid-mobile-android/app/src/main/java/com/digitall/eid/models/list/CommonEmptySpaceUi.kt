/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.models.list

import android.os.Parcelable
import com.digitall.eid.extensions.equalTo
import kotlinx.parcelize.Parcelize

@Parcelize
data class CommonEmptySpaceUi(
    override val elementId: Int? = null,
    override val elementEnum: CommonListElementIdentifier? = null,
    val size: CommonEmptySpaceSizeEnum,
) : CommonListElementAdapterMarker, Parcelable {

    override fun isItemSame(other: Any?): Boolean {
        return equalTo(other)
    }

    override fun isContentSame(other: Any?): Boolean {
        return equalTo(other)
    }

}

enum class CommonEmptySpaceSizeEnum {
    SIZE_4,
    SIZE_8,
    SIZE_12,
    SIZE_16,
}