/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.models.notifications.notifications

import com.digitall.eid.extensions.equalTo

data class NotificationChildUi(
    val id: String,
    val name: String,
    val parentId: String?,
    val isSelected: Boolean,
    val isMandatory: Boolean?,
) : NotificationAdapterMarker {

    override fun isItemSame(other: Any?): Boolean {
        return equalTo(
            other,
            { id },
            { parentId },
        )
    }

    override fun isContentSame(other: Any?): Boolean {
        return equalTo(
            other,
            { id },
            { name },
            { parentId },
            { isSelected },
        )
    }

}