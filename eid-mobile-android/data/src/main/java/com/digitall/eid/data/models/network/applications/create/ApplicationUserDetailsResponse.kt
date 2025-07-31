/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.models.network.applications.create

import com.google.gson.annotations.SerializedName

data class ApplicationUserDetailsResponse(
    @SerializedName("email") val email: String?,
    @SerializedName("active") val active: Boolean?,
    @SerializedName("lastName") val lastName: String?,
    @SerializedName("firstName") val firstName: String?,
    @SerializedName("secondName") val secondName: String?,
    @SerializedName("lastNameLatin") val lastNameLatin: String?,
    @SerializedName("firstNameLatin") val firstNameLatin: String?,
    @SerializedName("secondNameLatin") val secondNameLatin: String?,
    @SerializedName("eidentityId") val eidentityId: String?,
    @SerializedName("phoneNumber") val phoneNumber: String?,
    @SerializedName("citizenProfileId") val citizenProfileId: String?,
    @SerializedName("citizenIdentifierType") val citizenIdentifierType: String?,
    @SerializedName("citizenIdentifierNumber") val citizenIdentifierNumber: String?,
    @SerializedName("is2FaEnabled") val twoFaEnabled: Boolean?,
)