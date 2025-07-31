package com.digitall.eid.models.auth.start

import com.digitall.eid.domain.models.base.TypeEnum
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonListElementIdentifier
import kotlinx.parcelize.Parcelize

@Parcelize
enum class AuthStartElementsEnumUi(override val type: String, val title: StringSource) :
    CommonListElementIdentifier,
    TypeEnum {

    ENVIRONMENT_SPINNER("ENVIRONMENT_SPINNER", StringSource(""))

}