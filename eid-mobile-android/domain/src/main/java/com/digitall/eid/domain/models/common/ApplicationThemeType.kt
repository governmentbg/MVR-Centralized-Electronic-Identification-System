/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.models.common

import com.digitall.eid.domain.models.base.TypeEnum

enum class ApplicationThemeType(override val type: String): TypeEnum {
    FOLLOW_SYSTEM("follow_system"),
    DARK("dark"),
    LIGHT("light")
}