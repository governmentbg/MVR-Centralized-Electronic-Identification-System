/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.models.signing.evrotrust

data class SigningEvrotrustUserStatusResponseModel(
    val registered: Boolean?,
    val identified: Boolean?,
    val rejected: Boolean?,
    val supervised: Boolean?,
    val readyToSign: Boolean?,
    val hasConfirmedPhone: Boolean?,
    val hasConfirmedEmail: Boolean?,
)