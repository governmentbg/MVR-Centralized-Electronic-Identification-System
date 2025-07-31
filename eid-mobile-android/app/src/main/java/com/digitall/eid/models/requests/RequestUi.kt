package com.digitall.eid.models.requests

import android.os.Parcelable
import com.digitall.eid.extensions.equalTo
import kotlinx.parcelize.Parcelize

@Parcelize
data class RequestUi(
    val id: String?,
    val username: String?,
    val levelOfAssurance: String?,
    val requestFrom: RequestFromUi?,
    val createDate: String?,
    val maxTtl: Int?,
    val expiresIn: Long?,
) : RequestAdapterMarker, Parcelable {

    override fun isItemSame(other: Any?): Boolean {
        return equalTo(other)
    }

    override fun isContentSame(other: Any?): Boolean {
        return equalTo(
            other,
            { id },
            { username },
            { levelOfAssurance },
            { createDate },
            { maxTtl },
            { expiresIn },
        )
    }
}

@Parcelize
data class RequestFromUi(
    val type: String?,
    val system: String?,
) : Parcelable