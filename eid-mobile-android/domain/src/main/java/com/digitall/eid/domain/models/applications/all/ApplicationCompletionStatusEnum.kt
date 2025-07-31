package com.digitall.eid.domain.models.applications.all

import android.os.Parcelable
import com.digitall.eid.domain.models.base.TypeEnum
import kotlinx.parcelize.Parcelize

@Parcelize
enum class ApplicationCompletionStatusEnum(override val type: String) : TypeEnum, Parcelable {
    COMPLETED("COMPLETED"),
    DENIED("DENIED"),
    UNKNOWN("UNKNOWN")
}