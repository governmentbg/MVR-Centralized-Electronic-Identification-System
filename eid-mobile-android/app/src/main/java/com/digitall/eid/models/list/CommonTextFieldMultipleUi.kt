package com.digitall.eid.models.list

import android.os.Parcelable
import com.digitall.eid.domain.models.common.OriginalModel
import com.digitall.eid.extensions.equalTo
import com.digitall.eid.models.common.StringSource
import kotlinx.parcelize.Parcelize

@Parcelize
data class CommonTextFieldMultipleUi(
    override val elementId: Int? = null,
    override val elementEnum: CommonListElementIdentifier,
    val required: Boolean,
    val question: Boolean,
    val text: StringSource,
    val title: StringSource,
    val error: StringSource? = null,
    val serverValues: List<String?>? = null,
    val originalModels: List<OriginalModel?>? = null,
    val isEnabled: Boolean = true,
    val maxLinesTitle: Int = 2,
    val maxLinesText: Int = 1,
    val maxLinesError: Int = 2,
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
            { text },
            { title },
            { error },
            { question },
            { required },
            { elementId },
            { elementEnum },
            { maxLinesText },
            { maxLinesTitle },
            { maxLinesError },
        )
    }

}