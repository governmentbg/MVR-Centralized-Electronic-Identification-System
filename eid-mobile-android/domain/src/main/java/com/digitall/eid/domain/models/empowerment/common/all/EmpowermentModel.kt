/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.models.empowerment.common.all

import android.os.Parcelable
import com.digitall.eid.domain.models.common.OriginalModel
import com.digitall.eid.domain.models.empowerment.common.AuthorizedUidModel
import com.digitall.eid.domain.models.empowerment.common.EmpowermentUidModel
import kotlinx.parcelize.Parcelize

@Parcelize
data class EmpowermentModel(
    val pageIndex: Int?,
    val totalItems: Int?,
    val data: List<EmpowermentItem>?,
) : Parcelable

@Parcelize
data class EmpowermentItem(
    val id: String,
    val uid: String?,
    val name: String?,
    val number: String?,
    val status: String?,
    val serviceId: String?,
    val startDate: String?,
    val createdOn: String?,
    val createdBy: String?,
    val expiryDate: String?,
    val onBehalfOf: String?,
    val serviceName: String?,
    val denialReason: String?,
    val providerId: String?,
    val providerName: String?,
    val issuerPosition: String?,
    val xmlRepresentation: String?,
    val calculatedStatusOn: String?,
    val empoweredUids: List<EmpowermentUidModel>?,
    val authorizerUids: List<AuthorizedUidModel>?,
    val statusHistory: List<EmpowermentStatusHistoryItem>?,
    val empowermentWithdrawals: List<EmpowermentWithdrawalItem>?,
    val empowermentSignatures: List<EmpowermentSignatureItem>?,
    val empowermentDisagreements: List<EmpowermentDisagreementItem>?,
    val volumeOfRepresentation: List<EmpowermentVolumeOfRepresentationItem>?,
) : Parcelable, OriginalModel

@Parcelize
data class EmpowermentSignatureItem(
    val dateTime: String?,
    val signerUid: String?,
) : Parcelable

@Parcelize
data class EmpowermentVolumeOfRepresentationItem(
    val code: String?,
    val name: String?,
) : Parcelable

@Parcelize
data class EmpowermentWithdrawalItem(
    val status: String?,
    val reason: String?,
    val issuerUid: String?,
    val startDateTime: String?,
    val activeDateTime: String?,
) : Parcelable

@Parcelize
data class EmpowermentDisagreementItem(
    val reason: String?,
    val issuerUid: String?,
    val activeDateTime: String?,
) : Parcelable

@Parcelize
data class EmpowermentStatusHistoryItem(
    val id: String?,
    val status: String?,
    val dateTime: String?,
) : Parcelable