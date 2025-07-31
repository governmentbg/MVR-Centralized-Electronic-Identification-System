package com.digitall.eid.ui.fragments.payments.history.list

import com.digitall.eid.models.payments.history.all.PaymentHistoryUi
import com.digitall.eid.utils.DefaultDiffUtilCallback
import com.hannesdorfmann.adapterdelegates4.AsyncListDifferDelegationAdapter
import org.koin.core.component.KoinComponent
import org.koin.core.component.inject

class PaymentsHistoryAdapter :
    AsyncListDifferDelegationAdapter<PaymentHistoryUi>(DefaultDiffUtilCallback()),
    KoinComponent {

    private val paymentsHistoryDelegate: PaymentsHistoryDelegate by inject()

    init {
        delegatesManager.apply {
            addDelegate(paymentsHistoryDelegate)
        }
    }
}