/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.models.journal.to.me

import android.os.Parcelable
import com.digitall.eid.extensions.equalTo
import com.digitall.eid.models.journal.common.all.JournalAdapterMarker
import kotlinx.parcelize.Parcelize

@Parcelize
data class JournalToMeUi(
    val eventId: String,
    val system: String,
    val eventDate: String,
    val eventType: String,
) : JournalAdapterMarker, Parcelable {

    override fun isItemSame(other: Any?): Boolean {
        return equalTo(
            other,
            { eventId },
        )
    }

    override fun isContentSame(other: Any?): Boolean {
        return equalTo(
            other,
            { system },
            { eventId },
            { eventDate },
            { eventType },
        )
    }

}