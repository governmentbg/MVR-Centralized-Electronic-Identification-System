/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.models.common

import com.digitall.eid.domain.models.base.TypeEnum

enum class SelectionState (override val type: String) : TypeEnum {
    SELECTED("SELECTED"),
    SELECTED_NOT_ACTIVE("SELECTED_NOT_ACTIVE"),
    NOT_SELECTED("NOT_SELECTED"),
    NOT_SELECTED_NOT_ACTIVE("NOT_SELECTED_NOT_ACTIVE"),
    INDETERMINATE("INDETERMINATE"),
    INDETERMINATE_NOT_ACTIVE("INDETERMINATE_NOT_ACTIVE"),
}