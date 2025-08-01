/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.models.common

import com.digitall.eid.domain.models.base.TypeEnum

enum class ButtonColorUi(
    override val type: String,
) : TypeEnum {
    BLUE("BLUE"),
    RED("RED"),
    GREEN("GREEN"),
    ORANGE("ORANGE"),
    TRANSPARENT("TRANSPARENT"),
}