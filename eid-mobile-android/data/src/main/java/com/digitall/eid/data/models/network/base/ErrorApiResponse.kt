/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.models.network.base

import com.google.gson.annotations.SerializedName

data class ErrorApiResponse(
    @SerializedName("status") val status: Int?,
    @SerializedName("title") val title: String?,
    @SerializedName("detail") val detail: String?,
    @SerializedName("errors") val errors: Any?,
) : ErrorResponse