/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.models.notifications.notifications

import com.digitall.eid.domain.models.common.SelectionState
import com.digitall.eid.extensions.equalTo

data class NotificationParentUi(
    val id: String,
    val name: String,
    var isOpened: Boolean,
    val selectionState: SelectionState,
) : NotificationAdapterMarker {

    override fun isItemSame(other: Any?): Boolean {
        return equalTo(
            other,
            { id },
        )
    }

    override fun isContentSame(other: Any?): Boolean {
        return equalTo(
            other,
            { id },
            { name },
            { isOpened },
            { selectionState },
        )
    }

}