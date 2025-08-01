package com.digitall.eid.domain.models.common

import com.digitall.eid.domain.models.base.TypeEnum

enum class DeviceType(override val type: String): TypeEnum {
    CHIP_CARD("CHIP_CARD"),
    MOBILE("MOBILE"),
    OTHER("OTHER")
}