package com.digitall.eid.domain.models.common

import com.digitall.eid.domain.models.base.TypeEnum

enum class LevelOfAssurance(override val type: String) :TypeEnum {
    LOW("LOW"),
    SUBSTANTIAL("SUBSTANTIAL"),
    HIGH("HIGH")
}