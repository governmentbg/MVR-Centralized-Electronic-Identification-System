package com.digitall.eid.domain.models.user

import android.os.Parcelable
import com.digitall.eid.domain.models.base.TypeEnum
import kotlinx.parcelize.Parcelize

@Parcelize
enum class UserAcrEnum(override val type: String): TypeEnum, Parcelable {
    LOW("eid_low"),
    SUBSTANTIAL("eid_substantial"),
    HIGH("eid_high");
}