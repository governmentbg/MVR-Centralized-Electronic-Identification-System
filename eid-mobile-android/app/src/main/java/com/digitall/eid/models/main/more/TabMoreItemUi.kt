/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.models.main.more

import com.digitall.eid.extensions.equalTo
import com.digitall.eid.models.common.StringSource

data class TabMoreItemUi(
    val type: TabMoreItems,
    val itemText: StringSource,
) : TabMoreAdapterMarker {

    override fun isItemSame(other: Any?): Boolean {
        return equalTo(other)
    }

    override fun isContentSame(other: Any?): Boolean {
        return equalTo(
            other,
            { type },
            { itemText },

        )
    }

}