/**
 * these constants are collected here so that you can view any constant from this class at the same time
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.models.certificates.filter

import android.os.Parcelable
import com.digitall.eid.domain.models.administrators.AdministratorModel
import com.digitall.eid.models.certificates.all.CertificatesStatusEnum
import kotlinx.parcelize.IgnoredOnParcel
import kotlinx.parcelize.Parcelize

@Parcelize
data class CertificatesFilterModel(
    val id: String?,
    val validityFrom: Long?,
    val validityUntil: Long?,
    val serialNumber: String?,
    val alias: String?,
    val status: CertificatesStatusEnum?,
    val deviceType: CertificateDeviceType?,
    val administrator: AdministratorModel?,
) : Parcelable {

    @IgnoredOnParcel
    val allPropertiesAreNull: Boolean
        get() {
            val primitiveMemberProps = listOf(
                id,
                alias,
                validityFrom,
                validityUntil,
                serialNumber,
                status,
                deviceType,
                administrator,
            )
            val arePrimitivesNotInit = primitiveMemberProps.all { member -> member == null }
            return arePrimitivesNotInit
        }
}