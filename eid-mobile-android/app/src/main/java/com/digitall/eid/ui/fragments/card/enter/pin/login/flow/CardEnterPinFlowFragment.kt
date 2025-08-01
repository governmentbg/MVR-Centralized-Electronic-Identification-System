package com.digitall.eid.ui.fragments.card.enter.pin.login.flow

import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentFlowContainerBinding
import com.digitall.eid.ui.fragments.base.flow.BaseFlowFragment
import org.koin.androidx.viewmodel.ext.android.viewModel

class CardEnterPinFlowFragment :
    BaseFlowFragment<FragmentFlowContainerBinding, CardEnterPinFlowViewModel>() {

    companion object {
        private const val TAG = "CardEnterPinFlowFragmentTag"
    }

    override fun getViewBinding() = FragmentFlowContainerBinding.inflate(layoutInflater)

    override val viewModel: CardEnterPinFlowViewModel by viewModel()

    override fun getFlowGraph() = R.navigation.nav_card_enter_pin

    override fun getStartDestination() = viewModel.getStartDestination()
}