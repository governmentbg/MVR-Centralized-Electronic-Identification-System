package com.digitall.eid.data.mappers.network.payments.history.response

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.payments.history.response.PaymentHistoryResponse
import com.digitall.eid.data.utils.StrictMapperConfig
import com.digitall.eid.domain.models.payments.history.PaymentHistoryModel
import org.mapstruct.Mapper
import org.mapstruct.factory.Mappers

class PaymentsHistoryResponseMapper :
    BaseMapper<List<PaymentHistoryResponse>, List<PaymentHistoryModel>>() {

    @Mapper(config = StrictMapperConfig::class)
    fun interface ModelMapper {
        fun map(from: List<PaymentHistoryResponse>): List<PaymentHistoryModel>
    }

    override fun map(from: List<PaymentHistoryResponse>): List<PaymentHistoryModel> {
        return Mappers.getMapper(ModelMapper::class.java).map(from)
    }
}