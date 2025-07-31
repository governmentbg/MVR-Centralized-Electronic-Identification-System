/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.models.network.authentication

import com.digitall.eid.domain.models.base.TypeEnum

enum class AuthCreatePassCodeResponseErrorCodes(override val type: String) : TypeEnum {
    PASS_CODE_ALREADY_USED("PASSWORD_ALREADY_USED"),
    PASS_CODE_CONTAINS_PHONE_NUMBER("PASSWORD_CONTAINS_PHONE"),
    PASS_CODE_IS_SEQUENT_DIGITS("PASSWORD_CONTAINS_SEQUENTIAL_DIGITS"),
    PASS_CODE_HAS_REPEATED_DIGITS("PASSWORD_CONTAINS_REPEATED_DIGITS"),
    PASS_CODE_HAS_DATE_OF_BIRTH("PASSWORD_CONTAINS_BIRTHDAY"),
}