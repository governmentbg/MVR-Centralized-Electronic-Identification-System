/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.models.network.empowerment.cancel

import com.google.gson.annotations.SerializedName

data class EmpowermentCancelRequestModel(
    @SerializedName("reason") val reason: String,
)