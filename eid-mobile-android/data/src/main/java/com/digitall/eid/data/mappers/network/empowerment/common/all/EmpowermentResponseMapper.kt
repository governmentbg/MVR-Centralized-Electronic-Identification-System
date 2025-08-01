/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.mappers.network.empowerment.common.all

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.empowerment.common.all.EmpowermentAuthorizerUidResponseItem
import com.digitall.eid.data.models.network.empowerment.common.all.EmpowermentDisagreementResponseItem
import com.digitall.eid.data.models.network.empowerment.common.all.EmpowermentEmpowererUidResponseItem
import com.digitall.eid.data.models.network.empowerment.common.all.EmpowermentResponse
import com.digitall.eid.data.models.network.empowerment.common.all.EmpowermentResponseItem
import com.digitall.eid.data.models.network.empowerment.common.all.EmpowermentSignatureResponseItem
import com.digitall.eid.data.models.network.empowerment.common.all.EmpowermentStatusHistoryResponseItem
import com.digitall.eid.data.models.network.empowerment.common.all.EmpowermentVolumeOfRepresentationResponseItem
import com.digitall.eid.data.models.network.empowerment.common.all.EmpowermentWithdrawalResponseItem
import com.digitall.eid.data.utils.StrictMapperConfig
import com.digitall.eid.domain.models.empowerment.common.AuthorizedUidModel
import com.digitall.eid.domain.models.empowerment.common.EmpowermentUidModel
import com.digitall.eid.domain.models.empowerment.common.all.EmpowermentDisagreementItem
import com.digitall.eid.domain.models.empowerment.common.all.EmpowermentItem
import com.digitall.eid.domain.models.empowerment.common.all.EmpowermentModel
import com.digitall.eid.domain.models.empowerment.common.all.EmpowermentSignatureItem
import com.digitall.eid.domain.models.empowerment.common.all.EmpowermentStatusHistoryItem
import com.digitall.eid.domain.models.empowerment.common.all.EmpowermentVolumeOfRepresentationItem
import com.digitall.eid.domain.models.empowerment.common.all.EmpowermentWithdrawalItem
import org.mapstruct.Mapper
import org.mapstruct.factory.Mappers

class EmpowermentResponseMapper : BaseMapper<EmpowermentResponse, EmpowermentModel>() {

    @Mapper(config = StrictMapperConfig::class)
    abstract class ModelMapper {

        abstract fun map(from: EmpowermentResponse): EmpowermentModel

        fun mapData(from: EmpowermentResponseItem?): EmpowermentItem? {
            return from?.run {
                EmpowermentItem(
                    id = id,
                    uid = uid,
                    name = name,
                    number = number,
                    status = status,
                    serviceId = serviceId,
                    startDate = startDate,
                    createdOn = createdOn,
                    createdBy = createdBy,
                    expiryDate = expiryDate,
                    onBehalfOf = onBehalfOf,
                    serviceName = serviceName,
                    denialReason = denialReason,
                    providerId = providerId,
                    providerName = providerName,
                    issuerPosition = issuerPosition,
                    xmlRepresentation = xmlRepresentation,
                    calculatedStatusOn = calculatedStatusOn,
                    empoweredUids = empoweredUids?.map(::mapEmpoweredUids),
                    authorizerUids = authorizerUids?.map(::mapAuthorizerUids),
                    statusHistory = statusHistory?.map(::mapStatusHistory),
                    empowermentSignatures = empowermentSignatures?.map(::mapEmpowermentSignatures),
                    empowermentWithdrawals = empowermentWithdrawals?.map(::mapEmpowermentWithdrawals),
                    volumeOfRepresentation = volumeOfRepresentation?.map(::mapVolumeOfRepresentation),
                    empowermentDisagreements = empowermentDisagreements?.map(::mapEmpowermentDisagreements),
                )
            }
        }

        abstract fun mapEmpoweredUids(from: EmpowermentEmpowererUidResponseItem): EmpowermentUidModel
        abstract fun mapAuthorizerUids(from: EmpowermentAuthorizerUidResponseItem): AuthorizedUidModel
        abstract fun mapStatusHistory(from: EmpowermentStatusHistoryResponseItem): EmpowermentStatusHistoryItem
        abstract fun mapEmpowermentWithdrawals(from: EmpowermentWithdrawalResponseItem): EmpowermentWithdrawalItem
        abstract fun mapEmpowermentSignatures(from: EmpowermentSignatureResponseItem): EmpowermentSignatureItem
        abstract fun mapEmpowermentDisagreements(from: EmpowermentDisagreementResponseItem): EmpowermentDisagreementItem
        abstract fun mapVolumeOfRepresentation(from: EmpowermentVolumeOfRepresentationResponseItem): EmpowermentVolumeOfRepresentationItem
    }

    override fun map(from: EmpowermentResponse): EmpowermentModel {
        return Mappers.getMapper(ModelMapper::class.java).map(from)
    }
}