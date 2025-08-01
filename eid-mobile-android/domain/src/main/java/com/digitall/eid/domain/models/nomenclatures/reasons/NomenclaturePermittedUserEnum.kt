package com.digitall.eid.domain.models.nomenclatures.reasons

import android.os.Parcelable
import com.digitall.eid.domain.models.base.TypeEnum
import kotlinx.parcelize.Parcelize

@Parcelize
enum class NomenclaturePermittedUserEnum(override val type: String): TypeEnum, Parcelable {
    PRIVATE("PRIVATE"),
    ADMIN("ADMIN"),
    PUBLIC("PUBLIC");
}