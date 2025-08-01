/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.models.signing.borica

import com.digitall.eid.domain.models.base.TypeEnum

data class SigningBoricaStatusResponseModel(
    val code: String?,
    val cert: String?,
    val status: String?,
    val message: String?,
    val signature: String?,
    val responseCode: String?,
    val signatureType: String?,
)

enum class SigningBoricaStatusEnum(
    override val type: String,
) : TypeEnum {
    NONE(""),
    ERROR("ERROR"),
    SIGNED("SIGNED"),
    REMOVED("REMOVED"),
    EXPIRED("EXPIRED"),
    RECEIVED("RECEIVED"),
    REJECTED("REJECTED"),
    ARCHIVED("ARCHIVED"),
    IN_PROGRESS("IN_PROGRESS"),
}

enum class SigningBoricaStatusCodeEnum(
    override val type: String,
) : TypeEnum {
    NONE(""),
    ACCEPTED("ACCEPTED"),
    REJECTED("REJECTED"),
    COMPLETED("COMPLETED"),
    IN_PROGRESS("IN_PROGRESS"),
}
