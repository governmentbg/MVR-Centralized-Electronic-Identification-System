/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.models.empowerment.common.filter

import android.os.Parcelable
import com.digitall.eid.domain.models.empowerment.common.EmpowermentUidModel
import kotlinx.parcelize.Parcelize

@Parcelize
data class EmpowermentFilterModel(
    val status: String?,
    val authorizer: String?,
    val onBehalfOf: String?,
    val fromNameOf: String?,
    val eik: String?,
    val serviceName: String?,
    val validToDate: String?,
    val providerName: String?,
    val empowermentNumber: String?,
    val authorizerList: List<String>,
    val onBehalfOfList: List<String>,
    val serviceNameList: List<String>,
    val providerNameList: List<String>,
    val showOnlyNoExpiryDate: Boolean,
    val empoweredIDs: List<EmpowermentUidModel>?,
) : Parcelable {

    val allPropertiesAreNull: Boolean
        get() {
            val primitiveMemberProps = listOf(
                eik,
                status,
                authorizer,
                onBehalfOf,
                fromNameOf,
                serviceName,
                validToDate,
                providerName,
                empowermentNumber,
            )
            val listMemberProps = listOf(
                authorizerList,
                onBehalfOfList,
                serviceNameList,
                providerNameList,
                empoweredIDs
            )
            val arePrimitivesNotInit = primitiveMemberProps.all { member -> member == null }
            val areListNotInit = listMemberProps.all { list -> list.isNullOrEmpty() }
            return arePrimitivesNotInit && areListNotInit && showOnlyNoExpiryDate.not()
        }
}