/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.models.list

import android.os.Parcelable
import androidx.annotation.DrawableRes
import com.digitall.eid.extensions.equalTo
import com.digitall.eid.models.common.ButtonColorUi
import com.digitall.eid.models.common.StringSource
import kotlinx.parcelize.Parcelize

@Parcelize
data class CommonButtonUi(
    override val elementId: Int? = null,
    override val elementEnum: CommonListElementIdentifier,
    val title: StringSource,
    val isEnabled: Boolean = true,
    val buttonColor: ButtonColorUi,
    @param:DrawableRes val iconResLeft: Int? = null,
    @param:DrawableRes val iconResRight: Int? = null,
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
            { isEnabled },
            { elementId },
            { elementEnum },
            { buttonColor },
            { iconResLeft },
            { iconResRight }
        )
    }

}