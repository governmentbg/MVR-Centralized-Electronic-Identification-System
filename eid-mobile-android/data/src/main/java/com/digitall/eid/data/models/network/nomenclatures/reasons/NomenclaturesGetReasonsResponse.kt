package com.digitall.eid.data.models.network.nomenclatures.reasons

import com.google.gson.annotations.SerializedName

data class NomenclaturesReasonsResponse(
    @SerializedName("id") val id: String?,
    @SerializedName("name") val name: String?,
    @SerializedName("nomenclatures") val nomenclatures: List<NomenclatureReasonResponse>?
)

data class NomenclatureReasonResponse(
    @SerializedName("id") val id: String?,
    @SerializedName("name") val name: String?,
    @SerializedName("description") val description: String?,
    @SerializedName("language") val language: String?,
    @SerializedName("textRequired") val textRequired: Boolean?,
    @SerializedName("permittedUser") val permittedUser: String?
)