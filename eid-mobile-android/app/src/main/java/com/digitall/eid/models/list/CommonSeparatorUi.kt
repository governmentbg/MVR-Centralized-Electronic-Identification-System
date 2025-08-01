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
data class CommonSeparatorUi(
    override val elementId: Int? = null,
    override val elementEnum: CommonListElementIdentifier? = null,
    val marginLeft: Int = 0,
    val marginTop: Int = 0,
    val marginRight: Int = 0,
    val marginBottom: Int = 0,
) : CommonListElementAdapterMarker, Parcelable {

    override fun isItemSame(other: Any?): Boolean {
        return equalTo(other)
    }

    override fun isContentSame(other: Any?): Boolean {
        return equalTo(
            other,
            { marginLeft },
            { marginTop },
            { marginRight },
            { marginBottom },
        )
    }

}