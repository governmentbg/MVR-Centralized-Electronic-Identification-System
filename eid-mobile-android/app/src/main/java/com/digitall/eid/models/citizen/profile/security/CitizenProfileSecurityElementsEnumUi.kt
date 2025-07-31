package com.digitall.eid.models.citizen.profile.security

import com.digitall.eid.R
import com.digitall.eid.domain.models.base.TypeEnum
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonListElementIdentifier
import kotlinx.parcelize.Parcelize

@Parcelize
enum class CitizenProfileSecurityElementsEnumUi(override val type: String, val title: StringSource) :
    CommonListElementIdentifier,
    TypeEnum {
    MULTI_FACTOR_AUTHENTICATION_CHECKBOX(
        "MULTI_FACTOR_AUTHENTICATION_CHECKBOX",
        StringSource(R.string.citizen_profile_security_multi_factor_authentication_title)
    ),
    PROFILE_SECURITY_PIN_CHECKBOX(
        "PROFILE_SECURITY_PIN_CHECKBOX",
        StringSource(R.string.citizen_profile_security_pin_title)
    ),
    PROFILE_SECURITY_BIOMETRICS_CHECKBOX(
        "MULTI_FACTOR_AUTHENTICATION_CHECKBOX",
        StringSource(R.string.citizen_profile_security_biometrics_title)
    ),
}