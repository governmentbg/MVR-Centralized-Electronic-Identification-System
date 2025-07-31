package com.digitall.eid.domain.repository.network.payments

import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.payments.history.PaymentHistoryModel
import kotlinx.coroutines.flow.Flow

interface PaymentsNetworkRepository {

    fun getHistory(): Flow<ResultEmittedData<List<PaymentHistoryModel>>>
}