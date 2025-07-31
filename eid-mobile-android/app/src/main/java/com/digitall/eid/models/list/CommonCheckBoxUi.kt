/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.models.list

import android.os.Parcelable
import com.digitall.eid.domain.models.common.OriginalModel
import com.digitall.eid.extensions.equalTo
import com.digitall.eid.models.common.StringSource
import kotlinx.parcelize.Parcelize

@Parcelize
data class CommonCheckBoxUi(
    override val elementId: Int? = null,
    override val elementEnum: CommonListElementIdentifier,
    val isChecked: Boolean,
    val title: StringSource,
    val originalModel: OriginalModel? = null,
    val isEnabled: Boolean = true,
    val maxLines: Int = 2,
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
            { title },
            { maxLines },
            { elementId },
            { isChecked },
            { isEnabled },
            { elementEnum },
        )
    }

}