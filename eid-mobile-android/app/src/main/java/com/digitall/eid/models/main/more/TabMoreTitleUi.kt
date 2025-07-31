/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.models.main.more

import androidx.annotation.DrawableRes
import com.digitall.eid.extensions.equalTo
import com.digitall.eid.models.common.StringSource

class TabMoreTitleUi(
    val itemText: StringSource,
    @param:DrawableRes val itemImageRes: Int,
) : TabMoreAdapterMarker {

    override fun isItemSame(other: Any?): Boolean {
        return equalTo(other)
    }

    override fun isContentSame(other: Any?): Boolean {
        return equalTo(
            other,
            { itemText },
            { itemImageRes },
        )
    }

}