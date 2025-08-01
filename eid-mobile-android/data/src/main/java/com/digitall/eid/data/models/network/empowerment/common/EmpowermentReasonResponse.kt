/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.models.network.empowerment.common

import com.google.gson.annotations.SerializedName

data class EmpowermentReasonResponse(
    @SerializedName("id") val id: String?,
    @SerializedName("translations") val translations: List<EmpowermentReasonTranslationResponse>?,
)

data class EmpowermentReasonTranslationResponse(
    @SerializedName("language") val language: String?,
    @SerializedName("name") val name: String?,
)