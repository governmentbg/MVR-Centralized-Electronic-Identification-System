/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.models.signing.evrotrust

import com.digitall.eid.domain.models.base.TypeEnum

data class SigningEvrotrustStatusResponseModel(
    val status: String?,
    val processing: Boolean?,
)

enum class SigningEvrotrustStatusEnum(
    override val type: String,
) : TypeEnum {
    NONE("0"),
    PENDING("1"),
    SIGNED("2"),
    REJECTED("3"),
    EXPIRED("4"),
    FAILED("5"),
    WITHDRAWN("6"),
    UNDELIVERABLE("7"),
    ON_HOLD("99"),
    UNKNOWN(""),
}