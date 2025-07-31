/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.models.notifications.channels

import com.digitall.eid.extensions.equalTo
import com.digitall.eid.models.common.StringSource

data class NotificationChannelUi(
    val id: String,
    val name: StringSource,
    val isEnabled: Boolean,
    val description: StringSource,
    val isSelected: Boolean,
) : NotificationChannelsAdapterMarker {

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
            { description },
            { isSelected },
        )
    }

}