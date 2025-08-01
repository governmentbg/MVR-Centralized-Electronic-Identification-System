/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.models.network.signing.evrotrust

import com.google.gson.annotations.SerializedName

data class SigningEvrotrustUserStatusResponse(
    @SerializedName("isRegistered") val registered: Boolean?,
    @SerializedName("isIdentified") val identified: Boolean?,
    @SerializedName("isRejected") val rejected: Boolean?,
    @SerializedName("isSupervised") val supervised: Boolean?,
    @SerializedName("isReadyToSign") val readyToSign: Boolean?,
    @SerializedName("hasConfirmedPhone") val hasConfirmedPhone: Boolean?,
    @SerializedName("hasConfirmedEmail") val hasConfirmedEmail: Boolean?,
)
