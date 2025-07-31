/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.repository.network.certificates

import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.certificates.CertificateAliasChangeRequestModel
import com.digitall.eid.domain.models.certificates.CertificateDetailsModel
import com.digitall.eid.domain.models.certificates.CertificateHistoryElementModel
import com.digitall.eid.domain.models.certificates.CertificateStatusChangeModel
import com.digitall.eid.domain.models.certificates.CertificateStatusChangeRequestModel
import com.digitall.eid.domain.models.certificates.CertificatesModel
import kotlinx.coroutines.flow.Flow

interface CertificatesNetworkRepository {

    fun getCertificates(
        page: Int,
        size: Int,
        id: String?,
        sort: String?,
        alias: String?,
        status: String?,
        serialNumber: String?,
        validityFrom: String?,
        validityUntil: String?,
        deviceType: List<String>?,
        eidAdministratorId: String?,
    ): Flow<ResultEmittedData<CertificatesModel>>

    fun getCertificateDetails(
        id: String,
    ): Flow<ResultEmittedData<CertificateDetailsModel>>

    fun getCertificateHistory(
        id: String
    ): Flow<ResultEmittedData<List<CertificateHistoryElementModel>>>

    fun certificateChangeStatus(
        changeRequestModel: CertificateStatusChangeRequestModel
    ): Flow<ResultEmittedData<CertificateStatusChangeModel>>

    fun setCertificateAlias(
        id: String,
        aliasRequestModel: CertificateAliasChangeRequestModel,
    ): Flow<ResultEmittedData<Unit>>

}