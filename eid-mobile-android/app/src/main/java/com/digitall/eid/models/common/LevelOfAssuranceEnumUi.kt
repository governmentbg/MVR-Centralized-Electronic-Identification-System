package com.digitall.eid.models.common

import com.digitall.eid.R
import com.digitall.eid.domain.models.base.TypeEnum

enum class LevelOfAssuranceEnumUi(
    override val type: String,
    val title: StringSource
): TypeEnum {
    LOW("LOW", StringSource(R.string.level_of_assurance_enum_low)),
    SUBSTANTIAL("SUBSTANTIAL", StringSource(R.string.level_of_assurance_enum_substantial)),
    HIGH("HIGH", StringSource(R.string.level_of_assurance_enum_high))

}