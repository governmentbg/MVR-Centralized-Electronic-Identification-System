package com.digitall.eid.ui.fragments.applications.payment

import com.digitall.eid.R
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.extensions.isFragmentInBackStack
import com.digitall.eid.extensions.readOnly
import com.digitall.eid.extensions.setValueOnMainThread
import com.digitall.eid.models.main.MainTabsEnum
import com.digitall.eid.ui.BaseViewModel
import com.digitall.eid.utils.SingleLiveEvent

class ApplicationPaymentViewModel : BaseViewModel() {

    companion object {
        private const val TAG = "ApplicationCreatePaymentViewModelTag"
    }

    private val _openPaymentEvent = SingleLiveEvent<String?>()
    val openPaymentEvent = _openPaymentEvent.readOnly()

    override var mainTabsEnum: MainTabsEnum? = MainTabsEnum.TAB_EIM

    private var paymentAccessCode: String? = null

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)

        when {
            findTabNavController().isFragmentInBackStack(R.id.applicationsFragment) -> popBackStackToFragment(
                R.id.applicationsFragment
            )

            else -> popBackStackToFragmentInTab(R.id.mainTabEIMFragment)
        }

    }

    fun openPayment() {
        _openPaymentEvent.setValueOnMainThread(paymentAccessCode).also {
            onBackPressed()
        }
    }

    fun setupModel(paymentAccessCode: String?) {
        this.paymentAccessCode = paymentAccessCode
    }

}